using EmployeeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> AddAsync(Payment payment);
        Task<IEnumerable<Payment>> GetByPatientIdAsync(Guid patientId);
        Task<Payment?> GetLatestPaymentAsync(Guid patientId);
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Payments
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PaidDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetLatestPaymentAsync(Guid patientId)
        {
            return await _context.Payments
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PaidDate)
                .FirstOrDefaultAsync();
        }
    }
}
