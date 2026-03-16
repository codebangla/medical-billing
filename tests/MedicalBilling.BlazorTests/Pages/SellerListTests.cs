using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MedicalBilling.BlazorServer.Components.Pages;
using MedicalBilling.BlazorServer.Services;
using Moq;
using System.Net.Http;

namespace MedicalBilling.BlazorTests.Pages;

/// <summary>
/// bUnit tests for SellerList Blazor component
/// Tests rendering, user interactions, and state management
/// </summary>
public class SellerListTests : TestContext
{
    private readonly Mock<HttpClient> _mockHttpClient;
    
    public SellerListTests()
    {
        _mockHttpClient = new Mock<HttpClient>();
        
        // Setup test context
        Services.AddScoped(_ => _mockHttpClient.Object);
        
        Services.AddScoped<IConfiguration>(sp => 
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ApiBaseUrl"] = "https://localhost:5001"
                })
                .Build();
            return config;
        });
        
        // Register SellerService
        Services.AddScoped<SellerService>();
    }
    
    [Fact]
    public void SellerList_RendersCorrectly()
    {
        // Arrange & Act
        var cut = RenderComponent<Sellers>();
        
        // Assert
        Assert.NotNull(cut);
        // Check for the header with icon
        var header = cut.Find("h2");
        Assert.Contains("Seller Management", header.TextContent);
    }
    
    [Fact]
    public void SellerList_ShowsLoadingOrContent()
    {
        // Arrange & Act
        var cut = RenderComponent<Sellers>();
        
        // Assert - Component should show either loading spinner or content
        // In test environment, it loads very quickly so we check for either state
        var hasSpinner = cut.FindAll(".spinner-border").Count > 0;
        var hasTable = cut.FindAll("table").Count > 0;
        var hasAlert = cut.FindAll(".alert").Count > 0;
        
        Assert.True(hasSpinner || hasTable || hasAlert, 
            "Component should show either loading spinner, table, or alert message");
    }
    
    [Fact]
    public void SellerList_HasAddButton()
    {
        // Arrange & Act
        var cut = RenderComponent<Sellers>();
        
        // Assert
        var addButton = cut.Find("button.btn-primary");
        Assert.Contains("Add New Seller", addButton.TextContent);
    }
    
    [Fact]
    public void SellerList_HasSearchInput()
    {
        // Arrange & Act
        var cut = RenderComponent<Sellers>();
        
        // Assert
        var searchInput = cut.Find("input[placeholder='Search sellers...']");
        Assert.NotNull(searchInput);
    }
    
    [Fact]
    public void SellerList_OpensModal_WhenAddButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<Sellers>();
        var addButton = cut.Find("button.btn-primary");
        
        // Act
        addButton.Click();
        
        // Assert
        var modal = cut.Find(".modal");
        Assert.NotNull(modal);
        Assert.Contains("Create New Seller", modal.TextContent);
    }
    
    [Fact]
    public void SellerList_ClosesModal_WhenCancelClicked()
    {
        // Arrange
        var cut = RenderComponent<Sellers>();
        var addButton = cut.Find("button.btn-primary");
        addButton.Click();
        
        // Act
        var cancelButton = cut.Find("button.btn-secondary");
        cancelButton.Click();
        
        // Assert
        Assert.Throws<ElementNotFoundException>(() => cut.Find(".modal"));
    }
}
