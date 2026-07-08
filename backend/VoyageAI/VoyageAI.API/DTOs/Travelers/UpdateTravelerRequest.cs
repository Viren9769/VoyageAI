namespace VoyageAI.API.DTOs.Travelers
{
    /// <summary>
    /// DTO for updating an existing traveler on a trip.
    /// Users can only update travelers on trips they own (validated in the service layer).
    /// 
    /// RECOMMENDED UPDATE WORKFLOW:
    /// ═══════════════════════════════════════════════════════════════════════════════════════════════════════════════
    /// 
    /// Step 1: FETCH CURRENT TRAVELER DATA (Pre-fill the form)
    /// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// GET /api/trips/{tripId}/travelers/{travelerId}
    /// Authorization: Bearer {jwt_token}
    /// 
    /// Response: Gets full traveler data including all current values
    /// Example: {
    ///     "travelerId": "550e8400-e29b-41d4-a716-446655440000",
    ///     "firstName": "John",
    ///     "lastName": "Doe",
    ///     "email": "john@example.com",
    ///     "passportNumber": "12345678",
    ///     "dateOfBirth": "1985-05-15T00:00:00Z",
    ///     ...
    /// }
    /// 
    /// Step 2: DISPLAY DATA IN FORM
    /// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// Frontend pre-fills the edit form with the current values from Step 1
    /// User can now see what the current values are
    /// User modifies ONLY the fields they want to change
    /// User leaves other fields as-is
    /// 
    /// Step 3: SEND ONLY CHANGED FIELDS (Partial Update)
    /// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// PUT /api/trips/{tripId}/travelers/{travelerId}
    /// Authorization: Bearer {jwt_token}
    /// Content-Type: application/json
    /// 
    /// {
    ///     "email": "newemail@example.com",
    ///     "dateOfBirth": "1985-05-16T00:00:00Z"
    /// }
    /// 
    /// NOTE: You only send the fields you want to change!
    /// Fields you don't include (firstName, lastName, etc.) are preserved as-is
    /// The API automatically keeps existing values for fields not provided
    /// 
    /// Response: 200 OK with updated traveler (all fields)
    /// {
    ///     "travelerId": "550e8400-e29b-41d4-a716-446655440000",
    ///     "firstName": "John",                    # Unchanged
    ///     "lastName": "Doe",                      # Unchanged
    ///     "email": "newemail@example.com",        # UPDATED ✓
    ///     "dateOfBirth": "1985-05-16T00:00:00Z",  # UPDATED ✓
    ///     "passportNumber": "12345678",           # Unchanged
    ///     ...
    /// }
    /// 
    /// BENEFITS:
    /// ═══════════════════════════════════════════════════════════════════════════════════════════════════════════════
    /// ✓ User sees current values (no need to remember)
    /// ✓ User doesn't have to re-enter fields they don't want to change
    /// ✓ Less data sent over the network
    /// ✓ Less chance of accidental overwrites
    /// ✓ Better UX - intuitive form editing experience
    /// ✓ Less error-prone than requiring all fields to be resent
    /// 
    /// This DTO supports partial updates - include only the fields you want to change!
    /// </summary>
    public class UpdateTravelerRequest
    {
        /// <summary>
        /// The updated first name of the traveler.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// The updated last name of the traveler.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// The updated middle name of the traveler.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// The updated date of birth.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// The updated gender.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// The updated email address.
        /// Optional - only provide if you want to change it.
        /// Must be valid if provided.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The updated phone number.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// The updated nationality.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// The updated passport number.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// The updated country that issued the passport.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? PassportCountry { get; set; }

        /// <summary>
        /// The updated passport expiry date.
        /// Optional - only provide if you want to change it.
        /// Validation ensures it's not already expired.
        /// </summary>
        public DateTime? PassportExpiry { get; set; }

        /// <summary>
        /// The updated emergency contact name.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// The updated emergency contact phone number.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// The updated relationship to the emergency contact.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? Relationship { get; set; }

        /// <summary>
        /// The updated dietary preferences.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? DietaryPreference { get; set; }

        /// <summary>
        /// The updated special requirements.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? SpecialRequirements { get; set; }

        /// <summary>
        /// The updated frequent flyer numbers.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? FrequentFlyerNumber { get; set; }

        /// <summary>
        /// The updated Known Traveler Number.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public string? KnownTravelerNumber { get; set; }

        /// <summary>
        /// Update whether this is the primary traveler.
        /// Optional - only provide if you want to change it.
        /// </summary>
        public bool? IsPrimaryTraveler { get; set; }
    }
}
