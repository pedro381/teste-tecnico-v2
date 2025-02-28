using Microsoft.Extensions.Logging;
using Moq;
using Thunders.TechTest.Application.Services;
using Thunders.TechTest.Domain.Models;
using Thunders.TechTest.Infrastructure.Interfaces;

namespace Thunders.TechTest.Tests.Application.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<ITollDataRepository> _repositoryMock;
        private readonly Mock<ILogger<ReportService>> _loggerMock;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _repositoryMock = new Mock<ITollDataRepository>();
            _loggerMock = new Mock<ILogger<ReportService>>();
            _reportService = new ReportService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GenerateHourlyCityReportAsync_ReturnsReports()
        {
            var expectedReports = new List<HourlyCityReport>
            {
                new() { City = "City1", Hour = DateTime.UtcNow, TotalAmount = 100m }
            };
            _repositoryMock.Setup(r => r.GetHourlyCityReportAsync()).ReturnsAsync(expectedReports);
            var result = await _reportService.GenerateHourlyCityReportAsync();
            Assert.Equal(expectedReports, result);
        }

        [Fact]
        public async Task GenerateTopTollPlazasReportAsync_ReturnsReports()
        {
            var topN = 2;
            var expectedReports = new List<TopTollPlazasReport>
            {
                new() { TollPlaza = "Plaza1", Month = DateTime.UtcNow.Month, TotalAmount = 200m },
                new() { TollPlaza = "Plaza2", Month = DateTime.UtcNow.Month, TotalAmount = 150m }
            };
            _repositoryMock.Setup(r => r.GetTopTollPlazasReportAsync(topN)).ReturnsAsync(expectedReports);
            var result = await _reportService.GenerateTopTollPlazasReportAsync(topN);
            Assert.Equal(expectedReports, result);
        }

        [Fact]
        public async Task GenerateTollUsageCountReportAsync_ReturnsReports()
        {
            var tollPlaza = "Plaza1";
            var expectedReports = new List<TollUsageCountReport>
            {
                new() { TollPlaza = tollPlaza, VehicleType = default, Count = 3 }
            };
            _repositoryMock.Setup(r => r.GetTollUsageCountReportAsync(tollPlaza)).ReturnsAsync(expectedReports);
            var result = await _reportService.GenerateTollUsageCountReportAsync(tollPlaza);
            Assert.Equal(expectedReports, result);
        }

        [Fact]
        public async Task GenerateHourlyCityReportAsync_ThrowsException()
        {
            var exception = new Exception("Test exception");
            _repositoryMock.Setup(r => r.GetHourlyCityReportAsync()).ThrowsAsync(exception);
            await Assert.ThrowsAsync<Exception>(() => _reportService.GenerateHourlyCityReportAsync());
        }

        [Fact]
        public async Task GenerateTopTollPlazasReportAsync_ThrowsException()
        {
            var exception = new Exception("Test exception");
            var topN = 2;
            _repositoryMock.Setup(r => r.GetTopTollPlazasReportAsync(topN)).ThrowsAsync(exception);
            await Assert.ThrowsAsync<Exception>(() => _reportService.GenerateTopTollPlazasReportAsync(topN));
        }

        [Fact]
        public async Task GenerateTollUsageCountReportAsync_ThrowsException()
        {
            var exception = new Exception("Test exception");
            var tollPlaza = "Plaza1";
            _repositoryMock.Setup(r => r.GetTollUsageCountReportAsync(tollPlaza)).ThrowsAsync(exception);
            await Assert.ThrowsAsync<Exception>(() => _reportService.GenerateTollUsageCountReportAsync(tollPlaza));
        }
    }
}
