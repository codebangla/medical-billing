namespace MedicalBilling.Application.DTOs;

/// <summary>
/// Data Transfer Object for Invoice entity
/// </summary>
public class InvoiceDto
{
    public int Id { get; set; }
    
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public int PatientId { get; set; }
    
    public string PatientName { get; set; } = string.Empty;
    
    public DateTime InvoiceDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string Status { get; set; } = "Draft";
    
    public DateTime? PaymentDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public List<BillingProcedureDto> BillingProcedures { get; set; } = new();
}
