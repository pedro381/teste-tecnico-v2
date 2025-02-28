using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Models;
using Thunders.TechTest.Infrastructure.Data;
using Thunders.TechTest.Infrastructure.Interfaces;

namespace Thunders.TechTest.Infrastructure.Repositories
{
    public class TollDataRepository(TollDataContext context) : ITollDataRepository
    {
        private readonly TollDataContext _context = context;

        public async Task AddTollUsageAsync(TollUsage usage)
        {
            await _context.TollUsages.AddAsync(usage);
        }

        public async Task<List<TollUsage>> GetAllTollUsagesAsync()
        {
            return await _context.TollUsages.ToListAsync();
        }

        public async Task<List<HourlyCityReport>> GetHourlyCityReportAsync()
        {
            var reports = await _context.TollUsages
                .GroupBy(u => new
                {
                    u.City,
                    Hour = new DateTime(u.UsageDateTime.Year, u.UsageDateTime.Month, u.UsageDateTime.Day, u.UsageDateTime.Hour, 0, 0)
                })
                .Select(g => new HourlyCityReport
                {
                    City = g.Key.City,
                    Hour = g.Key.Hour,
                    TotalAmount = g.Sum(u => u.AmountPaid)
                }).ToListAsync();

            return reports;
        }

        public async Task<List<TopTollPlazasReport>> GetTopTollPlazasReportAsync(int topN)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var reports = await _context.TollUsages
                .Where(u => u.UsageDateTime >= firstDayOfMonth)
                .GroupBy(u => u.TollPlaza)
                .Select(g => new TopTollPlazasReport
                {
                    Month = now.Month,
                    TollPlaza = g.Key,
                    TotalAmount = g.Sum(u => u.AmountPaid)
                })
                .OrderByDescending(r => r.TotalAmount)
                .Take(topN)
                .ToListAsync();

            return reports;
        }

        public async Task<List<TollUsageCountReport>> GetTollUsageCountReportAsync(string tollPlaza)
        {
            var reports = await _context.TollUsages
                .Where(u => u.TollPlaza == tollPlaza)
                .GroupBy(u => u.VehicleType)
                .Select(g => new TollUsageCountReport
                {
                    TollPlaza = tollPlaza,
                    VehicleType = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return reports;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
