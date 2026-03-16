using Microsoft.AspNetCore.Mvc;
using MedicalBilling.Application.DTOs;
using MedicalBilling.Application.Interfaces;

namespace MedicalBilling.API.Controllers;

/// <summary>
/// REST API controller for Patient operations
/// GDPR-compliant patient data management
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsController> _logger;
    
    public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }
    
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPatientsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _patientService.GetPatientsPagedAsync(pageNumber, pageSize);
        
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
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatientById(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        
        if (patient == null)
            return NotFound($"Patient with ID {id} not found");
        
        return Ok(patient);
    }
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string query)
    {
        var patients = await _patientService.SearchPatientsAsync(query);
        return Ok(patients);
    }
    
    [HttpPost]
    // [Authorize(Roles = "Admin,Seller")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] PatientDto patientDto)
    {
        var createdPatient = await _patientService.CreatePatientAsync(patientDto);
        return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
    }
    
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin,Seller")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] PatientDto patientDto)
    {
        var updatedPatient = await _patientService.UpdatePatientAsync(id, patientDto);
        return Ok(updatedPatient);
    }
    
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePatient(int id)
    {
        await _patientService.DeletePatientAsync(id);
        return NoContent();
    }
}
