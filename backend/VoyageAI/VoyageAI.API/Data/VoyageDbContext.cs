using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Data
{
    public class VoyageDbContext : DbContext
    {
        public VoyageDbContext(DbContextOptions<VoyageDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<ItineraryDay> ItineraryDays { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CountryCode).HasMaxLength(2);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.Property(e => e.Language).HasMaxLength(20);
                entity.Property(e => e.TimeZone).HasMaxLength(50);
                entity.Property(e => e.Theme).HasMaxLength(20);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(e => e.RefreshTokens)
                    .WithOne(rt => rt.User)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken Entity Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.RefreshTokenId);
                entity.Property(e => e.RefreshTokenId).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.UserId);
            });

            // Trip Entity Configuration
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.TripId);
                entity.Property(e => e.TripId).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.TripName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.DestinationCountry).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DestinationCity).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Budget).HasPrecision(10, 2);
                entity.Property(e => e.Currency).HasMaxLength(10);
                entity.Property(e => e.TravelStyle).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Trips)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Traveler Entity Configuration
            modelBuilder.Entity<Traveler>(entity =>
            {
                entity.HasKey(e => e.TravelerId);
                entity.Property(e => e.TravelerId).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.MiddleName).HasMaxLength(100);
                entity.Property(e => e.DateOfBirth);
                entity.Property(e => e.Gender).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Nationality).HasMaxLength(100);
                entity.Property(e => e.PassportNumber).HasMaxLength(50);
                entity.Property(e => e.PassportCountry).HasMaxLength(100);
                entity.Property(e => e.PassportExpiry);
                entity.Property(e => e.EmergencyContactName).HasMaxLength(200);
                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
                entity.Property(e => e.Relationship).HasMaxLength(50);
                entity.Property(e => e.DietaryPreference).HasMaxLength(500);
                entity.Property(e => e.SpecialRequirements).HasMaxLength(1000);
                entity.Property(e => e.FrequentFlyerNumber).HasMaxLength(100);
                entity.Property(e => e.KnownTravelerNumber).HasMaxLength(100);
                entity.Property(e => e.IsPrimaryTraveler).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes for performance
                entity.HasIndex(e => e.TripId);
                entity.HasIndex(e => e.PassportNumber);
                entity.HasIndex(e => e.Email);

                entity.HasOne(e => e.Trip)
                    .WithMany(t => t.Travelers)
                    .HasForeignKey(e => e.TripId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ItineraryDay Entity Configuration
            modelBuilder.Entity<ItineraryDay>(entity =>
            {
                entity.HasKey(e => e.DayId);
                entity.Property(e => e.DayId).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Summary).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(3000);
                entity.Property(e => e.WeatherSummary).HasMaxLength(500);
                entity.Property(e => e.EstimatedBudget).HasPrecision(10, 2);
                entity.Property(e => e.ActualBudget).HasPrecision(10, 2);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes for performance
                entity.HasIndex(e => e.TripId);
                entity.HasIndex(e => new { e.TripId, e.DayNumber }).IsUnique();
                entity.HasIndex(e => new { e.TripId, e.Date });
                entity.HasIndex(e => new { e.TripId, e.IsDeleted });

                entity.HasOne(e => e.Trip)
                    .WithMany(t => t.ItineraryDays)
                    .HasForeignKey(e => e.TripId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Activity Entity Configuration
            modelBuilder.Entity<Activity>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.ActivityId);
                entity.Property(e => e.ActivityId).HasDefaultValueSql("gen_random_uuid()");

                // Required Properties
                entity.Property(e => e.ActivityName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.DayId).IsRequired();

                // Optional String Properties with Max Length
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.LocationName).HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.BookingReference).HasMaxLength(100);
                entity.Property(e => e.Website).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                // Enum Properties (stored as int in database)
                entity.Property(e => e.Category);
                entity.Property(e => e.Priority);
                entity.Property(e => e.Status);

                // Coordinate Properties with Precision
                entity.Property(e => e.Latitude).HasPrecision(10, 8).HasDefaultValue(0);
                entity.Property(e => e.Longitude).HasPrecision(11, 8).HasDefaultValue(0);

                // Currency Properties with Precision
                entity.Property(e => e.EstimatedCost).HasPrecision(10, 2).HasDefaultValue(0);
                entity.Property(e => e.ActualCost).HasPrecision(10, 2).HasDefaultValue(0);

                // Time Properties
                // TimeOnly is stored as time without time zone in PostgreSQL
                entity.Property(e => e.StartTime);
                entity.Property(e => e.EndTime);

                // Duration and Meta
                entity.Property(e => e.DurationMinutes).HasDefaultValue(0);
                entity.Property(e => e.IsAiGenerated).HasDefaultValue(false);

                // Soft Delete and Audit Fields
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes for Performance
                entity.HasIndex(e => e.DayId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.StartTime);
                entity.HasIndex(e => new { e.DayId, e.IsDeleted });

                // Foreign Key Relationship
                entity.HasOne(e => e.ItineraryDay)
                    .WithMany(id => id.Activities)
                    .HasForeignKey(e => e.DayId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
