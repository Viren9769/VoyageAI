using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VoyageAI.API.Configuration;
using VoyageAI.API.DTOs.Weather;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Retrieves current weather data from WeatherAPI.com for a given city/country.
    /// Results are cached briefly per destination to avoid redundant external calls.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

        private readonly HttpClient _httpClient;
        private readonly WeatherApiOptions _options;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiOptions> options, IMemoryCache cache, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WeatherResult?> GetCurrentWeatherAsync(string city, string country, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                _logger.LogWarning("Cannot retrieve weather: destination city is missing");
                return null;
            }

            if (string.IsNullOrWhiteSpace(_options.BaseUrl) || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("Weather API is not configured; skipping weather lookup for {City}", city);
                return null;
            }

            var location = string.IsNullOrWhiteSpace(country) ? city : $"{city},{country}";
            var cacheKey = $"weather:{location.ToLowerInvariant()}";

            if (_cache.TryGetValue(cacheKey, out WeatherResult? cached))
            {
                return cached;
            }

            try
            {
                var query = $"?key={Uri.EscapeDataString(_options.ApiKey)}&q={Uri.EscapeDataString(location)}&aqi=no";
                using var response = await _httpClient.GetAsync(_options.BaseUrl.TrimEnd('/') + "/current.json" + query, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("WeatherAPI.com returned {StatusCode} for {Location}", response.StatusCode, location);
                    return null;
                }

                var payload = await response.Content.ReadFromJsonAsync<WeatherApiComResponse>(cancellationToken: cancellationToken);
                if (payload?.Current == null)
                {
                    _logger.LogWarning("WeatherAPI.com returned an unexpected payload for {Location}", location);
                    return null;
                }

                var conditionText = payload.Current.Condition?.Text;
                var result = new WeatherResult
                {
                    City = string.IsNullOrWhiteSpace(payload.Location?.Name) ? city : payload.Location!.Name!,
                    Country = string.IsNullOrWhiteSpace(payload.Location?.Country) ? country : payload.Location!.Country!,
                    Temperature = Math.Round(payload.Current.TempC, 0),
                    Condition = string.IsNullOrWhiteSpace(conditionText) ? "Unavailable" : conditionText!,
                    Icon = MapIcon(conditionText),
                    Humidity = payload.Current.Humidity,
                    WindSpeed = (int)Math.Round(payload.Current.WindKph, 0),
                    FeelsLike = Math.Round(payload.Current.FeelsLikeC, 0)
                };

                _cache.Set(cacheKey, result, CacheDuration);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve weather for {City}, {Country}", city, country);
                return null;
            }
        }

        // Angular renders weather.icon as a mat-icon ligature, so map WeatherAPI.com's condition text to one.
        private static string MapIcon(string? conditionText)
        {
            if (string.IsNullOrWhiteSpace(conditionText))
            {
                return "wb_sunny";
            }

            var text = conditionText.ToLowerInvariant();
            return text switch
            {
                _ when text.Contains("thunder") => "thunderstorm",
                _ when text.Contains("snow") || text.Contains("sleet") || text.Contains("ice") || text.Contains("blizzard") => "ac_unit",
                _ when text.Contains("rain") || text.Contains("drizzle") || text.Contains("shower") => "rainy",
                _ when text.Contains("fog") || text.Contains("mist") || text.Contains("haze") => "foggy",
                _ when text.Contains("cloud") || text.Contains("overcast") => "wb_cloudy",
                _ when text.Contains("clear") || text.Contains("sunny") => "wb_sunny",
                _ => "wb_sunny"
            };
        }
    }
}
