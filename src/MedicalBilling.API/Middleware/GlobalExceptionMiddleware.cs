using System.Net;
using System.Text.Json;
using MedicalBilling.Application.Exceptions;

namespace MedicalBilling.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// Provides centralized error handling with structured responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = Guid.NewGuid().ToString();
        
        _logger.LogError(exception, "An error occurred. CorrelationId: {CorrelationId}", correlationId);
        
        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message, null),
            ValidationException validation => (HttpStatusCode.BadRequest, validation.Message, validation.Errors),
            UnauthorizedException unauthorized => (HttpStatusCode.Unauthorized, unauthorized.Message, null),
            ServiceException service => (HttpStatusCode.BadRequest, service.Message, null),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred", null)
        };
        
        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
