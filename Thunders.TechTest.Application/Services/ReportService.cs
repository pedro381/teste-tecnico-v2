using Microsoft.Extensions.Logging;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Domain.Models;
using Thunders.TechTest.Infrastructure.Interfaces;

namespace Thunders.TechTest.Application.Services
{
    public class ReportService(ITollDataRepository repository, ILogger<ReportService> logger) : IReportService
    {
        private readonly ITollDataRepository _repository = repository;
        private readonly ILogger<ReportService> _logger = logger;

        public async Task<List<HourlyCityReport>> GenerateHourlyCityReportAsync()
        {
            try
            {
                _logger.LogInformation("Gerando relatório de valor total por hora e cidade.");
                return await _repository.GetHourlyCityReportAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de valor total por hora e cidade.");
                throw;
            }
        }

        public async Task<List<TopTollPlazasReport>> GenerateTopTollPlazasReportAsync(int topN)
        {
            try
            {
                _logger.LogInformation("Gerando relatório das top {TopN} praças mais faturadas.", topN);
                return await _repository.GetTopTollPlazasReportAsync(topN);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório das praças mais faturadas.");
                throw;
            }
        }

        public async Task<List<TollUsageCountReport>> GenerateTollUsageCountReportAsync(string tollPlaza)
        {
            try
            {
                _logger.LogInformation("Gerando relatório de contagem de veículos para a praça {TollPlaza}.", tollPlaza);
                return await _repository.GetTollUsageCountReportAsync(tollPlaza);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de contagem de veículos.");
                throw;
            }
        }
    }
}
