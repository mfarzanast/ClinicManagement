using EmployeeAPi.Models;

namespace EmployeeAPI.Models
{
    public class Payments
    {

        public class Payment
        {
            public Guid Id { get; set; }
            public Guid PatientId { get; set; }
            public decimal? TotalAmount { get; set; }
            public decimal? ReceivedAmount { get; set; } = 0; // ✅ new column
            public decimal? PendingAmount { get; set; }
            public DateTime PaidDate { get; set; } = DateTime.UtcNow;

            

            public Patient Patient { get; set; }
        }
    }
}
