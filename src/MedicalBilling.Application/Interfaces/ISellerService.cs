using MedicalBilling.Application.DTOs;

namespace MedicalBilling.Application.Interfaces;

/// <summary>
/// Service interface for Seller operations
/// </summary>
public interface ISellerService
{
    Task<SellerDto?> GetSellerByIdAsync(int id);
    Task<IEnumerable<SellerDto>> GetAllSellersAsync();
    Task<(IEnumerable<SellerDto> Items, int TotalCount)> GetSellersPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<SellerDto>> SearchSellersAsync(string searchTerm);
    Task<SellerDto> CreateSellerAsync(SellerDto sellerDto);
    Task<SellerDto> UpdateSellerAsync(int id, SellerDto sellerDto);
    Task DeleteSellerAsync(int id);
    Task<bool> SellerExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null);
}
