using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Thunders.TechTest.Application.Messaging;

namespace Thunders.TechTest.OutOfBox.Queues
{
    public static class RebusServiceCollectionExtensions
    {
        public static IServiceCollection AddBus(
            this IServiceCollection services, 
            IConfiguration configuration, 
            SubscriptionBuilder? subscriptionBuilder = null)
        {
            services.AutoRegisterHandlersFromAssembly(typeof(TollUsageMessageHandler).Assembly);

            services.AddRebus(c => c
                .Transport(t =>
                {
                    t.UseRabbitMq(configuration.GetConnectionString("RabbitMq"), "Thunders.TechTest.TollUsage");
                }), 
                onCreated: async bus =>
                {
                    if (subscriptionBuilder != null)
                    {
                        foreach (var type in subscriptionBuilder.TypesToSubscribe)
                        {
                            await bus.Subscribe(type);
                        }
                    }

                });

            return services;
        }
    }

    public class SubscriptionBuilder
    {
        internal List<Type> TypesToSubscribe { get; private set; } = [];

        public SubscriptionBuilder Add<T>()
        {
            TypesToSubscribe.Add(typeof(T));

            return this;
        }
    }
}
