using Microsoft.Extensions.Logging;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Infrastructure.Interfaces;

namespace Thunders.TechTest.Application.Services
{
    public class TollDataService(ITollDataRepository repository, ILogger<TollDataService> logger) : ITollDataService
    {
        private readonly ITollDataRepository _repository = repository;
        private readonly ILogger<TollDataService> _logger = logger;

        public async Task ProcessTollUsageAsync(TollUsage usage)
        {
            try
            {
                _logger.LogInformation("Processando registro de pedágio: {TollPlaza} em {UsageDateTime}", usage.TollPlaza, usage.UsageDateTime);
                
                await _repository.AddTollUsageAsync(usage);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar registro de pedágio");
                throw;
            }
        }
    }
}
