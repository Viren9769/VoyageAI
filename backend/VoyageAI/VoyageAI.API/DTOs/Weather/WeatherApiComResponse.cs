using System.Text.Json.Serialization;

namespace VoyageAI.API.DTOs.Weather
{
    /// <summary>
    /// Strongly typed shape of the WeatherAPI.com "current weather" (/current.json) response.
    /// Only the fields required by the Dashboard weather card are mapped.
    /// </summary>
    public class WeatherApiComResponse
    {
        [JsonPropertyName("location")]
        public WeatherApiComLocation? Location { get; set; }

        [JsonPropertyName("current")]
        public WeatherApiComCurrent? Current { get; set; }
    }

    public class WeatherApiComLocation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class WeatherApiComCurrent
    {
        [JsonPropertyName("temp_c")]
        public decimal TempC { get; set; }

        [JsonPropertyName("feelslike_c")]
        public decimal FeelsLikeC { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("wind_kph")]
        public decimal WindKph { get; set; }

        [JsonPropertyName("condition")]
        public WeatherApiComCondition? Condition { get; set; }
    }

    public class WeatherApiComCondition
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}
