using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GetTemps;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configure open telemetry
        // https://www.youtube.com/watch?v=-jrUiq21gS4
        // (skip ahead to 33:20)
        // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel#5-configure-opentelemetry-with-the-correct-providers
        // https://opentelemetry.io/docs/languages/net/exporters/
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });
        // TODO: If this is not a suitable default change to add OtlpExporters only if the config is present
        var otlpEndpoint = new Uri(builder.Configuration["OTLP_ENDPOINT_URL"] ?? "https://otel:4317");
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(c => c.AddService("GetTemps"))
            //.WithMetrics(metrics =>
            // The video "stopped" here, couldn't see what was in WithMetrics (or below)
            // Did see his usings, Metrics, Resources, Traces (but NOT Logs)
            .WithTracing(tracing =>
            {
                tracing.AddHttpClientInstrumentation();
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = otlpEndpoint;
                });
            })
            // Without this I didn't see any ILogger output on the Structured "tab"
            .WithLogging(logging =>
            {
                logging.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = otlpEndpoint;
                });
            });

        builder.Services.AddHostedService<GetTemps>();

        var host = builder.Build();
        host.Run();
    }
}
