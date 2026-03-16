using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Exceptions;
using MedicalBilling.Application.Interfaces;
using MedicalBilling.Application.Mapping;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Domain.Interfaces;

namespace MedicalBilling.Application.Services;

/// <summary>
/// Service implementation for Seller operations with business logic and validation
/// </summary>
public class SellerService : ISellerService
{
    private readonly IRepository<Seller> _sellerRepository;
    
    public SellerService(IRepository<Seller> sellerRepository)
    {
        _sellerRepository = sellerRepository ?? throw new ArgumentNullException(nameof(sellerRepository));
    }
    
    public async Task<SellerDto?> GetSellerByIdAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Seller ID must be greater than 0");
            
        var seller = await _sellerRepository.GetByIdAsync(id);
        return seller != null ? CustomMapper.ToDto(seller) : null;
    }
    
    public async Task<IEnumerable<SellerDto>> GetAllSellersAsync()
    {
        var sellers = await _sellerRepository.GetAllAsync();
        return CustomMapper.ToDtoList(sellers);
    }
    
    public async Task<(IEnumerable<SellerDto> Items, int TotalCount)> GetSellersPagedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ValidationException("Page number must be greater than 0");
        if (pageSize < 1 || pageSize > 100)
            throw new ValidationException("Page size must be between 1 and 100");
            
        var (items, totalCount) = await _sellerRepository.GetPagedAsync(
            pageNumber, 
            pageSize,
            orderBy: q => q.OrderByDescending(s => s.Id)
        );
        
        return (CustomMapper.ToDtoList(items), totalCount);
    }
    
    public async Task<IEnumerable<SellerDto>> SearchSellersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllSellersAsync();
            
        var sellers = await _sellerRepository.FindAsync(s => 
            s.Name.Contains(searchTerm) || 
            s.Email.Contains(searchTerm) ||
            s.LicenseNumber.Contains(searchTerm) ||
            (s.Specialty != null && s.Specialty.Contains(searchTerm))
        );
        
        return CustomMapper.ToDtoList(sellers);
    }
    
    public async Task<SellerDto> CreateSellerAsync(SellerDto sellerDto)
    {
        // Validation
        ValidateSellerDto(sellerDto);
        
        // Check for duplicate email
        if (await EmailExistsAsync(sellerDto.Email))
            throw new ValidationException($"Email '{sellerDto.Email}' is already in use");
            
        // Check for duplicate license number
        if (await LicenseNumberExistsAsync(sellerDto.LicenseNumber))
            throw new ValidationException($"License number '{sellerDto.LicenseNumber}' is already in use");
        
        var seller = CustomMapper.ToEntity(sellerDto);
        seller.CreatedAt = DateTime.UtcNow;
        seller.UpdatedAt = DateTime.UtcNow;
        
        var createdSeller = await _sellerRepository.AddAsync(seller);
        return CustomMapper.ToDto(createdSeller);
    }
    
    public async Task<SellerDto> UpdateSellerAsync(int id, SellerDto sellerDto)
    {
        if (id <= 0)
            throw new ValidationException("Seller ID must be greater than 0");
            
        var existingSeller = await _sellerRepository.GetByIdAsync(id);
        if (existingSeller == null)
            throw new NotFoundException(nameof(Seller), id);
        
        // Validation
        ValidateSellerDto(sellerDto);
        
        // Check for duplicate email (excluding current seller)
        if (await EmailExistsAsync(sellerDto.Email, id))
            throw new ValidationException($"Email '{sellerDto.Email}' is already in use");
            
        // Check for duplicate license number (excluding current seller)
        if (await LicenseNumberExistsAsync(sellerDto.LicenseNumber, id))
            throw new ValidationException($"License number '{sellerDto.LicenseNumber}' is already in use");
        
        // Update properties
        existingSeller.Name = sellerDto.Name;
        existingSeller.Email = sellerDto.Email;
        existingSeller.LicenseNumber = sellerDto.LicenseNumber;
        existingSeller.Specialty = sellerDto.Specialty;
        existingSeller.UpdatedAt = DateTime.UtcNow;
        
        await _sellerRepository.UpdateAsync(existingSeller);
        return CustomMapper.ToDto(existingSeller);
    }
    
    public async Task DeleteSellerAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Seller ID must be greater than 0");
            
        var exists = await _sellerRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException(nameof(Seller), id);
        
        await _sellerRepository.DeleteAsync(id);
    }
    
    public async Task<bool> SellerExistsAsync(int id)
    {
        return await _sellerRepository.ExistsAsync(id);
    }
    
    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        var sellers = await _sellerRepository.FindAsync(s => s.Email == email);
        
        if (excludeId.HasValue)
            sellers = sellers.Where(s => s.Id != excludeId.Value);
            
        return sellers.Any();
    }
    
    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            return false;
            
        var sellers = await _sellerRepository.FindAsync(s => s.LicenseNumber == licenseNumber);
        
        if (excludeId.HasValue)
            sellers = sellers.Where(s => s.Id != excludeId.Value);
            
        return sellers.Any();
    }
    
    private void ValidateSellerDto(SellerDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(nameof(dto.Name), new[] { "Name is required" });
        else if (dto.Name.Length > 200)
            errors.Add(nameof(dto.Name), new[] { "Name cannot exceed 200 characters" });
            
        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add(nameof(dto.Email), new[] { "Email is required" });
        else if (dto.Email.Length > 200)
            errors.Add(nameof(dto.Email), new[] { "Email cannot exceed 200 characters" });
        else if (!IsValidEmail(dto.Email))
            errors.Add(nameof(dto.Email), new[] { "Email format is invalid" });
            
        if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
            errors.Add(nameof(dto.LicenseNumber), new[] { "License number is required" });
        else if (dto.LicenseNumber.Length > 100)
            errors.Add(nameof(dto.LicenseNumber), new[] { "License number cannot exceed 100 characters" });
            
        if (dto.Specialty != null && dto.Specialty.Length > 100)
            errors.Add(nameof(dto.Specialty), new[] { "Specialty cannot exceed 100 characters" });
        
        if (errors.Any())
            throw new ValidationException(errors);
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
