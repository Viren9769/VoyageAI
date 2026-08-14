namespace VoyageAI.API.Configuration
{
    /// <summary>
    /// WeatherAPI.com configuration settings for retrieving current weather by location.
    /// Binds to appsettings.json WeatherApi section (ApiKey should be set via User Secrets locally).
    /// </summary>
    public class WeatherApiOptions
    {
        /// <summary>
        /// Gets or sets the base URL of the weather API (e.g. https://api.weatherapi.com/v1).
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key used to authenticate requests to the weather API.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
