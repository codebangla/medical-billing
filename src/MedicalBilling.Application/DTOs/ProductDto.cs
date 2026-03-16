namespace MedicalBilling.Application.DTOs;

/// <summary>
/// Data Transfer Object for Product entity
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    
    public int SellerId { get; set; }
    
    public string SellerName { get; set; } = string.Empty;
    
    public string ServiceCode { get; set; } = string.Empty;
    
    public string ServiceName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public string? EBMCode { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
