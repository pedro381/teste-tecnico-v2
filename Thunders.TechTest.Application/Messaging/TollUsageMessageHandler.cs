using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Thunders.TechTest.Application.Interfaces;
using Thunders.TechTest.Domain.Entities;

namespace Thunders.TechTest.Application.Messaging;

public class TollUsageMessageHandler(ITollDataService tollDataService, ILogger<TollUsageMessageHandler> logger) : IHandleMessages<TollUsage>
{
    public async Task Handle(TollUsage usage)
    {
        try
        {
            logger.LogInformation("Processando mensagem para a praça {TollPlaza}.", usage.TollPlaza);
            await tollDataService.ProcessTollUsageAsync(usage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar mensagem de pedágio.");
            throw;
        }
    }
}
