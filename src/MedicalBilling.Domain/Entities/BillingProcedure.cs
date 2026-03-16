namespace MedicalBilling.Domain.Entities;

/// <summary>
/// Represents a billing procedure based on Form 3 (AAV85) data
/// Core business entity for outpatient medical billing
/// </summary>
public class BillingProcedure
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int ProductId { get; set; }
    
    public int? InvoiceId { get; set; }
    
    public DateTime ProcedureDate { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Official Form 3 reference number for German medical billing
    /// </summary>
    public string Form3ReferenceNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Status: Pending, Approved, Rejected, Paid
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    
    public Product Product { get; set; } = null!;
    
    public Invoice? Invoice { get; set; }
}
