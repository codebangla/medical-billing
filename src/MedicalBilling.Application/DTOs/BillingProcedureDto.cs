namespace MedicalBilling.Application.DTOs;

/// <summary>
/// Data Transfer Object for BillingProcedure entity
/// </summary>
public class BillingProcedureDto
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public string PatientName { get; set; } = string.Empty;
    
    public int ProductId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public int? InvoiceId { get; set; }
    
    public string? InvoiceNumber { get; set; }
    
    public DateTime ProcedureDate { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string Form3ReferenceNumber { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
