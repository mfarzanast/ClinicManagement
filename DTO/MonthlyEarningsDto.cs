namespace EmployeeAPI.DTO
{
    public class MonthlyEarningsDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? TotalEarnings { get; set; }
    }
}
