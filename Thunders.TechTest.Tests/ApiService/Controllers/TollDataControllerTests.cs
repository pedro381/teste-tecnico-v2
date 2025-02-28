using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Thunders.TechTest.ApiService.Controllers;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Domain.Enums;
using Thunders.TechTest.Domain.Models;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.Tests.ApiService.Controllers
{
    public class TollDataControllerTests
    {
        private readonly Mock<IReportService> _reportServiceMock;
        private readonly Mock<IMessageSender> _messageSenderMock;
        private readonly Mock<ILogger<TollDataController>> _loggerMock;
        private readonly TollDataController _controller;

        public TollDataControllerTests()
        {
            _reportServiceMock = new Mock<IReportService>();
            _messageSenderMock = new Mock<IMessageSender>();
            _loggerMock = new Mock<ILogger<TollDataController>>();
            _controller = new TollDataController(_reportServiceMock.Object, _messageSenderMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task IngestTollUsage_ReturnsBadRequest_WhenUsageIsNull()
        {
            var result = await _controller.IngestTollUsage(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos.", badRequest.Value);
        }

        [Fact]
        public async Task IngestTollUsage_ReturnsOk_WhenMessagePublishedSuccessfully()
        {
            var usage = new TollUsage { TollPlaza = "Plaza1", UsageDateTime = DateTime.UtcNow };
            _messageSenderMock.Setup(m => m.Publish(usage)).Returns(Task.CompletedTask);
            var result = await _controller.IngestTollUsage(usage);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Mensagem de pedágio enviada com sucesso.", okResult.Value);
            _messageSenderMock.Verify(m => m.Publish(usage), Times.Once);
        }

        [Fact]
        public async Task IngestTollUsage_ReturnsStatusCode500_WhenPublishThrowsException()
        {
            var usage = new TollUsage { TollPlaza = "Plaza1", UsageDateTime = DateTime.UtcNow };
            _messageSenderMock.Setup(m => m.Publish(usage)).ThrowsAsync(new Exception("Error"));
            var result = await _controller.IngestTollUsage(usage);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro interno no servidor.", statusResult.Value);
        }

        [Fact]
        public async Task GetHourlyCityReport_ReturnsOk_WithReportData()
        {
            var reportData = new List<HourlyCityReport>
            {
                new() { City = "City1", Hour = DateTime.UtcNow, TotalAmount = 100m }
            };
            _reportServiceMock.Setup(r => r.GenerateHourlyCityReportAsync()).ReturnsAsync(reportData);
            var result = await _controller.GetHourlyCityReport();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reportData, okResult.Value);
        }

        [Fact]
        public async Task GetHourlyCityReport_ReturnsStatusCode500_WhenExceptionThrown()
        {
            _reportServiceMock.Setup(r => r.GenerateHourlyCityReportAsync()).ThrowsAsync(new Exception("Error"));
            var result = await _controller.GetHourlyCityReport();
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro interno no servidor.", statusResult.Value);
        }

        [Fact]
        public async Task GetTopTollPlazasReport_ReturnsOk_WithReportData()
        {
            var topN = 5;
            var reportData = new List<TopTollPlazasReport>
            {
                new() { TollPlaza = "Plaza1", Month = DateTime.UtcNow.Month, TotalAmount = 200m }
            };
            _reportServiceMock.Setup(r => r.GenerateTopTollPlazasReportAsync(topN)).ReturnsAsync(reportData);
            var result = await _controller.GetTopTollPlazasReport(topN);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reportData, okResult.Value);
        }

        [Fact]
        public async Task GetTopTollPlazasReport_ReturnsStatusCode500_WhenExceptionThrown()
        {
            var topN = 5;
            _reportServiceMock.Setup(r => r.GenerateTopTollPlazasReportAsync(topN)).ThrowsAsync(new Exception("Error"));
            var result = await _controller.GetTopTollPlazasReport(topN);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro interno no servidor.", statusResult.Value);
        }

        [Fact]
        public async Task GetTollUsageCountReport_ReturnsBadRequest_WhenTollPlazaIsEmpty()
        {
            var result = await _controller.GetTollUsageCountReport(string.Empty);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O parâmetro 'tollPlaza' é obrigatório.", badRequest.Value);
        }

        [Fact]
        public async Task GetTollUsageCountReport_ReturnsOk_WithReportData()
        {
            var tollPlaza = "Plaza1";
            var reportData = new List<TollUsageCountReport>
            {
                new() { TollPlaza = tollPlaza, VehicleType = VehicleType.Car, Count = 3 }
            };
            _reportServiceMock.Setup(r => r.GenerateTollUsageCountReportAsync(tollPlaza)).ReturnsAsync(reportData);
            var result = await _controller.GetTollUsageCountReport(tollPlaza);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reportData, okResult.Value);
        }

        [Fact]
        public async Task GetTollUsageCountReport_ReturnsStatusCode500_WhenExceptionThrown()
        {
            var tollPlaza = "Plaza1";
            _reportServiceMock.Setup(r => r.GenerateTollUsageCountReportAsync(tollPlaza)).ThrowsAsync(new Exception("Error"));
            var result = await _controller.GetTollUsageCountReport(tollPlaza);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro interno no servidor.", statusResult.Value);
        }
    }
}
