using EmployeeAPi.Models;
using EmployeeAPI.DTO;
using EmployeeAPI.Models;
using EmployeeAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Service
{
    public interface IPatientService
    {
        Task<Patient?> PayPendingAmountAsync(Guid patientId, decimal amount);
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> UpdatePatientAsync(Guid id, PatientCreateDto dto);
        Task<Patient?> DischargePatientAsync(Guid id);

        Task<Patient> CreatePatientAsync(PatientCreateDto dto);
    }

    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository,
            AppDbContext appDbContext)
        {
            _context = appDbContext;
            _repository = repository;
        }
        public async Task<Patient?> PayPendingAmountAsync(Guid patientId, decimal amount)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) return null;

            if (patient.PendingAmount == null || patient.PendingAmount <= 0)
                throw new InvalidOperationException("No pending amount to pay.");

            if (amount > patient.PendingAmount)
                throw new InvalidOperationException("Payment exceeds pending amount.");

            patient.PaidAmount = (patient.PaidAmount ?? 0) + amount;
            patient.PendingAmount = (patient.PendingAmount ?? 0) - amount;

            return await _repository.UpdateAsync(patient);
        }
       
        public async Task<Patient> CreatePatientAsync(PatientCreateDto dto)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Phone = dto.Phone,
                ReferenceNumber = dto.ReferenceNumber,
                Gender = dto.Gender,
                AdmittedDate = dto.AdmittedDate.Date,
                Treatments = dto.Treatments,
                TreatmentDescription = dto.TreatmentDescription,
                TotalAmount = dto.TotalAmount,
                Age=dto.Age,
                PaidAmount = dto.PaidAmount,
                Status = true,
                PendingAmount = dto.TotalAmount - dto.PaidAmount,
                HealthInformations = dto.HealthInformations.Select(h => new HealthInformation
                {
                    Id = Guid.NewGuid(),
                    Description = h.Description,
                    PatientId = Guid.Empty
                }).ToList(),
            };

            return await _repository.AddAsync(patient);
        }
        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Patient?> UpdatePatientAsync(Guid id, PatientCreateDto dto)
        {
            var existing = await _context.Patients
                .Include(p => p.HealthInformations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null)
                throw new KeyNotFoundException("Patient not found");

            existing.Phone = dto.Phone;
            existing.Gender = dto.Gender;
            existing.Name = dto.Name;
            existing.PaidAmount = dto.PaidAmount;
            existing.TotalAmount = dto.TotalAmount;
            existing.PendingAmount = dto.TotalAmount - dto.PaidAmount;
            existing.ReferenceNumber = dto.ReferenceNumber;
            existing.Treatments = dto.Treatments;
            existing.TreatmentDescription = dto.TreatmentDescription;
            existing.Age = dto.Age;

            if (existing.HealthInformations.Any())
            {
                _context.HealthInformations.RemoveRange(existing.HealthInformations);
            }

            if (dto.HealthInformations != null && dto.HealthInformations.Any())
            {
                var newInfos = dto.HealthInformations.Select(hi => new HealthInformation
                {
                    Id = Guid.NewGuid(),
                    Description = hi.Description,
                    PatientId = existing.Id
                }).ToList();

                await _context.HealthInformations.AddRangeAsync(newInfos);
            }

            await _context.SaveChangesAsync();

            return existing;
        }
        public async Task<Patient?> DischargePatientAsync(Guid patientId)
        {
            var patient = await _repository.GetByIdAsync(patientId);
            if (patient == null) return null;

            if (patient.PendingAmount > 0)
                throw new InvalidOperationException("Patient cannot be discharged with pending dues.");

            patient.Status = false;

            return await _repository.UpdateAsync(patient);
        }



    }
}