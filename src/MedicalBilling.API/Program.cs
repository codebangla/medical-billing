using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MedicalBilling.API.Middleware;
using MedicalBilling.Application.Interfaces;
using MedicalBilling.Application.Services;
using MedicalBilling.Domain.Interfaces;
using MedicalBilling.Infrastructure.Data;
using MedicalBilling.Infrastructure.Repositories;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Medical Billing API",
        Version = "v1",
        Description = "REST API for Medical Billing System with Form 3 (AAV85) support"
    });
    
    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Repository registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Service registration
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPatientService, PatientService>();

// Authentication with Keycloak (JWT Bearer)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false, // DEBUG: Disabled
            ValidateAudience = false,
            ValidateLifetime = false, // DEBUG: Disabled
            ValidateIssuerSigningKey = false, // DEBUG: Disabled - Trust any signature
            SignatureValidator = delegate (string token, Microsoft.IdentityModel.Tokens.TokenValidationParameters parameters)
            {
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token);
                return jwt;
            },
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "preferred_username",
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    // Map realm_access roles
                    var realmAccess = context.Principal?.Claims.FirstOrDefault(c => c.Type == "realm_access");
                    if (realmAccess != null)
                    {
                        try 
                        {
                            using var doc = System.Text.Json.JsonDocument.Parse(realmAccess.Value);
                            if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                            {
                                foreach (var role in rolesElement.EnumerateArray())
                                {
                                    var roleValue = role.GetString();
                                    if (!string.IsNullOrEmpty(roleValue))
                                    {
                                        claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, roleValue));
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Ignore parsing errors
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Rate Limiting (100 requests per minute per IP)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Response Caching
builder.Services.AddResponseCaching();

// Memory Cache
builder.Services.AddMemoryCache();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Billing API v1");
    });
    
    // Seed Database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await MedicalBilling.Infrastructure.Data.DbSeeder.SeedAsync(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Seeding failed: {ex.Message}");
        }
    }
}

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCaching();

app.UseRateLimiter();

app.UseAuthentication();


// DEBUG: Log Authorization Header
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (authHeader != null)
    {
        Console.WriteLine($"[DEBUG] Received Token: {authHeader.Substring(0, Math.Min(authHeader.Length, 50))}...");
    }
    else 
    {
        Console.WriteLine("[DEBUG] No Authorization Header received");
    }
    await next();
});

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();
