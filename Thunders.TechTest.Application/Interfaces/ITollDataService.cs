using Thunders.TechTest.Domain.Entities;

namespace Thunders.TechTest.Application.Interfaces
{
    public interface ITollDataService
    {
        Task ProcessTollUsageAsync(TollUsage usage);
    }
}
