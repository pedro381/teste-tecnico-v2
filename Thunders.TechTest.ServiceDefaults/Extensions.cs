using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Thunders.TechTest.ServiceDefaults
{
    // Adiciona serviços comuns: descoberta de serviços, resiliência, health checks e OpenTelemetry.
    // Esse projeto deve ser referenciado por cada serviço na solução.
    // Para mais detalhes, veja https://aka.ms/dotnet/aspire/service-defaults
    public static class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Ativa resiliência por padrão
                http.AddStandardResilienceHandler(options =>
                {
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(20); // Deve ser pelo menos o dobro do AttemptTimeout
                });

                // Ativa a descoberta de serviços por padrão
                http.AddServiceDiscovery();
            });

            return builder;
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                        //.AddGrpcClientInstrumentation() // Descomente para habilitar instrumentação gRPC (necessita do pacote OpenTelemetry.Instrumentation.GrpcNetClient)
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Para habilitar o exportador do Azure Monitor (necessita do pacote Azure.Monitor.OpenTelemetry.AspNetCore):
            // if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            // {
            //     builder.Services.AddOpenTelemetry().UseAzureMonitor();
            // }

            return builder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                // Adiciona uma verificação de liveness para garantir que a aplicação esteja responsiva
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Atenção: disponibilizar endpoints de health check em ambientes não-desenvolvimento pode ter implicações de segurança.
            // Veja https://aka.ms/dotnet/aspire/healthchecks para detalhes.
            if (app.Environment.IsDevelopment())
            {
                // Todos os health checks devem passar para que a aplicação esteja pronta para receber tráfego
                app.MapHealthChecks("/health");

                // Apenas os health checks com a tag "live" devem passar para considerar a aplicação viva
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }
    }
}
