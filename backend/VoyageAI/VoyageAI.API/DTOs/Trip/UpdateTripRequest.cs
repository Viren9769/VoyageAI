namespace VoyageAI.API.DTOs.Trip
{
    /// <summary>
    /// DTO for updating an existing trip.
    /// Users can only update trips they own (validated in the service layer).
    /// 
    /// RECOMMENDED UPDATE WORKFLOW:
    /// ═════════════════════════════════════════════════════════════════
    /// 
    /// Step 1: FETCH CURRENT TRIP DATA (Pre-fill the form)
    /// ────────────────────────────────────────────────────────────────
    /// GET /api/trips/{tripId}
    /// Authorization: Bearer {jwt_token}
    /// 
    /// Response: Gets full trip data including all current values
    /// Example: {
    ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
    ///     "tripName": "Summer Europe Adventure 2025",
    ///     "destinationCountry": "France",
    ///     "destinationCity": "Paris",
    ///     "startDate": "2025-06-01T00:00:00Z",
    ///     "endDate": "2025-06-15T00:00:00Z",
    ///     "budget": 5000,
    ///     "currency": "USD",
    ///     "travelStyle": "Luxury",
    ///     "status": "Planning",
    ///     ...
    /// }
    /// 
    /// Step 2: DISPLAY DATA IN FORM
    /// ────────────────────────────────────────────────────────────────
    /// Frontend pre-fills the edit form with the current values from Step 1
    /// User can now see what the current values are
    /// User modifies ONLY the fields they want to change
    /// User leaves other fields as-is
    /// 
    /// Step 3: SEND ONLY CHANGED FIELDS (Partial Update)
    /// ────────────────────────────────────────────────────────────────
    /// PUT /api/trips/{tripId}
    /// Authorization: Bearer {jwt_token}
    /// Content-Type: application/json
    /// 
    /// {
    ///     "budget": 6000,
    ///     "status": "Confirmed"
    /// }
    /// 
    /// NOTE: You only send the fields you want to change!
    /// Fields you don't include (tripName, destination, etc.) are preserved as-is
    /// The API automatically keeps existing values for fields not provided
    /// 
    /// Response: 200 OK with updated trip (all fields)
    /// {
    ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
    ///     "tripName": "Summer Europe Adventure 2025",     # Unchanged
    ///     "destinationCountry": "France",                 # Unchanged
    ///     "destinationCity": "Paris",                     # Unchanged
    ///     "startDate": "2025-06-01T00:00:00Z",            # Unchanged
    ///     "endDate": "2025-06-15T00:00:00Z",              # Unchanged
    ///     "budget": 6000,                                 # UPDATED ✓
    ///     "currency": "USD",                              # Unchanged
    ///     "travelStyle": "Luxury",                        # Unchanged
    ///     "status": "Confirmed",                          # UPDATED ✓
    ///     ...
    /// }
    /// 
    /// BENEFITS:
    /// ═════════════════════════════════════════════════════════════════
    /// ✓ User sees current values (no need to remember)
    /// ✓ User doesn't have to re-enter fields they don't want to change
    /// ✓ Less data sent over the network
    /// ✓ Less chance of accidental overwrites
    /// ✓ Better UX - intuitive form editing experience
    /// ✓ Less error-prone than requiring all fields to be resent
    /// 
    /// FRONTEND EXAMPLE (React/JavaScript):
    /// ═════════════════════════════════════════════════════════════════
    /// 
    /// // On mount/edit button click, fetch current data
    /// const trip = await fetch('/api/trips/{tripId}').then(r => r.json());
    /// 
    /// // Pre-fill form with current values
    /// setFormValues({
    ///     tripName: trip.tripName,
    ///     budget: trip.budget,
    ///     status: trip.status,
    ///     ... all fields
    /// });
    /// 
    /// // On submit, send ONLY changed fields
    /// const changedFields = {};
    /// if (formValues.budget !== trip.budget) changedFields.budget = formValues.budget;
    /// if (formValues.status !== trip.status) changedFields.status = formValues.status;
    /// 
    /// // Send update with only changed fields
    /// await fetch(`/api/trips/{tripId}`, {
    ///     method: 'PUT',
    ///     headers: { 'Authorization': `Bearer ${token}`, ... },
    ///     body: JSON.stringify(changedFields)  // Only changed fields!
    /// });
    /// 
    /// This DTO supports partial updates - include only the fields you want to change!
    /// </summary>
    public class UpdateTripRequest
    {
        /// <summary>
        /// The updated name of the trip.
        /// </summary>
        public string? TripName { get; set; }

        /// <summary>
        /// The updated destination country.
        /// </summary>
        public string? DestinationCountry { get; set; }

        /// <summary>
        /// The updated destination city.
        /// </summary>
        public string? DestinationCity { get; set; }

        /// <summary>
        /// The updated start date of the trip.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The updated end date of the trip.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The updated budget for the trip.
        /// </summary>
        public decimal? Budget { get; set; }

        /// <summary>
        /// The updated currency code.
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// The updated travel style.
        /// </summary>
        public string? TravelStyle { get; set; }

        /// <summary>
        /// The updated description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The updated cover image URL.
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// The updated status (e.g., "Planning", "Confirmed", "Completed", "Cancelled").
        /// </summary>
        public string? Status { get; set; }
    }
}
