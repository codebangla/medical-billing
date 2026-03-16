using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Exceptions;
using MedicalBilling.Application.Interfaces;
using MedicalBilling.Application.Mapping;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Domain.Interfaces;

namespace MedicalBilling.Application.Services;

/// <summary>
/// Service implementation for Product operations with seller relationship validation
/// </summary>
public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Seller> _sellerRepository;
    
    public ProductService(IRepository<Product> productRepository, IRepository<Seller> sellerRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _sellerRepository = sellerRepository ?? throw new ArgumentNullException(nameof(sellerRepository));
    }
    
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Product ID must be greater than 0");
            
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? CustomMapper.ToDto(product) : null;
    }
    
    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return CustomMapper.ToDtoList(products);
    }
    
    public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetProductsPagedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ValidationException("Page number must be greater than 0");
        if (pageSize < 1 || pageSize > 100)
            throw new ValidationException("Page size must be between 1 and 100");
            
        var (items, totalCount) = await _productRepository.GetPagedAsync(
            pageNumber, 
            pageSize,
            orderBy: q => q.OrderByDescending(p => p.Id)
        );
        
        return (CustomMapper.ToDtoList(items), totalCount);
    }
    
    public async Task<IEnumerable<ProductDto>> GetProductsBySellerIdAsync(int sellerId)
    {
        if (sellerId <= 0)
            throw new ValidationException("Seller ID must be greater than 0");
            
        var products = await _productRepository.FindAsync(p => p.SellerId == sellerId);
        return CustomMapper.ToDtoList(products);
    }
    
    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllProductsAsync();
            
        var products = await _productRepository.FindAsync(p => 
            p.ServiceName.Contains(searchTerm) || 
            p.ServiceCode.Contains(searchTerm) ||
            (p.Description != null && p.Description.Contains(searchTerm)) ||
            (p.EBMCode != null && p.EBMCode.Contains(searchTerm))
        );
        
        return CustomMapper.ToDtoList(products);
    }
    
    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        // Validation
        ValidateProductDto(productDto);
        
        // Verify seller exists
        var sellerExists = await _sellerRepository.ExistsAsync(productDto.SellerId);
        if (!sellerExists)
            throw new ValidationException($"Seller with ID {productDto.SellerId} does not exist");
        
        var product = CustomMapper.ToEntity(productDto);
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        
        var createdProduct = await _productRepository.AddAsync(product);
        return CustomMapper.ToDto(createdProduct);
    }
    
    public async Task<ProductDto> UpdateProductAsync(int id, ProductDto productDto)
    {
        if (id <= 0)
            throw new ValidationException("Product ID must be greater than 0");
            
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            throw new NotFoundException(nameof(Product), id);
        
        // Validation
        ValidateProductDto(productDto);
        
        // Verify seller exists
        var sellerExists = await _sellerRepository.ExistsAsync(productDto.SellerId);
        if (!sellerExists)
            throw new ValidationException($"Seller with ID {productDto.SellerId} does not exist");
        
        // Update properties
        existingProduct.SellerId = productDto.SellerId;
        existingProduct.ServiceCode = productDto.ServiceCode;
        existingProduct.ServiceName = productDto.ServiceName;
        existingProduct.Description = productDto.Description;
        existingProduct.UnitPrice = productDto.UnitPrice;
        existingProduct.EBMCode = productDto.EBMCode;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        await _productRepository.UpdateAsync(existingProduct);
        return CustomMapper.ToDto(existingProduct);
    }
    
    public async Task DeleteProductAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Product ID must be greater than 0");
            
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException(nameof(Product), id);
        
        await _productRepository.DeleteAsync(id);
    }
    
    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _productRepository.ExistsAsync(id);
    }
    
    private void ValidateProductDto(ProductDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        
        if (dto.SellerId <= 0)
            errors.Add(nameof(dto.SellerId), new[] { "Seller ID is required" });
            
        if (string.IsNullOrWhiteSpace(dto.ServiceCode))
            errors.Add(nameof(dto.ServiceCode), new[] { "Service code is required" });
        else if (dto.ServiceCode.Length > 50)
            errors.Add(nameof(dto.ServiceCode), new[] { "Service code cannot exceed 50 characters" });
            
        if (string.IsNullOrWhiteSpace(dto.ServiceName))
            errors.Add(nameof(dto.ServiceName), new[] { "Service name is required" });
        else if (dto.ServiceName.Length > 200)
            errors.Add(nameof(dto.ServiceName), new[] { "Service name cannot exceed 200 characters" });
            
        if (dto.Description != null && dto.Description.Length > 500)
            errors.Add(nameof(dto.Description), new[] { "Description cannot exceed 500 characters" });
            
        if (dto.UnitPrice < 0)
            errors.Add(nameof(dto.UnitPrice), new[] { "Unit price cannot be negative" });
        else if (dto.UnitPrice > 999999.99m)
            errors.Add(nameof(dto.UnitPrice), new[] { "Unit price cannot exceed 999,999.99" });
            
        if (dto.EBMCode != null && dto.EBMCode.Length > 50)
            errors.Add(nameof(dto.EBMCode), new[] { "EBM code cannot exceed 50 characters" });
        
        if (errors.Any())
            throw new ValidationException(errors);
    }
}
