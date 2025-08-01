using System.Text.Json.Serialization;
using SharedModule.Converters;

namespace SharedModule;

public class WeatherForecast
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    [JsonPropertyName("temperatureC")]
    public int TemperatureC { get; set; }

    [JsonPropertyName("temperatureF")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("description")]
    public string? Description => $"{Summary} at {TemperatureC}°C";

    [JsonIgnore] // Ignore all properties by default
    public string? InternalNote { get; set; }
}

public class WeatherForecastCustomFormatDto : WeatherForecast
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyJsonConverterType2))] // Custom format: "yyyy/MM/dd"
    public DateOnly Date { get; set; }
}