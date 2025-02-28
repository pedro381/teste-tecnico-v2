using Thunders.TechTest.Domain.Models;

namespace Thunders.TechTest.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<HourlyCityReport>> GenerateHourlyCityReportAsync();
        Task<List<TopTollPlazasReport>> GenerateTopTollPlazasReportAsync(int topN);
        Task<List<TollUsageCountReport>> GenerateTollUsageCountReportAsync(string tollPlaza);
    }
}
