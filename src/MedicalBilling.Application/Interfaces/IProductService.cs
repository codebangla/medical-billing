using MedicalBilling.Application.DTOs;

namespace MedicalBilling.Application.Interfaces;

/// <summary>
/// Service interface for Product operations
/// </summary>
public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetProductsPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<ProductDto>> GetProductsBySellerIdAsync(int sellerId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}
