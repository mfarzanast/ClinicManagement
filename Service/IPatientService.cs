using EmployeeAPi.Models;
using EmployeeAPI.DTO;
using EmployeeAPI.Models;
using EmployeeAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Service
{
    public interface IPatientService
    {
        Task<Patient> CreatePatientAsync(PatientCreateDto dto);
        Task<Patient?> UpdatePatientAsync(Guid id, PatientCreateDto dto);
        Task<Patient?> DischargePatientAsync(Guid patientId);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(Guid id);
        Task<Patient?> PayPendingAmountAsync(Guid patientId, decimal amount);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PatientService(IPatientRepository patientRepository, IPaymentRepository paymentRepository)
        {
            _patientRepository = patientRepository;
            _paymentRepository = paymentRepository;
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
                Age = dto.Age,
                Status = true,
                HealthInformations = dto.HealthInformations?.Select(h => new HealthInformation
                {
                    Id = Guid.NewGuid(),
                    Description = h.Description,
                }).ToList() ?? new List<HealthInformation>()
            };

            await _patientRepository.AddAsync(patient);

            var firstPayment = new Payment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                TotalAmount = patient.TotalAmount,
                ReceivedAmount = dto.PaidAmount,
                PendingAmount = dto.TotalAmount - dto.PaidAmount
            };

            await _paymentRepository.AddAsync(firstPayment);

            return patient;
        }

        public async Task<Patient?> UpdatePatientAsync(Guid id, PatientCreateDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            patient.Phone = dto.Phone;
            patient.Gender = dto.Gender;
            patient.Name = dto.Name;
            patient.TotalAmount = dto.TotalAmount;
            patient.ReferenceNumber = dto.ReferenceNumber;
            patient.Treatments = dto.Treatments;
            patient.TreatmentDescription = dto.TreatmentDescription;
            patient.Age = dto.Age;

            patient.HealthInformations.Clear();
            if (dto.HealthInformations != null)
            {
                foreach (var hi in dto.HealthInformations)
                {
                    patient.HealthInformations.Add(new HealthInformation
                    {
                        Id = Guid.NewGuid(),
                        Description = hi.Description,
                        PatientId = patient.Id
                    });
                }
            }

            await _patientRepository.UpdateAsync(patient);
            return patient;
        }

        public async Task<Patient?> DischargePatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null) return null;

            var lastPayment = patient.Payments.OrderByDescending(p => p.PaidDate).FirstOrDefault();
            if (lastPayment != null && lastPayment.PendingAmount > 0)
                throw new InvalidOperationException("Patient cannot be discharged with pending dues.");

            patient.Status = false;
            await _patientRepository.UpdateAsync(patient);

            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public async Task<Patient?> PayPendingAmountAsync(Guid patientId, decimal amount)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null) return null;

            var lastPayment = await _paymentRepository.GetLatestPaymentAsync(patientId);
            decimal? pending = lastPayment?.PendingAmount ?? patient.TotalAmount;
            decimal? received = lastPayment?.ReceivedAmount ?? 0;

            if (pending <= 0)
                throw new InvalidOperationException("No pending amount to pay.");
            if (amount > pending)
                throw new InvalidOperationException("Payment exceeds pending amount.");

            var newPayment = new Payment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                TotalAmount = patient.TotalAmount,
                ReceivedAmount = amount,
                PendingAmount = pending - amount
            };

            await _paymentRepository.AddAsync(newPayment);
            return patient;
        }
    }
}
