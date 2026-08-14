using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Dashboard;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IItineraryRepository _itineraryRepository;
        private readonly ITravelerRepository _travelerRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWeatherService _weatherService;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            ITripRepository tripRepository,
            IItineraryRepository itineraryRepository,
            ITravelerRepository travelerRepository,
            IExpenseRepository expenseRepository,
            IUserRepository userRepository,
            IWeatherService weatherService,
            ILogger<DashboardService> logger)
        {
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _itineraryRepository = itineraryRepository ?? throw new ArgumentNullException(nameof(itineraryRepository));
            _travelerRepository = travelerRepository ?? throw new ArgumentNullException(nameof(travelerRepository));
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<DashboardResponse>> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var trips = await _tripRepository.GetUserTripsAsync(userId, cancellationToken);
                var today = DateTime.UtcNow.Date;
                var upcomingTrip = trips
                    .Where(trip => trip.StartDate.Date >= today)
                    .OrderBy(trip => trip.StartDate)
                    .FirstOrDefault();

                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                var totalTravelers = 0;
                var totalDays = 0;
                var totalBudget = 0m;
                var totalSpent = 0m;

                foreach (var trip in trips)
                {
                    var travelers = await _travelerRepository.GetTravelersByTripAsync(trip.TripId, cancellationToken);
                    totalTravelers += travelers.Count;

                    var days = await _itineraryRepository.GetByTripIdAsync(trip.TripId, cancellationToken);
                    totalDays += days.Count;
                    totalBudget += trip.Budget;

                    var expenses = await _expenseRepository.GetByTripIdAsync(trip.TripId, cancellationToken);
                    totalSpent += expenses.Sum(expense => expense.Amount);
                }

                var userName = user is null ? "Traveler" : $"{user.FirstName} {user.LastName}".Trim();
                
                var dashboard = new DashboardResponse
                {
                    Welcome = new WelcomeSectionResponse
                    {
                        Greeting = "Welcome back,",
                        UserName = userName + " 👋",
                        Subtitle = "Ready for your next adventure?"
                    },
                    Stats = new List<DashboardStatResponse>
                    {
                        new()
                        {
                            Title = "Total Trips",
                            Value = trips.Count,
                            Icon = "luggage",
                            Change = trips.Count,
                            Trend = "up",
                            Color = "#8B5CF6"
                        },
                        new()
                        {
                            Title = "Countries Visited",
                            Value = trips.Select(trip => trip.DestinationCountry).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                            Icon = "public",
                            Change = trips.Select(trip => trip.DestinationCountry).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                            Trend = "up",
                            Color = "#3B82F6"
                        },
                        new()
                        {
                            Title = "Total Budget",
                            Value = $"${totalBudget:0.00}",
                            Icon = "account_balance_wallet",
                            Change = totalSpent > 0 ? Math.Round((totalSpent / Math.Max(totalBudget, 1m)) * 100m, 0) : 0,
                            Trend = totalSpent > totalBudget ? "up" : "down",
                            Color = "#F59E0B"
                        },
                        new()
                        {
                            Title = "Total Travelers",
                            Value = totalTravelers,
                            Icon = "groups",
                            Change = totalTravelers,
                            Trend = "up",
                            Color = "#10B981"
                        }
                    },
                    AiPlanner = new AiPlannerResponse
                    {
                        Title = "AI Trip Planner",
                        Subtitle = "Describe your dream trip and let VoyageAI build the perfect itinerary for you.",
                        Placeholder = "Example: Plan a 7-day trip to Switzerland with a budget of $3000",
                        ButtonText = "Generate Trip"
                    },
                    UpcomingTrip = upcomingTrip == null
                        ? new UpcomingTripResponse()
                        : BuildUpcomingTrip(upcomingTrip, today),
                    Weather = await BuildWeatherAsync(upcomingTrip, cancellationToken),
                    Reminders = BuildReminders(upcomingTrip),
                    TravelMap = BuildTravelMap(trips),
                    ExpenseOverview = BuildExpenseOverview(totalBudget, totalSpent),
                    RecentTrips = trips
                        .OrderByDescending(trip => trip.CreatedAt)
                        .Take(4)
                        .Select(BuildRecentTrip)
                        .ToList(),
                    TravelerProfile = new TravelerProfileResponse
                    {
                        Name = userName,
                        Level = Math.Min(99, Math.Max(1, trips.Count * 5)),
                        VoyagePoints = Math.Max(0, trips.Count * 120 + totalTravelers * 35 + totalDays * 10)
                    }
                };

                return ApiResponse<DashboardResponse>.SuccessResponse(dashboard, "Dashboard data retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard for user {UserId}", userId);
                throw;
            }
        }

        private static UpcomingTripResponse BuildUpcomingTrip(VoyageAI.API.Models.Entities.Trip trip, DateTime today)
        {
            var daysLeft = Math.Max(0, (trip.StartDate.Date - today).Days);
            var duration = Math.Max(1, (trip.EndDate.Date - trip.StartDate.Date).Days + 1);
            var elapsed = Math.Clamp((today - trip.StartDate.Date).Days + 1, 0, duration);
            var progress = duration == 0 ? 0 : (int)Math.Round((elapsed / (decimal)duration) * 100m, 0);

            return new UpcomingTripResponse
            {
                Title = trip.TripName,
                Destination = trip.DestinationCountry,
                Image = trip.CoverImageUrl ?? string.Empty,
                StartDate = trip.StartDate.ToString("MMM dd, yyyy"),
                EndDate = trip.EndDate.ToString("MMM dd, yyyy"),
                DaysLeft = daysLeft,
                Progress = progress
            };
        }

        private async Task<WeatherCardResponse> BuildWeatherAsync(VoyageAI.API.Models.Entities.Trip? upcomingTrip, CancellationToken cancellationToken)
        {
            if (upcomingTrip == null || string.IsNullOrWhiteSpace(upcomingTrip.DestinationCity))
            {
                return new WeatherCardResponse();
            }

            try
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(
                    upcomingTrip.DestinationCity,
                    upcomingTrip.DestinationCountry,
                    cancellationToken);

                if (weather == null)
                {
                    return new WeatherCardResponse
                    {
                        City = upcomingTrip.DestinationCity,
                        Country = upcomingTrip.DestinationCountry
                    };
                }

                return new WeatherCardResponse
                {
                    City = weather.City,
                    Country = weather.Country,
                    Temperature = weather.Temperature,
                    Condition = weather.Condition,
                    Icon = weather.Icon,
                    Humidity = weather.Humidity,
                    WindSpeed = weather.WindSpeed,
                    FeelsLike = weather.FeelsLike
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build weather card for trip {TripId}", upcomingTrip.TripId);
                return new WeatherCardResponse
                {
                    City = upcomingTrip.DestinationCity,
                    Country = upcomingTrip.DestinationCountry
                };
            }
        }

        private static List<ReminderResponse> BuildReminders(VoyageAI.API.Models.Entities.Trip? upcomingTrip)
        {
            if (upcomingTrip == null)
            {
                return new List<ReminderResponse>();
            }

            return new List<ReminderResponse>
            {
                new()
                {
                    Title = $"{upcomingTrip.TripName} starts soon",
                    Date = upcomingTrip.StartDate.ToString("MMM dd, yyyy • h:mm tt"),
                    Icon = "flight_takeoff",
                    Color = "#2563EB",
                    TimeLeft = "Upcoming"
                }
            };
        }

        private static TravelMapResponse BuildTravelMap(IEnumerable<VoyageAI.API.Models.Entities.Trip> trips)
        {
            var destinations = trips
                .Select((trip, index) => new DestinationResponse
                {
                    Name = trip.DestinationCountry,
                    Top = $"{30 + (index * 7) % 45}%",
                    Left = $"{15 + (index * 11) % 70}%"
                })
                .DistinctBy(destination => destination.Name)
                .Take(4)
                .ToList();

            return new TravelMapResponse
            {
                Destinations = destinations
            };
        }

        private static ExpenseOverviewResponse BuildExpenseOverview(decimal totalBudget, decimal totalSpent)
        {
            var remaining = Math.Max(totalBudget - totalSpent, 0);
            return new ExpenseOverviewResponse
            {
                Total = $"${totalSpent:0.00}",
                Change = totalBudget > 0 ? Math.Round(((totalSpent - totalBudget) / totalBudget) * 100m, 0) : 0,
                Categories = new List<ExpenseCategoryResponse>
                {
                    new() { Name = "Accommodation", Amount = $"${remaining * 0.40m:0.00}", Percent = 40, Color = "#6D5DFB" },
                    new() { Name = "Transportation", Amount = $"${remaining * 0.25m:0.00}", Percent = 25, Color = "#3B82F6" },
                    new() { Name = "Food", Amount = $"${remaining * 0.15m:0.00}", Percent = 15, Color = "#4FD1C5" },
                    new() { Name = "Activities", Amount = $"${remaining * 0.10m:0.00}", Percent = 10, Color = "#FBBF24" },
                    new() { Name = "Transport", Amount = $"${remaining * 0.05m:0.00}", Percent = 5, Color = "#F87171" }
                }
            };
        }

        private static RecentTripResponse BuildRecentTrip(VoyageAI.API.Models.Entities.Trip trip)
        {
            return new RecentTripResponse
            {
                Title = trip.TripName,
                Country = trip.DestinationCountry,
                Image = trip.CoverImageUrl ?? string.Empty,
                StartDate = trip.StartDate.ToString("MMM dd, yyyy"),
                EndDate = trip.EndDate.ToString("MMM dd, yyyy"),
                Status = trip.Status
            };
        }
    }
}