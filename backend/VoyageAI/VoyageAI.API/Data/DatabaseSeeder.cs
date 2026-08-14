using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Models.Enums;

namespace VoyageAI.API.Data
{
    public static class DatabaseSeeder
    {
        private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        private static readonly List<TripSeed> Trips = new()
        {
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222221"),
                "Switzerland Escape",
                "Switzerland",
                "Zurich",
                "https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=600",
                new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 26, 0, 0, 0, DateTimeKind.Utc),
                5400m,
                "USD",
                "Luxury",
                "A scenic Switzerland trip through Zurich, Lucerne, and Interlaken.",
                "Active",
                new DateTime(2026, 6, 10, 14, 35, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Tokyo Explorer",
                "Japan",
                "Tokyo",
                "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=600",
                new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                6200m,
                "USD",
                "Adventure",
                "Modern Tokyo highlights with food, tech, and local culture.",
                "Upcoming",
                new DateTime(2026, 7, 2, 9, 20, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222223"),
                "Bali Adventure",
                "Indonesia",
                "Ubud",
                "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=600",
                new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 4, 12, 0, 0, 0, DateTimeKind.Utc),
                3100m,
                "USD",
                "Adventure",
                "A relaxed Bali itinerary around Ubud, temples, and rice terraces.",
                "Completed",
                new DateTime(2026, 4, 12, 20, 10, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222224"),
                "Paris Getaway",
                "France",
                "Paris",
                "https://images.unsplash.com/photo-1431274172761-fca41d930114?w=600",
                new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc),
                4700m,
                "USD",
                "Romantic",
                "A classic Paris trip with museums, views, and city walks.",
                "Completed",
                new DateTime(2026, 5, 22, 18, 0, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222225"),
                "Iceland Road Trip",
                "Iceland",
                "Reykjavik",
                "https://images.unsplash.com/photo-1476610182048-b716b8518aae?w=600",
                new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                7800m,
                "USD",
                "Road Trip",
                "A draft Iceland ring-road plan built around flexible scenic stops.",
                "Draft",
                new DateTime(2026, 8, 3, 11, 0, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222226"),
                "Dubai Luxury",
                "UAE",
                "Dubai",
                "https://images.unsplash.com/photo-1512453979798-5ea266f8880c?w=600",
                new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc),
                9200m,
                "USD",
                "Luxury",
                "Premium Dubai stays, dining, and skyline experiences.",
                "Upcoming",
                new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222227"),
                "New York City Break",
                "USA",
                "New York",
                "https://images.unsplash.com/photo-1499092346589-b9b6be3e94b2?w=600",
                new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 11, 23, 0, 0, 0, DateTimeKind.Utc),
                5100m,
                "USD",
                "Business",
                "A short New York trip for city sights, food, and shows.",
                "Draft",
                new DateTime(2026, 8, 4, 16, 10, 0, DateTimeKind.Utc)),
            new(
                Guid.Parse("22222222-2222-2222-2222-222222222228"),
                "Santorini Getaway",
                "Greece",
                "Santorini",
                "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=600",
                new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 12, 16, 0, 0, 0, DateTimeKind.Utc),
                6500m,
                "USD",
                "Romantic",
                "A scenic Santorini stay with sunset views and island stops.",
                "Completed",
                new DateTime(2026, 1, 5, 9, 10, 0, DateTimeKind.Utc))
        };

        public static async Task SeedAsync(VoyageDbContext context, CancellationToken cancellationToken = default)
        {
            if (await context.Users.AnyAsync(user => user.Email == "demo@voyage.ai", cancellationToken))
            {
                return;
            }

            var demoUser = new User
            {
                UserId = DemoUserId,
                FirstName = "Viren",
                LastName = "Voyager",
                Email = "demo@voyage.ai",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Phone = "+1 (555) 555-0100",
                CountryCode = "US",
                Currency = "USD",
                Language = "en",
                TimeZone = "UTC",
                Theme = "light",
                ProfileImageUrl = "images/avatar.png",
                Status = UserStatus.Active,
                EmailVerified = true,
                ProfileCompleted = true,
                CreatedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            await context.Users.AddAsync(demoUser, cancellationToken);

            foreach (var tripSeed in Trips)
            {
                if (!await context.Trips.AnyAsync(trip => trip.TripId == tripSeed.TripId, cancellationToken))
                {
                    await context.Trips.AddAsync(new Trip
                    {
                        TripId = tripSeed.TripId,
                        UserId = DemoUserId,
                        TripName = tripSeed.TripName,
                        DestinationCountry = tripSeed.DestinationCountry,
                        DestinationCity = tripSeed.DestinationCity,
                        StartDate = tripSeed.StartDate,
                        EndDate = tripSeed.EndDate,
                        Budget = tripSeed.Budget,
                        Currency = tripSeed.Currency,
                        TravelStyle = tripSeed.TravelStyle,
                        Description = tripSeed.Description,
                        CoverImageUrl = tripSeed.CoverImageUrl,
                        Status = tripSeed.Status,
                        CreatedAt = tripSeed.CreatedAt,
                        UpdatedAt = tripSeed.CreatedAt
                    }, cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            var itineraryDays = new List<ItineraryDay>
            {
                Day("31111111-1111-1111-1111-111111111111", Trips[0].TripId, 1, "Arrival in Zurich", new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc), "Arrival", "Arrive, check in, and keep the first evening light.", "", 160m, 120m, "Sunny, 18°C"),
                Day("31111111-1111-1111-1111-111111111112", Trips[0].TripId, 2, "Zurich Old Town", new DateTime(2026, 6, 19, 0, 0, 0, DateTimeKind.Utc), "Culture", "Walking tour through the old town and riverfront.", "Museum and old town stops.", 220m, 0m, "Clear, 20°C"),
                Day("31111111-1111-1111-1111-111111111113", Trips[0].TripId, 3, "Lucerne and Lake Cruise", new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc), "Scenic", "Day trip to Lucerne with a lake cruise.", "Bring a camera.", 260m, 0m, "Partly sunny, 19°C"),
                Day("31111111-1111-1111-1111-111111111114", Trips[0].TripId, 4, "Interlaken Adventure", new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc), "Adventure", "Outdoor adventure day in Interlaken.", "Reserve transport early.", 320m, 0m, "Cool, 17°C"),
                Day("32222222-2222-2222-2222-222222222221", Trips[1].TripId, 1, "Shinjuku Arrival", new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc), "Arrival", "Hotel check-in and evening food walk.", "Tokyo rail pass ready.", 180m, 0m, "Humid, 29°C"),
                Day("32222222-2222-2222-2222-222222222222", Trips[1].TripId, 2, "Asakusa and Senso-ji", new DateTime(2026, 7, 21, 0, 0, 0, DateTimeKind.Utc), "Culture", "Temple visit and local lunch.", "Start early to avoid crowds.", 240m, 0m, "Sunny, 31°C"),
                Day("32222222-2222-2222-2222-222222222223", Trips[1].TripId, 3, "Akihabara and TeamLab", new DateTime(2026, 7, 22, 0, 0, 0, DateTimeKind.Utc), "Tech", "Arcades, shopping, and immersive art.", "Keep free time after dinner.", 300m, 0m, "Cloudy, 30°C"),
                Day("33333333-3333-3333-3333-333333333331", Trips[2].TripId, 1, "Ubud Check-In", new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc), "Arrival", "Villa check-in and pool time.", "Light evening schedule.", 120m, 120m, "Warm, 28°C"),
                Day("33333333-3333-3333-3333-333333333332", Trips[2].TripId, 2, "Rice Terrace Tour", new DateTime(2026, 4, 6, 0, 0, 0, DateTimeKind.Utc), "Nature", "Rice terrace and coffee plantation day.", "Bring water.", 200m, 180m, "Sunny, 29°C"),
                Day("33333333-3333-3333-3333-333333333333", Trips[2].TripId, 3, "Temple Trail", new DateTime(2026, 4, 7, 0, 0, 0, DateTimeKind.Utc), "Culture", "Temple visits and local market dinner.", "Respect dress code.", 180m, 140m, "Partly cloudy, 27°C"),
                Day("34444444-4444-4444-4444-444444444441", Trips[3].TripId, 1, "Arrival in Paris", new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc), "Arrival", "Airport arrival and evening stroll.", "Keep dinner close to hotel.", 150m, 150m, "Mild, 21°C"),
                Day("34444444-4444-4444-4444-444444444442", Trips[3].TripId, 2, "Louvre and Seine", new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Utc), "Culture", "Museum morning and river cruise.", "Book museum slot in advance.", 260m, 220m, "Sunny, 23°C"),
                Day("35555555-5555-5555-5555-555555555551", Trips[4].TripId, 1, "Reykjavik Arrival", new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc), "Arrival", "Pick up the rental car and settle in.", "Road-trip checklist ready.", 250m, 0m, "Windy, 11°C"),
                Day("35555555-5555-5555-5555-555555555552", Trips[4].TripId, 2, "Golden Circle", new DateTime(2026, 9, 11, 0, 0, 0, DateTimeKind.Utc), "Scenic", "Golden Circle scenic drive.", "Drive carefully.", 320m, 0m, "Cool, 10°C")
            };

            foreach (var day in itineraryDays)
            {
                if (!await context.ItineraryDays.AnyAsync(existing => existing.DayId == day.DayId, cancellationToken))
                {
                    await context.ItineraryDays.AddAsync(day, cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            var activities = new List<Activity>
            {
                Activity("41111111-1111-1111-1111-111111111111", itineraryDays[0].DayId, "Flight Arrival", ActivityCategory.Flight, ActivityStatus.Booked, Priority.MustVisit, new TimeOnly(8, 30), new TimeOnly(9, 45), "Zurich Airport", 0m, "LX-4492", "Land at Zurich and clear immigration.", "https://images.unsplash.com/photo-1516483638261-f4dbaf036963?w=800"),
                Activity("41111111-1111-1111-1111-111111111112", itineraryDays[0].DayId, "Hotel Check-in", ActivityCategory.Hotel, ActivityStatus.Booked, Priority.High, new TimeOnly(11, 0), new TimeOnly(11, 45), "Hilton Zurich Airport", 320m, string.Empty, "Early check-in at the city center hotel.", string.Empty),
                Activity("41111111-1111-1111-1111-111111111113", itineraryDays[0].DayId, "Evening Walk", ActivityCategory.Sightseeing, ActivityStatus.Planned, Priority.Low, new TimeOnly(17, 0), new TimeOnly(18, 30), "Bahnhofstrasse", 0m, string.Empty, "Leisure walk around Bahnhofstrasse.", string.Empty),
                Activity("41111111-1111-1111-1111-111111111114", itineraryDays[1].DayId, "Old Town Guided Tour", ActivityCategory.Sightseeing, ActivityStatus.Booked, Priority.MustVisit, new TimeOnly(9, 0), new TimeOnly(11, 30), "Zurich Old Town", 85m, string.Empty, "Local guided walking tour through historic districts.", string.Empty),
                Activity("41111111-1111-1111-1111-111111111115", itineraryDays[1].DayId, "Swiss Lunch", ActivityCategory.Restaurant, ActivityStatus.Booked, Priority.Medium, new TimeOnly(13, 0), new TimeOnly(14, 0), "Restaurant Zeughauskeller", 55m, string.Empty, "Traditional fondue lunch.", string.Empty),
                Activity("42222222-2222-2222-2222-222222222221", itineraryDays[4].DayId, "Airport Limousine Bus", ActivityCategory.Transportation, ActivityStatus.Booked, Priority.High, new TimeOnly(10, 30), new TimeOnly(11, 30), "Haneda Airport", 25m, string.Empty, "Transfer from Haneda to hotel.", string.Empty),
                Activity("42222222-2222-2222-2222-222222222222", itineraryDays[4].DayId, "Shinjuku Dinner", ActivityCategory.Restaurant, ActivityStatus.Planned, Priority.Medium, new TimeOnly(19, 0), new TimeOnly(20, 30), "Shinjuku", 70m, string.Empty, "Welcome dinner in Omoide Yokocho.", string.Empty),
                Activity("43333333-3333-3333-3333-333333333331", itineraryDays[7].DayId, "Villa Transfer", ActivityCategory.Transportation, ActivityStatus.Booked, Priority.High, new TimeOnly(9, 45), new TimeOnly(11, 15), "Ngurah Rai Airport", 35m, string.Empty, "Private transfer to Ubud villa.", string.Empty),
                Activity("44444444-4444-4444-4444-444444444441", itineraryDays[9].DayId, "Temple Trail", ActivityCategory.Sightseeing, ActivityStatus.Planned, Priority.High, new TimeOnly(9, 0), new TimeOnly(12, 0), "Ubud Temples", 40m, string.Empty, "Temple trail and market browsing.", string.Empty),
                Activity("44444444-4444-4444-4444-444444444442", itineraryDays[10].DayId, "Louvre Visit", ActivityCategory.Museum, ActivityStatus.Booked, Priority.MustVisit, new TimeOnly(10, 0), new TimeOnly(13, 0), "Louvre Museum", 60m, string.Empty, "Timed museum entry and river walk.", string.Empty)
            };

            foreach (var activity in activities)
            {
                if (!await context.Activities.AnyAsync(existing => existing.ActivityId == activity.ActivityId, cancellationToken))
                {
                    await context.Activities.AddAsync(activity, cancellationToken);
                }
            }

            var travelers = new List<Traveler>
            {
                Traveler("51111111-1111-1111-1111-111111111111", Trips[0].TripId, "Nikita", "Vishwakarma", "nikita@example.com", "+1 (281) 555-0100", "USA", "P1234561", true, 32),
                Traveler("51111111-1111-1111-1111-111111111112", Trips[0].TripId, "Rahul", "Vishwakarma", "rahul@example.com", "+1 (832) 555-0145", "USA", "P1234562", false, 34),
                Traveler("51111111-1111-1111-1111-111111111113", Trips[1].TripId, "Priya", "Sharma", "priya@example.com", "+1 (713) 555-0189", "India", "P1234563", true, 29),
                Traveler("51111111-1111-1111-1111-111111111114", Trips[2].TripId, "Aarav", "Vishwakarma", "aarav@example.com", "+1 (281) 555-0190", "USA", "P1234564", false, 10),
                Traveler("51111111-1111-1111-1111-111111111115", Trips[2].TripId, "Diya", "Vishwakarma", "diya@example.com", "+1 (281) 555-0181", "USA", "P1234565", false, 7),
                Traveler("51111111-1111-1111-1111-111111111116", Trips[3].TripId, "Meera", "Sharma", "meera@example.com", "+1 (713) 555-0192", "India", "P1234566", false, 31)
            };

            foreach (var traveler in travelers)
            {
                if (!await context.Travelers.AnyAsync(existing => existing.TravelerId == traveler.TravelerId, cancellationToken))
                {
                    await context.Travelers.AddAsync(traveler, cancellationToken);
                }
            }

            var expenses = new List<Expense>
            {
                Expense("61111111-1111-1111-1111-111111111111", Trips[0].TripId, new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc), "Flight to Zurich", "Houston -> Zurich", "Transportation", "Credit Card", 450m),
                Expense("61111111-1111-1111-1111-111111111112", Trips[0].TripId, new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc), "Hilton Zurich Airport", "1 Night Stay", "Accommodation", "Credit Card", 320m),
                Expense("61111111-1111-1111-1111-111111111113", Trips[0].TripId, new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc), "Dinner at Swiss Restaurant", "Traditional Swiss Cuisine", "Food & Dining", "Cash", 65m),
                Expense("61111111-1111-1111-1111-111111111114", Trips[0].TripId, new DateTime(2026, 6, 19, 0, 0, 0, DateTimeKind.Utc), "Swiss National Museum", "Entry Ticket", "Activities", "Credit Card", 42m),
                Expense("61111111-1111-1111-1111-111111111115", Trips[1].TripId, new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc), "Airport Transfer", "Haneda to hotel", "Transportation", "Credit Card", 38m),
                Expense("61111111-1111-1111-1111-111111111116", Trips[2].TripId, new DateTime(2026, 4, 6, 0, 0, 0, DateTimeKind.Utc), "Villa Stay", "Ubud private villa", "Accommodation", "Bank Transfer", 260m),
                Expense("61111111-1111-1111-1111-111111111117", Trips[3].TripId, new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Utc), "Louvre Ticket", "Timed entry", "Activities", "Credit Card", 55m)
            };

            foreach (var expense in expenses)
            {
                if (!await context.Expenses.AnyAsync(existing => existing.ExpenseId == expense.ExpenseId, cancellationToken))
                {
                    await context.Expenses.AddAsync(expense, cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private static ItineraryDay Day(string id, Guid tripId, int dayNumber, string title, DateTime date, string summary, string notes, string? placeholder, decimal estimatedBudget, decimal actualBudget, string weatherSummary)
        {
            return new ItineraryDay
            {
                DayId = Guid.Parse(id),
                TripId = tripId,
                DayNumber = dayNumber,
                Date = date,
                Title = title,
                Summary = summary,
                Notes = notes,
                EstimatedBudget = estimatedBudget,
                ActualBudget = actualBudget,
                WeatherSummary = weatherSummary,
                IsDeleted = false,
                CreatedAt = date,
                UpdatedAt = date,
                CreatedBy = DemoUserId,
                LastModifiedBy = DemoUserId
            };
        }

        private static Activity Activity(string id, Guid dayId, string name, ActivityCategory category, ActivityStatus status, Priority priority, TimeOnly startTime, TimeOnly endTime, string locationName, decimal estimatedCost, string bookingReference, string description, string imageUrl)
        {
            var duration = (endTime.Hour * 60 + endTime.Minute) - (startTime.Hour * 60 + startTime.Minute);

            return new Activity
            {
                ActivityId = Guid.Parse(id),
                DayId = dayId,
                ActivityName = name,
                Description = description,
                Category = (int)category,
                LocationName = locationName,
                Address = string.Empty,
                Latitude = 0m,
                Longitude = 0m,
                StartTime = startTime,
                EndTime = endTime,
                EstimatedCost = estimatedCost,
                ActualCost = estimatedCost,
                BookingReference = bookingReference,
                Website = string.Empty,
                Phone = string.Empty,
                Notes = string.Empty,
                Priority = (int)priority,
                Status = (int)status,
                DurationMinutes = duration,
                ImageUrl = imageUrl,
                IsAiGenerated = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = DemoUserId,
                LastModifiedBy = DemoUserId
            };
        }

        private static Traveler Traveler(string id, Guid tripId, string firstName, string lastName, string email, string phone, string nationality, string passportNumber, bool isPrimary, int age)
        {
            return new Traveler
            {
                TravelerId = Guid.Parse(id),
                TripId = tripId,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = string.Empty,
                DateOfBirth = DateTime.UtcNow.AddYears(-age),
                Gender = string.Empty,
                Email = email,
                Phone = phone,
                Nationality = nationality,
                PassportNumber = passportNumber,
                PassportCountry = nationality,
                PassportExpiry = DateTime.UtcNow.AddYears(5),
                EmergencyContactName = string.Empty,
                EmergencyContactPhone = string.Empty,
                Relationship = string.Empty,
                DietaryPreference = string.Empty,
                SpecialRequirements = string.Empty,
                FrequentFlyerNumber = string.Empty,
                KnownTravelerNumber = string.Empty,
                IsPrimaryTraveler = isPrimary,
                Age = age,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = DemoUserId,
                LastModifiedBy = DemoUserId
            };
        }

        private static Expense Expense(string id, Guid tripId, DateTime expenseDate, string description, string note, string category, string paymentMethod, decimal amount)
        {
            return new Expense
            {
                ExpenseId = Guid.Parse(id),
                TripId = tripId,
                ExpenseDate = expenseDate,
                Description = description,
                Note = note,
                Category = category,
                PaymentMethod = paymentMethod,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = DemoUserId,
                LastModifiedBy = DemoUserId,
                IsDeleted = false
            };
        }

        private record TripSeed(
            Guid TripId,
            string TripName,
            string DestinationCountry,
            string DestinationCity,
            string CoverImageUrl,
            DateTime StartDate,
            DateTime EndDate,
            decimal Budget,
            string Currency,
            string TravelStyle,
            string Description,
            string Status,
            DateTime CreatedAt);
    }
}