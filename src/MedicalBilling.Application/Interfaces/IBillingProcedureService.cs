using MedicalBilling.Application.DTOs;

namespace MedicalBilling.Application.Interfaces;

/// <summary>
/// Service interface for BillingProcedure operations
/// </summary>
public interface IBillingProcedureService
{
    Task<BillingProcedureDto?> GetBillingProcedureByIdAsync(int id);
    Task<IEnumerable<BillingProcedureDto>> GetAllBillingProceduresAsync();
    Task<(IEnumerable<BillingProcedureDto> Items, int TotalCount)> GetBillingProceduresPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<BillingProcedureDto>> GetBillingProceduresByPatientIdAsync(int patientId);
    Task<BillingProcedureDto> CreateBillingProcedureAsync(BillingProcedureDto billingProcedureDto);
    Task<BillingProcedureDto> UpdateBillingProcedureAsync(int id, BillingProcedureDto billingProcedureDto);
    Task DeleteBillingProcedureAsync(int id);
}
