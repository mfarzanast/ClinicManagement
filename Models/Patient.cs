using EmployeeAPI.Models;

namespace EmployeeAPi.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
           public string Phone { get; set; }
        public string  ReferenceNumber { get; set; }




        public Gender Gender { get; set; }
        public  DateTime AdmittedDate { get; set; }

        public string? Treatments { get; set; }

        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? PendingAmount { get; set; }



        public ICollection<HealthInformation> HealthInformations { get; set; }


    }
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
}
