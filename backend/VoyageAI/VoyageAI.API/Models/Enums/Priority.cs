namespace VoyageAI.API.Models.Enums
{
    /// <summary>
    /// Enumeration of activity priority levels.
    /// 
    /// Priority helps users organize activities by importance.
    /// Higher priority activities should be prioritized if time/budget constraints arise.
    /// 
    /// Priorities (from lowest to highest):
    /// - Low: Nice to have, optional, skip if time/budget is tight
    /// - Medium: Should do if possible, not critical but important
    /// - High: Very important, should try to include in itinerary
    /// - MustVisit: Essential/critical, cannot miss, must be included
    /// 
    /// Usage:
    /// When creating an activity, users assign a priority level.
    /// This helps with:
    /// - Itinerary optimization when time/budget is limited
    /// - Day-by-day planning and scheduling
    /// - Identifying must-see attractions
    /// - User preferences and preferences-based recommendations
    /// 
    /// Filtering & Sorting:
    /// Users can filter their itinerary by priority to see MustVisit activities.
    /// Can sort by priority to focus on most important activities.
    /// 
    /// AI/Optimization Integration:
    /// - When optimizing itineraries, prioritize higher priority activities
    /// - If time is limited, drop Low priority activities first
    /// - Flag if any MustVisit activities would be lost due to constraints
    /// - Suggest consolidating nearby MustVisit activities
    /// 
    /// Examples:
    /// - "Quick coffee shop" -> Low (optional)
    /// - "Visit museum for 2 hours" -> Medium (should do)
    /// - "Climbing mount Kilimanjaro" -> High (very important)
    /// - "Eiffel Tower visit" -> MustVisit (cannot miss)
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Low priority: nice to have, optional, skip if time/budget is tight.
        /// Only include if there is surplus time/budget.
        /// Examples: casual shopping, optional cafe stop, extra rest time.
        /// Value: 0
        /// </summary>
        Low = 0,

        /// <summary>
        /// Medium priority: should do if possible, not critical but important.
        /// Include in itinerary under normal circumstances.
        /// Skip only if significant constraints (time, weather, cost) arise.
        /// Examples: secondary museum, optional side trip, non-essential dining.
        /// Value: 1
        /// </summary>
        Medium = 1,

        /// <summary>
        /// High priority: very important, should try to include in itinerary.
        /// Core activities that define the trip experience.
        /// Only skip under serious constraints (emergency, major cost overrun, extreme weather).
        /// Examples: famous landmark visit, key museum, main activity of the day.
        /// Value: 2
        /// </summary>
        High = 2,

        /// <summary>
        /// MustVisit (Critical): essential/critical activity, cannot miss, must be included.
        /// Defines the primary purpose of visiting this location.
        /// Should never be skipped unless impossible circumstances (closure, emergency).
        /// Priority optimization: if any MustVisit would be excluded, flag as critical issue.
        /// Examples: Eiffel Tower visit for Paris trip, Safari for African adventure trip.
        /// Value: 3
        /// </summary>
        MustVisit = 3
    }
}
