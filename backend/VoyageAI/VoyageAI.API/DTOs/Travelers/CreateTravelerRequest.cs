namespace VoyageAI.API.DTOs.Travelers
{
    /// <summary>
    /// DTO for creating a new traveler on a trip.
    /// This request is sent by authenticated users to add a traveler to their trip.
    /// 
    /// The TripId is provided as a route parameter (from URL path), not the request body.
    /// This ensures users cannot arbitrarily add travelers to trips they don't own.
    /// Ownership verification is performed by the service layer.
    /// </summary>
    public class CreateTravelerRequest
    {
        /// <summary>
        /// Traveler's first name.
        /// Required field. Length: 1-100 characters.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Traveler's last name.
        /// Required field. Length: 1-100 characters.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Traveler's middle name.
        /// Optional field. Length: 0-100 characters.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Traveler's date of birth.
        /// Optional but recommended for travel documents and itinerary planning.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Traveler's gender (e.g., "Male", "Female", "Other", "Prefer not to say").
        /// Optional field. Length: 0-50 characters.
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Traveler's email address.
        /// Optional but must be valid if provided.
        /// Used for communication purposes.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Traveler's phone number.
        /// Optional field. Length: 0-20 characters.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Traveler's nationality.
        /// Optional field. Length: 0-100 characters.
        /// Important for visa and entry requirements.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// Traveler's passport number.
        /// Optional but critical for international travel.
        /// Length: 0-50 characters.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// The country that issued the traveler's passport.
        /// Optional field. Length: 0-100 characters.
        /// Should be provided together with PassportNumber.
        /// </summary>
        public string? PassportCountry { get; set; }

        /// <summary>
        /// Date when the traveler's passport expires.
        /// Optional but critical for international travel.
        /// Validation ensures it's not already expired.
        /// </summary>
        public DateTime? PassportExpiry { get; set; }

        /// <summary>
        /// Emergency contact name.
        /// Optional field. Length: 0-200 characters.
        /// </summary>
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// Emergency contact phone number.
        /// Optional field. Length: 0-20 characters.
        /// </summary>
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// Relationship of emergency contact to the traveler
        /// (e.g., "Spouse", "Parent", "Sibling", "Friend").
        /// Optional field. Length: 0-50 characters.
        /// </summary>
        public string? Relationship { get; set; }

        /// <summary>
        /// Dietary preferences or restrictions
        /// (e.g., "Vegetarian", "Vegan", "Gluten-free", "Kosher", "Halal").
        /// Optional field. Length: 0-500 characters.
        /// Important for meal planning and restaurant reservations.
        /// </summary>
        public string? DietaryPreference { get; set; }

        /// <summary>
        /// Special requirements or accommodations needed
        /// (e.g., "Wheelchair access", "Allergy to shellfish", "Requires translator").
        /// Optional field. Length: 0-1000 characters.
        /// Critical for accessibility and safety planning.
        /// </summary>
        public string? SpecialRequirements { get; set; }

        /// <summary>
        /// Frequent flyer number(s) for airline loyalty programs.
        /// Optional field. Length: 0-100 characters.
        /// Can contain multiple numbers separated by commas.
        /// Useful for earning miles on bookings.
        /// </summary>
        public string? FrequentFlyerNumber { get; set; }

        /// <summary>
        /// Known Traveler Number (KTN) from TSA (U.S.) or similar program.
        /// Optional field. Length: 0-100 characters.
        /// Speeds up airport security screening.
        /// </summary>
        public string? KnownTravelerNumber { get; set; }

        /// <summary>
        /// Indicates if this is the primary traveler for the trip.
        /// Usually the person who created/owns the trip.
        /// Only one traveler per trip should be marked as primary.
        /// Default: false.
        /// </summary>
        public bool IsPrimaryTraveler { get; set; } = false;
    }
}
