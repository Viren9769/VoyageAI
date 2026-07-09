namespace VoyageAI.API.Models.Entities
{
    /// <summary>
    /// Traveler entity representing a person traveling on a specific trip.
    /// 
    /// A Traveler always belongs to exactly ONE Trip.
    /// A Trip can contain multiple Travelers.
    /// Users can only manage Travelers belonging to Trips they own.
    /// 
    /// Hard Delete Pattern:
    /// - Travelers are physically deleted from the database
    /// - Deletion cascades from Trip to Travelers
    /// 
    /// Audit Fields:
    /// - CreatedAt: When the traveler was added to the trip
    /// - CreatedBy: UserId of who added the traveler
    /// - LastModifiedBy: UserId of who last updated the traveler
    /// </summary>
    public class Traveler
    {
        /// <summary>
        /// Unique identifier for the traveler.
        /// </summary>
        public Guid TravelerId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Foreign key to the Trip this traveler belongs to.
        /// CRITICAL: Every traveler must belong to exactly ONE trip.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        /// Traveler's first name.
        /// Required, non-nullable. Length: 1-100 characters.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Traveler's last name.
        /// Required, non-nullable. Length: 1-100 characters.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Traveler's middle name.
        /// Optional. Length: 0-100 characters.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Traveler's date of birth.
        /// Used to calculate age and validate traveler is not in the future.
        /// Maximum age: 120 years.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Traveler's gender (e.g., "Male", "Female", "Other", "Prefer not to say").
        /// Optional. Length: 0-50 characters.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Traveler's email address.
        /// Optional but must be valid if provided.
        /// Length: 0-255 characters.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Traveler's phone number.
        /// Optional. Length: 0-20 characters.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Traveler's nationality.
        /// Optional. Length: 0-100 characters.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Traveler's passport number.
        /// Optional but critical for international travel.
        /// Length: 0-50 characters.
        /// Index on this field for quick lookup.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// The country that issued the traveler's passport.
        /// Optional. Length: 0-100 characters.
        /// </summary>
        public string PassportCountry { get; set; }

        /// <summary>
        /// Date when the traveler's passport expires.
        /// Validation: Must not be in the past (for active trips).
        /// Optional.
        /// </summary>
        public DateTime? PassportExpiry { get; set; }

        /// <summary>
        /// Emergency contact name.
        /// Optional. Length: 0-200 characters.
        /// </summary>
        public string EmergencyContactName { get; set; }

        /// <summary>
        /// Emergency contact phone number.
        /// Optional. Length: 0-20 characters.
        /// </summary>
        public string EmergencyContactPhone { get; set; }

        /// <summary>
        /// Relationship of emergency contact to the traveler (e.g., "Spouse", "Parent", "Sibling").
        /// Optional. Length: 0-50 characters.
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// Dietary preferences or restrictions (e.g., "Vegetarian", "Vegan", "Gluten-free").
        /// Optional. Length: 0-500 characters.
        /// </summary>
        public string DietaryPreference { get; set; }

        /// <summary>
        /// Special requirements or accommodations needed (e.g., "Wheelchair access", "Allergy to shellfish").
        /// Optional. Length: 0-1000 characters.
        /// </summary>
        public string SpecialRequirements { get; set; }

        /// <summary>
        /// Frequent flyer number(s) for airline loyalty programs.
        /// Optional. Length: 0-100 characters.
        /// Can contain multiple numbers separated by commas.
        /// </summary>
        public string FrequentFlyerNumber { get; set; }

        /// <summary>
        /// Known Traveler Number (KTN) from TSA (U.S.) or similar program.
        /// Optional. Length: 0-100 characters.
        /// </summary>
        public string KnownTravelerNumber { get; set; }

        /// <summary>
        /// Indicates if this is the primary traveler for the trip.
        /// Usually the person who created/owns the trip.
        /// Default: false.
        /// </summary>
        public bool IsPrimaryTraveler { get; set; } = false;

        // Age - Computed from DateOfBirth for backward compatibility
        /// <summary>
        /// Deprecated: Age should be calculated from DateOfBirth at runtime.
        /// Kept for backward compatibility.
        /// </summary>
        public int Age { get; set; }

        // ============================================================
        // SOFT DELETE & AUDIT FIELDS
        // ============================================================

        /// <summary>
        /// Indicates if this traveler record is deleted.
        /// Soft delete pattern: IsDeleted = true instead of physical deletion.
        /// Default: false.
        /// 
        /// Index: (TripId, IsDeleted) for efficient querying of active travelers.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// UTC timestamp when the traveler was deleted.
        /// Null if not deleted.
        /// Only set when IsDeleted becomes true.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// UTC timestamp when the traveler record was created.
        /// Set automatically by the service layer.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UTC timestamp when the traveler record was last modified.
        /// Updated every time any field changes.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UserId of the user who created this traveler record.
        /// Extracted from JWT token at creation time.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// UserId of the user who last modified this traveler record.
        /// Updated every time the traveler is modified.
        /// </summary>
        public Guid LastModifiedBy { get; set; }

        // ============================================================
        // NAVIGATION PROPERTIES
        // ============================================================

        /// <summary>
        /// Navigation property to the Trip this traveler belongs to.
        /// CRITICAL: A traveler can only belong to ONE trip.
        /// Always populated by EF Core queries.
        /// </summary>
        public Trip Trip { get; set; }
    }
}
