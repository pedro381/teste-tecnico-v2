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
    // Adiciona servi�os comuns: descoberta de servi�os, resili�ncia, health checks e OpenTelemetry.
    // Esse projeto deve ser referenciado por cada servi�o na solu��o.
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
                // Ativa resili�ncia por padr�o
                http.AddStandardResilienceHandler(options =>
                {
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(20); // Deve ser pelo menos o dobro do AttemptTimeout
                });

                // Ativa a descoberta de servi�os por padr�o
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
                        //.AddGrpcClientInstrumentation() // Descomente para habilitar instrumenta��o gRPC (necessita do pacote OpenTelemetry.Instrumentation.GrpcNetClient)
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
                // Adiciona uma verifica��o de liveness para garantir que a aplica��o esteja responsiva
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Aten��o: disponibilizar endpoints de health check em ambientes n�o-desenvolvimento pode ter implica��es de seguran�a.
            // Veja https://aka.ms/dotnet/aspire/healthchecks para detalhes.
            if (app.Environment.IsDevelopment())
            {
                // Todos os health checks devem passar para que a aplica��o esteja pronta para receber tr�fego
                app.MapHealthChecks("/health");

                // Apenas os health checks com a tag "live" devem passar para considerar a aplica��o viva
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }
    }
}
