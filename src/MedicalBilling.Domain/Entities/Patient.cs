namespace MedicalBilling.Domain.Entities;

/// <summary>
/// Represents a patient in the medical billing system
/// </summary>
public class Patient
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
    
    // Navigation properties
    public ICollection<BillingProcedure> BillingProcedures { get; set; } = new List<BillingProcedure>();
    
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
