using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GetTemps;

//
// I started this after watching a video (see comments in the code) where
// he said it was possible to add OpenTelemetry integration to an existing
// program and have send that info to the Aspire dashboard that runs in
// a container.
//
// This link discusses how to use the "standalone" Aspire dashboard:
// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-otlp-example
// More good OpenTelemetry, includes a link to a "fully functional" example:
// https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Extensions.Hosting/README.md
// Good overviews, etc, links to MS reference docs:
// https://opentelemetry.io/docs/languages/dotnet/
//

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
        // See where to push the telemetry to, default to local
        var otlpEndpoint = new Uri(builder.Configuration["OTLP_ENDPOINT_URL"] ?? "http://localhost:4317");
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(c => c.AddService("GetTemps"))
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
            })
            .WithMetrics(metrics =>
            {
                metrics.AddHttpClientInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = otlpEndpoint;
                });
            });

        builder.Services.AddHostedService<GetTemps>();

        var host = builder.Build();
        host.Run();
    }
}
