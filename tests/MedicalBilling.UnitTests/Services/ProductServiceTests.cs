using Xunit;
using Moq;
using MedicalBilling.Application.Services;
using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Exceptions;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Domain.Interfaces;

namespace MedicalBilling.UnitTests.Services;

/// <summary>
/// Unit tests for ProductService
/// Tests seller-product relationships and business logic
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _mockProductRepository;
    private readonly Mock<IRepository<Seller>> _mockSellerRepository;
    private readonly ProductService _service;
    
    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IRepository<Product>>();
        _mockSellerRepository = new Mock<IRepository<Seller>>();
        _service = new ProductService(_mockProductRepository.Object, _mockSellerRepository.Object);
    }
    
    [Fact]
    public async Task CreateProductAsync_ValidData_CreatesSuccessfully()
    {
        // Arrange
        var productDto = new ProductDto
        {
            SellerId = 1,
            ServiceCode = "SVC001",
            ServiceName = "Blood Test",
            UnitPrice = 50.00m,
            EBMCode = "32001"
        };
        
        _mockSellerRepository.Setup(r => r.ExistsAsync(1))
            .ReturnsAsync(true);
        
        _mockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        
        // Act
        var result = await _service.CreateProductAsync(productDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Blood Test", result.ServiceName);
        _mockProductRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateProductAsync_InvalidSeller_ThrowsValidationException()
    {
        // Arrange
        var productDto = new ProductDto
        {
            SellerId = 999,
            ServiceCode = "SVC001",
            ServiceName = "Test",
            UnitPrice = 50.00m
        };
        
        _mockSellerRepository.Setup(r => r.ExistsAsync(999))
            .ReturnsAsync(false);
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProductAsync(productDto));
    }
    
    [Fact]
    public async Task CreateProductAsync_NegativePrice_ThrowsValidationException()
    {
        // Arrange
        var productDto = new ProductDto
        {
            SellerId = 1,
            ServiceCode = "SVC001",
            ServiceName = "Test",
            UnitPrice = -10.00m
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProductAsync(productDto));
    }
    
    [Fact]
    public async Task GetProductsBySellerIdAsync_ReturnsCorrectProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, SellerId = 1, ServiceName = "Service 1", ServiceCode = "S1", UnitPrice = 100 },
            new Product { Id = 2, SellerId = 1, ServiceName = "Service 2", ServiceCode = "S2", UnitPrice = 200 }
        };
        
        _mockProductRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(products);
        
        // Act
        var result = await _service.GetProductsBySellerIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateProductAsync_EmptyServiceCode_ThrowsValidationException(string serviceCode)
    {
        // Arrange
        var productDto = new ProductDto
        {
            SellerId = 1,
            ServiceCode = serviceCode!,
            ServiceName = "Test",
            UnitPrice = 50.00m
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProductAsync(productDto));
    }
}
