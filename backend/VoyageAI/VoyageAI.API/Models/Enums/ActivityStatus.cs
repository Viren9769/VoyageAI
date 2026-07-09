namespace VoyageAI.API.Models.Enums
{
    /// <summary>
    /// Enumeration of activity statuses.
    /// 
    /// These statuses track the lifecycle of an activity within an itinerary day.
    /// They help users understand at what stage each activity is.
    /// 
    /// Statuses:
    /// - Planned: Activity is planned but not yet booked or confirmed
    /// - Booked: Activity is confirmed/booked with a reservation
    /// - Completed: Activity has been completed/visited during the trip
    /// - Cancelled: Activity was cancelled before the trip date
    /// - Skipped: Activity was skipped on the trip date without cancellation
    /// 
    /// Usage:
    /// When creating an activity, the default status is typically "Planned".
    /// Users can update the status as they progress through their trip.
    /// 
    /// Workflow Example:
    /// 1. Create Activity -> Status = Planned
    /// 2. User books the activity -> Status = Booked
    /// 3. User visits/completes -> Status = Completed
    /// 
    /// Cancel/Skip:
    /// - User cancels before trip -> Status = Cancelled
    /// - User skips on trip day -> Status = Skipped
    /// 
    /// Reporting/Analytics:
    /// - Completed activities help track actual itinerary execution
    /// - Skipped/Cancelled help analyze planning vs. actual behavior
    /// - Used for post-trip reviews and recommendations
    /// 
    /// Future Integration:
    /// - Notifications when activities are upcoming (based on status)
    /// - Weather alerts for outdoor activities still in Planned/Booked status
    /// - Automatic status updates based on date comparison
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// Activity is planned but not yet booked or confirmed.
        /// This is typically the initial status when creating an activity.
        /// User intends to do this activity but hasn't made firm reservations.
        /// Value: 0
        /// </summary>
        Planned = 0,

        /// <summary>
        /// Activity is confirmed/booked with a reservation.
        /// User has made a binding reservation or booking.
        /// Booking reference should be populated when status is Booked.
        /// Value: 1
        /// </summary>
        Booked = 1,

        /// <summary>
        /// Activity has been completed/visited.
        /// User has already visited or experienced this activity.
        /// Typically set after the trip date has passed.
        /// ActualCost should be populated for completed activities.
        /// Value: 2
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Activity was cancelled before the trip date.
        /// User explicitly cancelled this activity.
        /// May include cancellation notes explaining why.
        /// Value: 3
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Activity was skipped on the trip date.
        /// User was planning/booked but didn't do it when the date arrived.
        /// Different from Cancelled: no explicit cancellation decision.
        /// Value: 4
        /// </summary>
        Skipped = 4
    }
}
