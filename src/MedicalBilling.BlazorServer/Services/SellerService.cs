using MedicalBilling.Application.DTOs;
using System.Net.Http.Json;

namespace MedicalBilling.BlazorServer.Services;

public class SellerService
{
    private readonly HttpClient _httpClient;

    public SellerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SellerDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<SellerDto>>("/api/Sellers");
            return response ?? new List<SellerDto>();
        }
        catch
        {
            return new List<SellerDto>();
        }
    }

    public async Task<PagedResult<SellerDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        try
        {
            var url = $"/api/Sellers/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var response = await _httpClient.GetFromJsonAsync<PagedResult<SellerDto>>(url);
            return response ?? new PagedResult<SellerDto>();
        }
        catch
        {
            return new PagedResult<SellerDto>();
        }
    }

    public async Task<SellerDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SellerDto>($"/api/Sellers/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateAsync(SellerDto seller)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Sellers", seller);
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

    public async Task<bool> UpdateAsync(int id, SellerDto seller)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Sellers/{id}", seller);
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
            var response = await _httpClient.DeleteAsync($"/api/Sellers/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
