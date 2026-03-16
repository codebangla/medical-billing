using Microsoft.AspNetCore.Mvc;
using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Interfaces;

namespace MedicalBilling.API.Controllers;

/// <summary>
/// REST API controller for Seller operations
/// Requires authentication, Admin role for create/delete
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize] - Disabled by user request
public class SellersController : ControllerBase
{
    private readonly ISellerService _sellerService;
    private readonly ILogger<SellersController> _logger;
    
    public SellersController(ISellerService sellerService, ILogger<SellersController> logger)
    {
        _sellerService = sellerService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all sellers (Admin only)
    /// </summary>
    [HttpGet]
    // [Authorize(Roles = "Admin")] - Disabled
    [ProducesResponseType(typeof(IEnumerable<SellerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SellerDto>>> GetAllSellers()
    {
        var sellers = await _sellerService.GetAllSellersAsync();
        return Ok(sellers);
    }
    
    /// <summary>
    /// Get sellers with paging (Admin only)
    /// </summary>
    [HttpGet("paged")]
    // [Authorize(Roles = "Admin")] - Disabled
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSellersPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _sellerService.GetSellersPagedAsync(pageNumber, pageSize);
        
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
    /// Get seller by ID (Admin or Seller viewing own data)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SellerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SellerDto>> GetSellerById(int id)
    {
        var seller = await _sellerService.GetSellerByIdAsync(id);
        
        if (seller == null)
            return NotFound($"Seller with ID {id} not found");
        
        return Ok(seller);
    }
    
    /// <summary>
    /// Search sellers (Admin only)
    /// </summary>
    [HttpGet("search")]
    // [Authorize(Roles = "Admin")] - Disabled
    [ProducesResponseType(typeof(IEnumerable<SellerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SellerDto>>> SearchSellers([FromQuery] string query)
    {
        var sellers = await _sellerService.SearchSellersAsync(query);
        return Ok(sellers);
    }
    
    /// <summary>
    /// Create new seller (Admin only)
    /// </summary>
    [HttpPost]
    // [Authorize(Roles = "Admin")] - Disabled
    [ProducesResponseType(typeof(SellerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SellerDto>> CreateSeller([FromBody] SellerDto sellerDto)
    {
        try 
        {
            Console.WriteLine($"[API DEBUG] Creating Seller: {System.Text.Json.JsonSerializer.Serialize(sellerDto)}");
            var createdSeller = await _sellerService.CreateSellerAsync(sellerDto);
            return CreatedAtAction(nameof(GetSellerById), new { id = createdSeller.Id }, createdSeller);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERROR] CreateSeller Failed: {ex}");
            // Return string representation of exception for debugging
            return BadRequest($"DEBUG ERROR: {ex.Message} \n STACK: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Update seller (Admin or Seller updating own data)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SellerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SellerDto>> UpdateSeller(int id, [FromBody] SellerDto sellerDto)
    {
        var updatedSeller = await _sellerService.UpdateSellerAsync(id, sellerDto);
        return Ok(updatedSeller);
    }
    
    /// <summary>
    /// Delete seller (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")] - Disabled
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSeller(int id)
    {
        await _sellerService.DeleteSellerAsync(id);
        return NoContent();
    }
}
