using Thunders.TechTest.Domain.Enums;

namespace Thunders.TechTest.Domain.Entities
{
    public class TollUsage
    {
        public int Id { get; set; }
        public DateTime UsageDateTime { get; set; }
        public string? TollPlaza { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public decimal AmountPaid { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}
