using MedicalBilling.Application.DTOs;
using System.Net.Http.Json;

namespace MedicalBilling.BlazorServer.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ProductDto>>("/api/Products");
            return response ?? new List<ProductDto>();
        }
        catch
        {
            return new List<ProductDto>();
        }
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        try
        {
            var url = $"/api/Products/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var response = await _httpClient.GetFromJsonAsync<PagedResult<ProductDto>>(url);
            return response ?? new PagedResult<ProductDto>();
        }
        catch
        {
            return new PagedResult<ProductDto>();
        }
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"/api/Products/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateAsync(ProductDto product)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Products", product);
            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }
            
            var error = await response.Content.ReadAsStringAsync();
            return (false, $"API Error ({response.StatusCode}): {error}");
        }
        catch (Exception ex)
        {
            return (false, $"Exception: {ex.Message}");
        }
    }

    public async Task<bool> UpdateAsync(int id, ProductDto product)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Products/{id}", product);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Products/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
