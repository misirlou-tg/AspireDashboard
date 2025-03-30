To run the dashboard run the script ./run-dashboard.sh

The dashboard will be run in docker, localhost:8080 will be redirected to it
The dashboard will output the URL needed to login, just replace the host with
localhost:8080

Running the GetTemps program will send data to the dashboard, it's logging
will show up as Structured Logs, it makes one request, it will show up in
Traces

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
