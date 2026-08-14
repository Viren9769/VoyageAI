namespace VoyageAI.API.DTOs.Weather
{
    /// <summary>
    /// Normalized current weather result returned by <see cref="Services.Interfaces.IWeatherService"/>.
    /// </summary>
    public class WeatherResult
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Temperature { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public decimal FeelsLike { get; set; }
    }
}
