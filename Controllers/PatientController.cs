using EmployeeAPI.DTO;
using EmployeeAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientCreateDto dto)
        {
            var patient = await _service.CreatePatientAsync(dto);
            return Ok(patient);
        }
        [HttpGet("monthly-earnings")]
        public async Task<IActionResult> GetMonthlyEarnings()
        {
            var result = await _service.GetMonthlyEarningsAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _service.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PatientCreateDto dto)
        {
            var patient = await _service.UpdatePatientAsync(id, dto);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPut("{id}/pay")]
        public async Task<IActionResult> PayPending(Guid id, [FromBody] PaymentUpdateDto dto)
        {
            try
            {
                var patient = await _service.PayPendingAmountAsync(id, dto.AmountPaid);
                if (patient == null) return NotFound();
                return Ok(patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}