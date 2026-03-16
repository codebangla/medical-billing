namespace MedicalBilling.Domain.Entities;

/// <summary>
/// Represents an invoice aggregating multiple billing procedures
/// </summary>
public class Invoice
{
    public int Id { get; set; }
    
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public int PatientId { get; set; }
    
    public DateTime InvoiceDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Status: Draft, Sent, Paid, Overdue, Cancelled
    /// </summary>
    public string Status { get; set; } = "Draft";
    
    public DateTime? PaymentDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    
    public ICollection<BillingProcedure> BillingProcedures { get; set; } = new List<BillingProcedure>();
}
