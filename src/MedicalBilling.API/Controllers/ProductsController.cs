using Microsoft.AspNetCore.Mvc;
using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Interfaces;

namespace MedicalBilling.API.Controllers;

/// <summary>
/// REST API controller for Product operations
/// Handles seller-product relationships
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all products (All authenticated users)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }
    
    /// <summary>
    /// Get products with paging
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _productService.GetProductsPagedAsync(pageNumber, pageSize);
        
        var response = new
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
        
        return Ok(response);
    }
    
    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
            return NotFound($"Product with ID {id} not found");
        
        return Ok(product);
    }
    
    /// <summary>
    /// Get products by seller ID
    /// </summary>
    [HttpGet("seller/{sellerId}")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySeller(int sellerId)
    {
        var products = await _productService.GetProductsBySellerIdAsync(sellerId);
        return Ok(products);
    }
    
    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string query)
    {
        var products = await _productService.SearchProductsAsync(query);
        return Ok(products);
    }
    
    /// <summary>
    /// Create new product (Admin or Seller creating own products)
    /// </summary>
    [HttpPost]
    // [Authorize(Roles = "Admin,Seller")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productDto)
    {
        var createdProduct = await _productService.CreateProductAsync(productDto);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }
    
    /// <summary>
    /// Update product (Admin or Seller updating own products)
    /// </summary>
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin,Seller")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
        return Ok(updatedProduct);
    }
    
    /// <summary>
    /// Delete product (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
