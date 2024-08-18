using System.Text.Json;

namespace GetTemps;

//
// Found out about this web service (https://data.rcc-acis.org/StnData) by
// capturing network activity when visiting weather.gov:
// https://www.weather.gov/wrh/Climate?wfo=fwd
//
// Later I found this article about the service which pointed me to a "builder"
// https://journals.ametsoc.org/view/journals/bams/96/2/bams-d-13-00032.1.xml
// http://builder.rcc-acis.org/
//
// The request weather.gov made had more headers (CORS stuff), in the code below
// I kept what I thought was relevant. The request was a POST of form data
// (content-type: application/x-www-form-urlencoded; charset=UTF-8) with the
// following payload (where 'params' and 'output' are the form's field names):
// params: {
//   "elems":[
//     {"name":"maxt","add":"t"},
//     {"name":"mint","add":"t"},
//     {"name":"avgt","add":"t"},
//     {"name":"avgt","normal":"departure91","add":"t"},
//     {"name":"hdd","add":"t"},
//     {"name":"cdd","add":"t"},
//     {"name":"pcpn","add":"t"},
//     {"name":"snow","add":"t"},
//     {"name":"snwd","add":"t"}
//   ],
//   "sid":"DFWthr 9",
//   "sDate":"2022-07-01",
//   "eDate":"2022-07-31"
// }
// output: json
// 
// I removed the data I wasn't interested in and the "add": "t" (it was 24 everywhere,
// not exactly sure what that was for, time/hours?)
//
// Other stations:
// ; Dallas/Ft Worth Area
// "sid":"DFWthr 9"
// ; DFW Airport [DAL-FTW WSCMO AP, in my tests same data as DFW Area]
// "sid":"KDFW 5"
// ; Addison Airport (doesn't show up in list/map, but found KADS in forecasts)
// "sid":"KADS 5"
// ; Denton ASOS [Denton Enterprise Airport (KDTO)]
// "sid":"KDTO 5"
// ; Denton 2 SE (South East city not county, in link below can find lat/long)
// ; http://research.jisao.washington.edu/greg/southcentral/states/TX/412404.html
// "sid":"412404 2"
// ; USC
// "sid": "KCQT"
//

public sealed class GetTemps(ILogger<GetTemps> _logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var stnDataParams = JsonSerializer.Serialize(
            new StnDataParams("KCQT", new DateOnly(2024, 8, 1), new DateOnly(2024, 8, 16)));

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://data.rcc-acis.org")
        };
        var response = await httpClient.PostAsync(
            "/StnData",
            new FormUrlEncodedContent([new("params", stnDataParams), new("output", "json")]),
            stoppingToken);
        var body = await response.Content.ReadAsStringAsync(stoppingToken);

        _logger.LogInformation("Response body: {body}", body);
    }
}
