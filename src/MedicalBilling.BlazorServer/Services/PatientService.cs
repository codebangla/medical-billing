using MedicalBilling.Application.DTOs;
using System.Net.Http.Json;

namespace MedicalBilling.BlazorServer.Services;

public class PatientService
{
    private readonly HttpClient _httpClient;

    public PatientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PatientDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PatientDto>>("/api/Patients");
            return response ?? new List<PatientDto>();
        }
        catch
        {
            return new List<PatientDto>();
        }
    }

    public async Task<PagedResult<PatientDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        try
        {
            var url = $"/api/Patients/paged?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var response = await _httpClient.GetFromJsonAsync<PagedResult<PatientDto>>(url);
            return response ?? new PagedResult<PatientDto>();
        }
        catch
        {
            return new PagedResult<PatientDto>();
        }
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PatientDto>($"/api/Patients/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateAsync(PatientDto patient)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Patients", patient);
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

    public async Task<bool> UpdateAsync(int id, PatientDto patient)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Patients/{id}", patient);
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
            var response = await _httpClient.DeleteAsync($"/api/Patients/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
