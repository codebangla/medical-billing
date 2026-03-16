using Xunit;
using Moq;
using MedicalBilling.Application.Services;
using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Exceptions;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Domain.Interfaces;

namespace MedicalBilling.UnitTests.Services;

/// <summary>
/// Unit tests for SellerService
/// Tests CRUD operations, validation, edge cases, and error handling
/// </summary>
public class SellerServiceTests
{
    private readonly Mock<IRepository<Seller>> _mockRepository;
    private readonly SellerService _service;
    
    public SellerServiceTests()
    {
        _mockRepository = new Mock<IRepository<Seller>>();
        _service = new SellerService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task GetSellerByIdAsync_ValidId_ReturnsSeller()
    {
        // Arrange
        var seller = new Seller
        {
            Id = 1,
            Name = "Dr. Smith Clinic",
            Email = "smith@clinic.com",
            LicenseNumber = "LIC123456",
            Specialty = "Cardiology"
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(seller);
        
        // Act
        var result = await _service.GetSellerByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Dr. Smith Clinic", result.Name);
    }
    
    [Fact]
    public async Task GetSellerByIdAsync_InvalidId_ThrowsValidationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.GetSellerByIdAsync(0));
        await Assert.ThrowsAsync<ValidationException>(() => _service.GetSellerByIdAsync(-1));
    }
    
    [Fact]
    public async Task GetSellerByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Seller?)null);
        
        // Act
        var result = await _service.GetSellerByIdAsync(999);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task CreateSellerAsync_ValidData_CreatesSuccessfully()
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = "New Clinic",
            Email = "new@clinic.com",
            LicenseNumber = "LIC789",
            Specialty = "Pediatrics"
        };
        
        _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seller, bool>>>()))
            .ReturnsAsync(new List<Seller>());
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Seller>()))
            .ReturnsAsync((Seller s) => s);
        
        // Act
        var result = await _service.CreateSellerAsync(sellerDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Clinic", result.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Seller>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateSellerAsync_EmptyName_ThrowsValidationException()
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = "",
            Email = "test@test.com",
            LicenseNumber = "LIC123"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateSellerAsync(sellerDto));
    }
    
    [Fact]
    public async Task CreateSellerAsync_InvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = "Test Clinic",
            Email = "invalid-email",
            LicenseNumber = "LIC123"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateSellerAsync(sellerDto));
    }
    
    [Fact]
    public async Task CreateSellerAsync_DuplicateEmail_ThrowsValidationException()
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = "Test Clinic",
            Email = "existing@clinic.com",
            LicenseNumber = "LIC123"
        };
        
        var existingSeller = new Seller
        {
            Id = 1,
            Email = "existing@clinic.com",
            Name = "Existing",
            LicenseNumber = "LIC999"
        };
        
        _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seller, bool>>>()))
            .ReturnsAsync(new List<Seller> { existingSeller });
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateSellerAsync(sellerDto));
    }
    
    [Fact]
    public async Task UpdateSellerAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var existingSeller = new Seller
        {
            Id = 1,
            Name = "Old Name",
            Email = "old@email.com",
            LicenseNumber = "LIC123",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
        
        var updateDto = new SellerDto
        {
            Id = 1,
            Name = "Updated Name",
            Email = "updated@email.com",
            LicenseNumber = "LIC123",
            Specialty = "Updated Specialty"
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingSeller);
        
        _mockRepository.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seller, bool>>>()))
            .ReturnsAsync(new List<Seller>());
        
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Seller>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _service.UpdateSellerAsync(1, updateDto);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Seller>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteSellerAsync_ExistingSeller_DeletesSuccessfully()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1))
            .ReturnsAsync(true);
        
        _mockRepository.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        
        // Act
        await _service.DeleteSellerAsync(1);
        
        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }
    
    [Fact]
    public async Task DeleteSellerAsync_NonExistentSeller_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999))
            .ReturnsAsync(false);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteSellerAsync(999));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task CreateSellerAsync_NullOrEmptyName_ThrowsValidationException(string name)
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = name!,
            Email = "test@test.com",
            LicenseNumber = "LIC123"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateSellerAsync(sellerDto));
    }
    
    [Fact]
    public async Task CreateSellerAsync_NameTooLong_ThrowsValidationException()
    {
        // Arrange
        var sellerDto = new SellerDto
        {
            Name = new string('A', 201), // Max is 200
            Email = "test@test.com",
            LicenseNumber = "LIC123"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateSellerAsync(sellerDto));
    }
}
