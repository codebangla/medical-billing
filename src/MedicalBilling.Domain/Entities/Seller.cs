namespace MedicalBilling.Domain.Entities;

/// <summary>
/// Represents a medical service provider (doctor, clinic, hospital)
/// </summary>
public class Seller
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string LicenseNumber { get; set; } = string.Empty;
    
    public string? Specialty { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property: One seller has many products
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
