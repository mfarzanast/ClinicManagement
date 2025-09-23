using EmployeeAPi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Repository
{
    public interface IPatientRepository
    {
        Task<Patient> UpdateAsync(Patient patient);
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetAllAsync();

        Task<Patient> AddAsync(Patient patient);
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Patient> UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.HealthInformations)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.HealthInformations)
                .Include(p => p.Payments)
                .ToListAsync();
        }
        public async Task<Patient> AddAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }


    }
}



