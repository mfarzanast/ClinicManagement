using EmployeeAPi.Models;

namespace EmployeeAPI.DTO
{
    public class PatientCreateDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ReferenceNumber { get; set; }

        public Gender Gender { get; set; }
        public DateTime AdmittedDate { get; set; }

        public string? Treatments { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? PendingAmount { get; set; }



        public List<HealthInformationDto> HealthInformations { get; set; }
    }

    public class HealthInformationDto
    {
        public string Description { get; set; }
    }
}
