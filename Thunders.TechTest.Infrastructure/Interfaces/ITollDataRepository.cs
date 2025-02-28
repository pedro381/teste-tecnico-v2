using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Models;

namespace Thunders.TechTest.Infrastructure.Interfaces
{
    public interface ITollDataRepository
    {
        Task AddTollUsageAsync(TollUsage usage);
        Task<List<TollUsage>> GetAllTollUsagesAsync();
        Task<List<HourlyCityReport>> GetHourlyCityReportAsync();
        Task<List<TopTollPlazasReport>> GetTopTollPlazasReportAsync(int topN);
        Task<List<TollUsageCountReport>> GetTollUsageCountReportAsync(string tollPlaza);
        Task SaveChangesAsync();
    }
}
