namespace MedicalBilling.Application.Exceptions;

/// <summary>
/// Base exception for service layer
/// </summary>
public class ServiceException : Exception
{
    public ServiceException(string message) : base(message)
    {
    }
    
    public ServiceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when entity is not found
/// </summary>
public class NotFoundException : ServiceException
{
    public NotFoundException(string entityName, int id) 
        : base($"{entityName} with ID {id} not found")
    {
    }
    
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown for validation errors
/// </summary>
public class ValidationException : ServiceException
{
    public Dictionary<string, string[]> Errors { get; }
    
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Validation failed")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown for unauthorized access
/// </summary>
public class UnauthorizedException : ServiceException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
