using Thunders.TechTest.Domain.Enums;

namespace Thunders.TechTest.Domain.Models
{
    public class HourlyCityReport
    {
        public DateTime Hour { get; set; }
        public string? City { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TopTollPlazasReport
    {
        public int Month { get; set; }
        public string? TollPlaza { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TollUsageCountReport
    {
        public string? TollPlaza { get; set; }
        public VehicleType VehicleType { get; set; }
        public int Count { get; set; }
    }
}
