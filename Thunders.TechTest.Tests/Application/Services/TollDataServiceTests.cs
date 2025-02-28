using Microsoft.Extensions.Logging;
using Moq;
using Thunders.TechTest.Application.Services;
using Thunders.TechTest.Domain.Entities;
using Thunders.TechTest.Infrastructure.Interfaces;

namespace Thunders.TechTest.Tests.Application.Services
{
    public class TollDataServiceTests
    {
        private readonly Mock<ITollDataRepository> _repositoryMock;
        private readonly Mock<ILogger<TollDataService>> _loggerMock;
        private readonly TollDataService _service;

        public TollDataServiceTests()
        {
            _repositoryMock = new Mock<ITollDataRepository>();
            _loggerMock = new Mock<ILogger<TollDataService>>();
            _service = new TollDataService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessTollUsageAsync_CallsRepositoryMethods()
        {
            var usage = new TollUsage
            {
                TollPlaza = "Plaza1",
                UsageDateTime = DateTime.UtcNow
            };

            await _service.ProcessTollUsageAsync(usage);

            _repositoryMock.Verify(r => r.AddTollUsageAsync(usage), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessTollUsageAsync_ThrowsException_WhenAddTollUsageFails()
        {
            var usage = new TollUsage
            {
                TollPlaza = "Plaza1",
                UsageDateTime = DateTime.UtcNow
            };

            var exception = new Exception("Test exception");
            _repositoryMock.Setup(r => r.AddTollUsageAsync(usage)).ThrowsAsync(exception);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.ProcessTollUsageAsync(usage));
            Assert.Equal(exception, ex);

            _repositoryMock.Verify(r => r.AddTollUsageAsync(usage), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessTollUsageAsync_ThrowsException_WhenSaveChangesFails()
        {
            var usage = new TollUsage
            {
                TollPlaza = "Plaza1",
                UsageDateTime = DateTime.UtcNow
            };

            var exception = new Exception("Test exception");
            _repositoryMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(exception);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.ProcessTollUsageAsync(usage));
            Assert.Equal(exception, ex);

            _repositoryMock.Verify(r => r.AddTollUsageAsync(usage), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
