using System.Text.Json.Serialization;

namespace GetTemps;

//
// Data-only class that can be serialized to JSON that contains all of the
// properties for 'params' in the form data POSTed to /StnData
//
internal class StnDataParams
{
    public class Element
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    [JsonPropertyName("elems")]
    public List<Element> Elements { get; init; }

    [JsonPropertyName("sid")]
    public string StationId { get; init; }

    [JsonPropertyName("sDate")]
    public string StartDate { get; init; }

    [JsonPropertyName("eDate")]
    public string EndDate { get; init; }

    public StnDataParams(string stationId, string startDate, string endDate)
    {
        Elements =
        [
            new() { Name = "maxt" },
            new() { Name = "mint" },
            new() { Name = "avgt" },
            new() { Name = "pcpn" }
        ];
        StationId = stationId;
        StartDate = startDate;
        EndDate = endDate;
    }

    private const string DATE_FORMAT = "yyyy-MM-dd";

    public StnDataParams(string stationId, DateOnly startDate, DateOnly endDate)
        : this(stationId, startDate.ToString(DATE_FORMAT), endDate.ToString(DATE_FORMAT))
    {
    }
}
