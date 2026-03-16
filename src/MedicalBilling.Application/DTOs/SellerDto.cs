namespace MedicalBilling.Application.DTOs;

/// <summary>
/// Data Transfer Object for Seller entity
/// </summary>
public class SellerDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string LicenseNumber { get; set; } = string.Empty;
    
    public string? Specialty { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
