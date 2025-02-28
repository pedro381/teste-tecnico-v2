using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollDataController(
        IReportService reportService,
        IMessageSender messageSender, 
        ILogger<TollDataController> logger) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;
        private readonly IMessageSender _messageSender = messageSender;
        private readonly ILogger<TollDataController> _logger = logger;

        [HttpPost("ingest")]
        public async Task<IActionResult> IngestTollUsage([FromBody] TollUsage? usage)
        {
            if (usage == null)
            {
                _logger.LogWarning("Dados de pedágio nulos recebidos.");
                return BadRequest("Dados inválidos.");
            }

            try
            {
                await _messageSender.Publish(usage);
                return Ok("Mensagem de pedágio enviada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de pedágio.");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpGet("reports/hourly-city")]
        public async Task<IActionResult> GetHourlyCityReport()
        {
            try
            {
                var report = await _reportService.GenerateHourlyCityReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relatório de valor por hora e cidade.");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpGet("reports/top-toll-plazas")]
        public async Task<IActionResult> GetTopTollPlazasReport([FromQuery] int topN = 5)
        {
            try
            {
                var report = await _reportService.GenerateTopTollPlazasReportAsync(topN);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relatório das praças mais faturadas.");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }

        [HttpGet("reports/toll-usage-count")]
        public async Task<IActionResult> GetTollUsageCountReport([FromQuery] string tollPlaza)
        {
            if (string.IsNullOrWhiteSpace(tollPlaza))
            {
                _logger.LogWarning("Parâmetro 'tollPlaza' não informado.");
                return BadRequest("O parâmetro 'tollPlaza' é obrigatório.");
            }

            try
            {
                var report = await _reportService.GenerateTollUsageCountReportAsync(tollPlaza);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relatório de contagem de veículos.");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}
