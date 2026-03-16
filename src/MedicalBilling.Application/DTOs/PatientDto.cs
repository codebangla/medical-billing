namespace MedicalBilling.Application.DTOs;

/// <summary>
/// Data Transfer Object for Patient entity
/// </summary>
public class PatientDto
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    public string InsuranceNumber { get; set; } = string.Empty;
    
    public string InsuranceProvider { get; set; } = string.Empty;
    
    public string? ContactInfo { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
