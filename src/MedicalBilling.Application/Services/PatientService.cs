using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Exceptions;
using MedicalBilling.Application.Interfaces;
using MedicalBilling.Application.Mapping;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Domain.Interfaces;

namespace MedicalBilling.Application.Services;

/// <summary>
/// Service implementation for Patient operations with GDPR-compliant data handling
/// </summary>
public class PatientService : IPatientService
{
    private readonly IRepository<Patient> _patientRepository;
    
    public PatientService(IRepository<Patient> patientRepository)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
    }
    
    public async Task<PatientDto?> GetPatientByIdAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Patient ID must be greater than 0");
            
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient != null ? CustomMapper.ToDto(patient) : null;
    }
    
    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return CustomMapper.ToDtoList(patients);
    }
    
    public async Task<(IEnumerable<PatientDto> Items, int TotalCount)> GetPatientsPagedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ValidationException("Page number must be greater than 0");
        if (pageSize < 1 || pageSize > 100)
            throw new ValidationException("Page size must be between 1 and 100");
            
        var (items, totalCount) = await _patientRepository.GetPagedAsync(
            pageNumber, 
            pageSize,
            orderBy: q => q.OrderByDescending(p => p.Id)
        );
        
        return (CustomMapper.ToDtoList(items), totalCount);
    }
    
    public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllPatientsAsync();
            
        var patients = await _patientRepository.FindAsync(p => 
            p.FirstName.Contains(searchTerm) || 
            p.LastName.Contains(searchTerm) ||
            p.InsuranceNumber.Contains(searchTerm)
        );
        
        return CustomMapper.ToDtoList(patients);
    }
    
    public async Task<PatientDto> CreatePatientAsync(PatientDto patientDto)
    {
        ValidatePatientDto(patientDto);
        
        // Check for duplicate insurance number
        var existing = await _patientRepository.FindAsync(p => p.InsuranceNumber == patientDto.InsuranceNumber);
        if (existing.Any())
            throw new ValidationException($"Insurance number '{patientDto.InsuranceNumber}' is already in use");
        
        var patient = CustomMapper.ToEntity(patientDto);
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;
        
        var createdPatient = await _patientRepository.AddAsync(patient);
        return CustomMapper.ToDto(createdPatient);
    }
    
    public async Task<PatientDto> UpdatePatientAsync(int id, PatientDto patientDto)
    {
        if (id <= 0)
            throw new ValidationException("Patient ID must be greater than 0");
            
        var existingPatient = await _patientRepository.GetByIdAsync(id);
        if (existingPatient == null)
            throw new NotFoundException(nameof(Patient), id);
        
        ValidatePatientDto(patientDto);
        
        // Check for duplicate insurance number (excluding current patient)
        var duplicates = await _patientRepository.FindAsync(p => 
            p.InsuranceNumber == patientDto.InsuranceNumber && p.Id != id);
        if (duplicates.Any())
            throw new ValidationException($"Insurance number '{patientDto.InsuranceNumber}' is already in use");
        
        existingPatient.FirstName = patientDto.FirstName;
        existingPatient.LastName = patientDto.LastName;
        existingPatient.DateOfBirth = patientDto.DateOfBirth;
        existingPatient.InsuranceNumber = patientDto.InsuranceNumber;
        existingPatient.InsuranceProvider = patientDto.InsuranceProvider;
        existingPatient.ContactInfo = patientDto.ContactInfo;
        existingPatient.UpdatedAt = DateTime.UtcNow;
        
        await _patientRepository.UpdateAsync(existingPatient);
        return CustomMapper.ToDto(existingPatient);
    }
    
    public async Task DeletePatientAsync(int id)
    {
        if (id <= 0)
            throw new ValidationException("Patient ID must be greater than 0");
            
        var exists = await _patientRepository.ExistsAsync(id);
        if (!exists)
            throw new NotFoundException(nameof(Patient), id);
        
        await _patientRepository.DeleteAsync(id);
    }
    
    public async Task<bool> PatientExistsAsync(int id)
    {
        return await _patientRepository.ExistsAsync(id);
    }
    
    private void ValidatePatientDto(PatientDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            errors.Add(nameof(dto.FirstName), new[] { "First name is required" });
        else if (dto.FirstName.Length > 100)
            errors.Add(nameof(dto.FirstName), new[] { "First name cannot exceed 100 characters" });
            
        if (string.IsNullOrWhiteSpace(dto.LastName))
            errors.Add(nameof(dto.LastName), new[] { "Last name is required" });
        else if (dto.LastName.Length > 100)
            errors.Add(nameof(dto.LastName), new[] { "Last name cannot exceed 100 characters" });
            
        if (dto.DateOfBirth == default)
            errors.Add(nameof(dto.DateOfBirth), new[] { "Date of birth is required" });
        else if (dto.DateOfBirth > DateTime.Today)
            errors.Add(nameof(dto.DateOfBirth), new[] { "Date of birth cannot be in the future" });
            
        if (string.IsNullOrWhiteSpace(dto.InsuranceNumber))
            errors.Add(nameof(dto.InsuranceNumber), new[] { "Insurance number is required" });
        else if (dto.InsuranceNumber.Length > 50)
            errors.Add(nameof(dto.InsuranceNumber), new[] { "Insurance number cannot exceed 50 characters" });
            
        if (string.IsNullOrWhiteSpace(dto.InsuranceProvider))
            errors.Add(nameof(dto.InsuranceProvider), new[] { "Insurance provider is required" });
        else if (dto.InsuranceProvider.Length > 200)
            errors.Add(nameof(dto.InsuranceProvider), new[] { "Insurance provider cannot exceed 200 characters" });
        
        if (errors.Any())
            throw new ValidationException(errors);
    }
}
