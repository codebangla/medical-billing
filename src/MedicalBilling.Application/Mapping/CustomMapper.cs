using MedicalBilling.Application.DTOs;
using MedicalBilling.Domain.Entities;

namespace MedicalBilling.Application.Mapping;

/// <summary>
/// Custom object mapper without external dependencies
/// Provides manual property mapping between entities and DTOs
/// </summary>
public static class CustomMapper
{
    #region Seller Mapping
    
    public static SellerDto ToDto(Seller entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        return new SellerDto
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            LicenseNumber = entity.LicenseNumber ?? string.Empty,
            Specialty = entity.Specialty,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    
    public static Seller ToEntity(SellerDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        return new Seller
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            LicenseNumber = dto.LicenseNumber ?? string.Empty,
            Specialty = dto.Specialty,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    
    public static IEnumerable<SellerDto> ToDtoList(IEnumerable<Seller> entities)
    {
        return entities?.Select(ToDto) ?? Enumerable.Empty<SellerDto>();
    }
    
    #endregion
    
    #region Product Mapping
    
    public static ProductDto ToDto(Product entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        return new ProductDto
        {
            Id = entity.Id,
            SellerId = entity.SellerId,
            SellerName = entity.Seller?.Name ?? string.Empty,
            ServiceCode = entity.ServiceCode ?? string.Empty,
            ServiceName = entity.ServiceName ?? string.Empty,
            Description = entity.Description,
            UnitPrice = entity.UnitPrice,
            EBMCode = entity.EBMCode,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    
    public static Product ToEntity(ProductDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        return new Product
        {
            Id = dto.Id,
            SellerId = dto.SellerId,
            ServiceCode = dto.ServiceCode ?? string.Empty,
            ServiceName = dto.ServiceName ?? string.Empty,
            Description = dto.Description,
            UnitPrice = dto.UnitPrice,
            EBMCode = dto.EBMCode,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    
    public static IEnumerable<ProductDto> ToDtoList(IEnumerable<Product> entities)
    {
        return entities?.Select(ToDto) ?? Enumerable.Empty<ProductDto>();
    }
    
    #endregion
    
    #region Patient Mapping
    
    public static PatientDto ToDto(Patient entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        return new PatientDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName ?? string.Empty,
            LastName = entity.LastName ?? string.Empty,
            DateOfBirth = entity.DateOfBirth,
            InsuranceNumber = entity.InsuranceNumber ?? string.Empty,
            InsuranceProvider = entity.InsuranceProvider ?? string.Empty,
            ContactInfo = entity.ContactInfo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    
    public static Patient ToEntity(PatientDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        return new Patient
        {
            Id = dto.Id,
            FirstName = dto.FirstName ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            DateOfBirth = dto.DateOfBirth,
            InsuranceNumber = dto.InsuranceNumber ?? string.Empty,
            InsuranceProvider = dto.InsuranceProvider ?? string.Empty,
            ContactInfo = dto.ContactInfo,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    
    public static IEnumerable<PatientDto> ToDtoList(IEnumerable<Patient> entities)
    {
        return entities?.Select(ToDto) ?? Enumerable.Empty<PatientDto>();
    }
    
    #endregion
    
    #region BillingProcedure Mapping
    
    public static BillingProcedureDto ToDto(BillingProcedure entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        return new BillingProcedureDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            PatientName = entity.Patient != null 
                ? $"{entity.Patient.FirstName} {entity.Patient.LastName}" 
                : string.Empty,
            ProductId = entity.ProductId,
            ProductName = entity.Product?.ServiceName ?? string.Empty,
            InvoiceId = entity.InvoiceId,
            InvoiceNumber = entity.Invoice?.InvoiceNumber,
            ProcedureDate = entity.ProcedureDate,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            TotalAmount = entity.TotalAmount,
            Form3ReferenceNumber = entity.Form3ReferenceNumber ?? string.Empty,
            Status = entity.Status ?? "Pending",
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    
    public static BillingProcedure ToEntity(BillingProcedureDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        return new BillingProcedure
        {
            Id = dto.Id,
            PatientId = dto.PatientId,
            ProductId = dto.ProductId,
            InvoiceId = dto.InvoiceId,
            ProcedureDate = dto.ProcedureDate,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            TotalAmount = dto.TotalAmount,
            Form3ReferenceNumber = dto.Form3ReferenceNumber ?? string.Empty,
            Status = dto.Status ?? "Pending",
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    
    public static IEnumerable<BillingProcedureDto> ToDtoList(IEnumerable<BillingProcedure> entities)
    {
        return entities?.Select(ToDto) ?? Enumerable.Empty<BillingProcedureDto>();
    }
    
    #endregion
    
    #region Invoice Mapping
    
    public static InvoiceDto ToDto(Invoice entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        return new InvoiceDto
        {
            Id = entity.Id,
            InvoiceNumber = entity.InvoiceNumber ?? string.Empty,
            PatientId = entity.PatientId,
            PatientName = entity.Patient != null 
                ? $"{entity.Patient.FirstName} {entity.Patient.LastName}" 
                : string.Empty,
            InvoiceDate = entity.InvoiceDate,
            TotalAmount = entity.TotalAmount,
            Status = entity.Status ?? "Draft",
            PaymentDate = entity.PaymentDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            BillingProcedures = entity.BillingProcedures != null 
                ? ToDtoList(entity.BillingProcedures).ToList() 
                : new List<BillingProcedureDto>()
        };
    }
    
    public static Invoice ToEntity(InvoiceDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        return new Invoice
        {
            Id = dto.Id,
            InvoiceNumber = dto.InvoiceNumber ?? string.Empty,
            PatientId = dto.PatientId,
            InvoiceDate = dto.InvoiceDate,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status ?? "Draft",
            PaymentDate = dto.PaymentDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
    
    public static IEnumerable<InvoiceDto> ToDtoList(IEnumerable<Invoice> entities)
    {
        return entities?.Select(ToDto) ?? Enumerable.Empty<InvoiceDto>();
    }
    
    #endregion
}
