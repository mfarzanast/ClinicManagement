using EmployeeAPi.Models;

namespace EmployeeAPI.Models
{
    public class HealthInformation
    {

        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
