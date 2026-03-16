namespace MedicalBilling.Domain.Entities;

/// <summary>
/// Represents a medical service or procedure offered by a seller
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    public int SellerId { get; set; }
    
    public string ServiceCode { get; set; } = string.Empty;
    
    public string ServiceName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// German EBM (Einheitlicher Bewertungsmaßstab) billing code
    /// </summary>
    public string? EBMCode { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property: Many-to-one with Seller
    public Seller Seller { get; set; } = null!;
    
    // Navigation property: One product can be used in many billing procedures
    public ICollection<BillingProcedure> BillingProcedures { get; set; } = new List<BillingProcedure>();
}
