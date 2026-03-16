using MedicalBilling.Application.DTOs;

namespace MedicalBilling.Application.Interfaces;

/// <summary>
/// Service interface for Patient operations
/// </summary>
public interface IPatientService
{
    Task<PatientDto?> GetPatientByIdAsync(int id);
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
    Task<(IEnumerable<PatientDto> Items, int TotalCount)> GetPatientsPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm);
    Task<PatientDto> CreatePatientAsync(PatientDto patientDto);
    Task<PatientDto> UpdatePatientAsync(int id, PatientDto patientDto);
    Task DeletePatientAsync(int id);
    Task<bool> PatientExistsAsync(int id);
}
