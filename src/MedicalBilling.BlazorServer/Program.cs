using MedicalBilling.BlazorServer.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// 1. Register TokenProvider (Scoped) - Keeping for legacy, but moving to Store
builder.Services.AddScoped<MedicalBilling.BlazorServer.Services.TokenProvider>();
builder.Services.AddSingleton<MedicalBilling.BlazorServer.Services.ServerSideTokenStore>();

// 2. Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 3. Register TokenHandler (Transient)
builder.Services.AddTransient<MedicalBilling.BlazorServer.Services.TokenHandler>();

// 4. Configure Typed Clients with TokenHandler
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5236";

builder.Services.AddHttpClient<MedicalBilling.BlazorServer.Services.SellerService>(client => 
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<MedicalBilling.BlazorServer.Services.TokenHandler>();

builder.Services.AddHttpClient<MedicalBilling.BlazorServer.Services.ProductService>(client => 
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<MedicalBilling.BlazorServer.Services.TokenHandler>();

builder.Services.AddHttpClient<MedicalBilling.BlazorServer.Services.PatientService>(client => 
    client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<MedicalBilling.BlazorServer.Services.TokenHandler>();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.ClientId = builder.Configuration["Keycloak:ClientId"];
    options.ClientSecret = builder.Configuration["Keycloak:ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.RequireHttpsMetadata = false; // For local development
    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "realm_access.roles"
    };

    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var accessToken = context.SecurityToken.RawData;
            var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
            
            if (identity != null && !string.IsNullOrEmpty(accessToken))
            {
                identity.AddClaim(new System.Security.Claims.Claim("access_token", accessToken));
                
                // CRITICAL: Save to Global Store immediately!
                var username = identity.Name ?? identity.FindFirst("preferred_username")?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    var tokenStore = context.HttpContext.RequestServices.GetRequiredService<MedicalBilling.BlazorServer.Services.ServerSideTokenStore>();
                    tokenStore.SaveToken(username, accessToken);
                    Console.WriteLine($"[Program.cs] SAVED TOKEN for {username} in OnTokenValidated");
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
