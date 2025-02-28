using Microsoft.Extensions.Logging;
using Moq;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Application.Messaging;
using Thunders.TechTest.Domain.Entities;

namespace Thunders.TechTest.Tests.Application.Messaging
{
    public class TollUsageMessageHandlerTests
    {
        private readonly Mock<ITollDataService> _tollDataServiceMock;
        private readonly Mock<ILogger<TollUsageMessageHandler>> _loggerMock;
        private readonly TollUsageMessageHandler _handler;

        public TollUsageMessageHandlerTests()
        {
            _tollDataServiceMock = new Mock<ITollDataService>();
            _loggerMock = new Mock<ILogger<TollUsageMessageHandler>>();
            _handler = new TollUsageMessageHandler(_tollDataServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_CallsProcessTollUsageAsync()
        {
            var usage = new TollUsage
            {
                TollPlaza = "Plaza1",
                UsageDateTime = DateTime.UtcNow
            };

            await _handler.Handle(usage);

            _tollDataServiceMock.Verify(x => x.ProcessTollUsageAsync(usage), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenProcessTollUsageAsyncFails()
        {
            var usage = new TollUsage
            {
                TollPlaza = "Plaza1",
                UsageDateTime = DateTime.UtcNow
            };

            var exception = new Exception("Test error");
            _tollDataServiceMock.Setup(x => x.ProcessTollUsageAsync(usage)).ThrowsAsync(exception);

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(usage));
            Assert.Equal(exception, ex);
        }
    }
}
