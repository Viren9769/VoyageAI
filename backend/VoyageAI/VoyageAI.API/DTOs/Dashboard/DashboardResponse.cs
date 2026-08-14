namespace VoyageAI.API.DTOs.Dashboard
{
    public class DashboardResponse
    {
        public WelcomeSectionResponse Welcome { get; set; } = new();

        public List<DashboardStatResponse> Stats { get; set; } = new();

        public AiPlannerResponse AiPlanner { get; set; } = new();

        public UpcomingTripResponse UpcomingTrip { get; set; } = new();

        public WeatherCardResponse Weather { get; set; } = new();

        public List<ReminderResponse> Reminders { get; set; } = new();

        public TravelMapResponse TravelMap { get; set; } = new();

        public ExpenseOverviewResponse ExpenseOverview { get; set; } = new();

        public List<RecentTripResponse> RecentTrips { get; set; } = new();

        public TravelerProfileResponse TravelerProfile { get; set; } = new();
    }

    public class WelcomeSectionResponse
    {
        public string Greeting { get; set; } = "Welcome back,";
        public string UserName { get; set; } = "Traveler 👋";
        public string Subtitle { get; set; } = "Ready for your next adventure?";
    }

    public class DashboardStatResponse
    {
        public string Title { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public decimal Change { get; set; }
        public string Trend { get; set; } = "up";
        public string Color { get; set; } = string.Empty;
    }

    public class AiPlannerResponse
    {
        public string Title { get; set; } = "AI Trip Planner";
        public string Subtitle { get; set; } = "Describe your dream trip and let VoyageAI build the perfect itinerary for you.";
        public string Placeholder { get; set; } = "Example: Plan a 7-day trip to Switzerland with a budget of $3000";
        public string ButtonText { get; set; } = "Generate Trip";
    }

    public class UpcomingTripResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int DaysLeft { get; set; }
        public int Progress { get; set; }
    }

    public class WeatherCardResponse
    {
        public string City { get; set; } = "--";
        public string Country { get; set; } = "--";
        public decimal Temperature { get; set; }
        public string Condition { get; set; } = "Unavailable";
        public string Icon { get; set; } = "wb_sunny";
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public decimal FeelsLike { get; set; }
    }

    public class ReminderResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string TimeLeft { get; set; } = string.Empty;
    }

    public class TravelMapResponse
    {
        public string Title { get; set; } = "My Travel Map";
        public string Image { get; set; } = "images/world.svg";
        public List<DestinationResponse> Destinations { get; set; } = new();
    }

    public class DestinationResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Top { get; set; } = string.Empty;
        public string Left { get; set; } = string.Empty;
    }

    public class ExpenseOverviewResponse
    {
        public string Total { get; set; } = "$0";
        public string Period { get; set; } = "This Year";
        public decimal Change { get; set; }
        public List<ExpenseCategoryResponse> Categories { get; set; } = new();
    }

    public class ExpenseCategoryResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public decimal Percent { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class RecentTripResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class TravelerProfileResponse
    {
        public string Name { get; set; } = "Traveler";
        public string Avatar { get; set; } = "images/default-avatar.png";
        public int Level { get; set; }
        public int VoyagePoints { get; set; }
    }
}