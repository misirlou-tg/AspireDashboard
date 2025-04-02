This repo contains a script to run the Aspire dashboard as a standalone
app (in docker) and a sample console app that will publish OpenTelemetry
data to it.

Standalone .NET Aspire dashboard
https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone

=== Running

To run the dashboard run the script ./run-dashboard.sh

The dashboard will be run in a mode where it allows anonymous access, just
browse to http://localhost:8080/

Running the GetTemps program will send Logging, Metrics and Traces to the
dashboard. It only makes one REST request which will show up in Traces.

=== Console logs

When you run the Aspire dashboard as part of an Aspire "app host" project
it will show more info, there will be a section on the left for Console logs.
This seems to come from a custom API (not OpenTelemetry), the following link
discusses how to configure the dashboard for it, but no details on how to
implement that API.
https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/configuration#resources

===

Notes from 8/18

https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash

Here's the video where I saw the demo of the "standalone" dashboard
Keynote: Introducing .NET Aspire - Cloud Native Development for .NET - Scott Hunter
https://www.youtube.com/watch?v=-jrUiq21gS4
(skip ahead to 33:20)

Setup the export of the logging... it seems to be working
- Diff: His structured logs page had a "Trace" column
        All of HIS logging had the same trace value
- His OT imports were:
  OpenTelemetry;
  OpenTelemetry.Metrics;
  OpenTelemetry.Resources;
  OpenTelemetry.Trace;

Copied some code from here
https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel
