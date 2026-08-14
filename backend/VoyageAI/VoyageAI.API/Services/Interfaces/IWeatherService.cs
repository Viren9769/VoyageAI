using VoyageAI.API.DTOs.Weather;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Retrieves current weather information for a given destination.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Gets the current weather for the given city/country, or null if it cannot be retrieved.
        /// </summary>
        Task<WeatherResult?> GetCurrentWeatherAsync(string city, string country, CancellationToken cancellationToken = default);
    }
}
