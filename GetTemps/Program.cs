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
// Good overview on distributed tracing:
// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing
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
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
        });

        // See where to push the telemetry to, default to local
        var otlpEndpoint = new Uri(builder.Configuration["OTLP_ENDPOINT_URL"] ?? "http://localhost:4317");
        // You can tell the exporters the protocol also, gRPC is the default
        //var otlpProtocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(c => c.AddService("GetTemps")) // Could also supply service version & instance ID
            // The video "stopped" here, couldn't see what was in WithMetrics (or below)
            // Did see his usings, Metrics, Resources, Traces (but NOT Logs)
            .WithTracing(tracing =>
            {
                tracing.AddHttpClientInstrumentation()
                    .AddSource(GetTemps.ACTIVITY_SOURCE_NAME) // This class creates activities
                    .AddOtlpExporter(otlpOptions =>
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
                metrics.AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = otlpEndpoint;
                    });
            });

        builder.Services.AddHostedService<GetTemps>();

        var host = builder.Build();
        host.Run();
    }
}
