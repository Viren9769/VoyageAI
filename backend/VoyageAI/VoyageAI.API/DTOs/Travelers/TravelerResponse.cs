namespace VoyageAI.API.DTOs.Travelers
{
    /// <summary>
    /// DTO for returning traveler information in API responses.
    /// This is used when retrieving travelers from the API.
    /// 
    /// Contains all publicly displayable information about a traveler.
    /// Sensitive information (like full passport details) should be restricted
    /// through proper authorization and returned only to the trip owner.
    /// </summary>
    public class TravelerResponse
    {
        /// <summary>
        /// The unique identifier of the traveler.
        /// </summary>
        public Guid TravelerId { get; set; }

        /// <summary>
        /// The ID of the trip this traveler belongs to.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        /// The traveler's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The traveler's last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The traveler's middle name (if provided).
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// The traveler's date of birth (if provided).
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// The traveler's gender (if provided).
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// The traveler's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The traveler's phone number.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// The traveler's nationality (if provided).
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// The traveler's passport number.
        /// Sensitive information - only returned to trip owner with proper authorization.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// The country that issued the traveler's passport.
        /// </summary>
        public string? PassportCountry { get; set; }

        /// <summary>
        /// The date when the traveler's passport expires.
        /// Important for verifying travel readiness.
        /// </summary>
        public DateTime? PassportExpiry { get; set; }

        /// <summary>
        /// The emergency contact name for this traveler.
        /// </summary>
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// The emergency contact phone number for this traveler.
        /// </summary>
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// The relationship of the emergency contact to the traveler.
        /// </summary>
        public string? Relationship { get; set; }

        /// <summary>
        /// The traveler's dietary preferences/restrictions.
        /// Important for meal planning and restaurant reservations.
        /// </summary>
        public string? DietaryPreference { get; set; }

        /// <summary>
        /// Special requirements or accommodations for this traveler.
        /// Examples: wheelchair access, allergy warnings, etc.
        /// </summary>
        public string? SpecialRequirements { get; set; }

        /// <summary>
        /// Frequent flyer numbers for airline loyalty programs.
        /// Can contain multiple numbers separated by commas.
        /// </summary>
        public string? FrequentFlyerNumber { get; set; }

        /// <summary>
        /// Known Traveler Number (KTN) from TSA or similar programs.
        /// Helps expedite airport security screening.
        /// </summary>
        public string? KnownTravelerNumber { get; set; }

        /// <summary>
        /// Indicates if this is the primary traveler for the trip.
        /// Usually the person who created/owns the trip.
        /// </summary>
        public bool IsPrimaryTraveler { get; set; }

        /// <summary>
        /// When the traveler record was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the traveler record was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Calculated age of the traveler based on DateOfBirth.
        /// Returns null if DateOfBirth is not provided.
        /// </summary>
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                    return null;

                var today = DateTime.UtcNow;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }

        /// <summary>
        /// Indicates how many days until the passport expires.
        /// Returns null if PassportExpiry is not provided.
        /// Returns negative number if passport is already expired.
        /// </summary>
        public int? DaysUntilPassportExpiry
        {
            get
            {
                if (!PassportExpiry.HasValue)
                    return null;

                return (int)(PassportExpiry.Value.Date - DateTime.UtcNow.Date).TotalDays;
            }
        }

        /// <summary>
        /// Indicates if the traveler's passport is currently valid.
        /// Returns false if PassportExpiry is not provided.
        /// </summary>
        public bool IsPassportValid
        {
            get
            {
                if (!PassportExpiry.HasValue)
                    return false;

                return PassportExpiry.Value >= DateTime.UtcNow;
            }
        }
    }
}
