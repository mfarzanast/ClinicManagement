using EmployeeAPI.DTO;
using EmployeeAPI.Repository;
using EmployeeAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IPatientService _service;
        private readonly IPaymentRepository _paymentRepository;

        public PatientController(IPatientService service, IPaymentRepository paymentRepository, AppDbContext context)
        {

            _context = context;
            _service = service;
            _paymentRepository = paymentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientCreateDto dto)
        {
            var patient = await _service.CreatePatientAsync(dto);
            return Ok(patient);
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

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(Guid id, [FromBody] PaymentUpdateDto dto)
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

        [HttpPut("{id}/discharge")]
        public async Task<IActionResult> Discharge(Guid id)
        {
            var patient = await _service.DischargePatientAsync(id);
            if (patient == null) return NotFound();

            return Ok(patient);
        }

        [HttpGet("{id}/payments")]
        public async Task<IActionResult> GetPayments(Guid id)
        {
            var payments = await _paymentRepository.GetByPatientIdAsync(id);
            return Ok(payments);
        }

        [HttpGet("monthly-earnings")]
        public async Task<IActionResult> GetMonthlyEarnings()
        {
            var result = await GetMonthlyEarningsAsync();
            return Ok(result);
        }

        private async Task<List<MonthlyEarningsDto>> GetMonthlyEarningsAsync()
        {
            var monthlyEarnings = await _context.Payments
                .GroupBy(p => new { p.PaidDate.Year, p.PaidDate.Month })
                .Select(g => new MonthlyEarningsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalEarnings = g.Sum(p => p.ReceivedAmount ?? 0) 
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            return monthlyEarnings;
        }
    }
}
