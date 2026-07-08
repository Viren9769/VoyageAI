namespace VoyageAI.API.Common.Models
{
    /// <summary>
    /// Standard paginated API response.
    /// Used for endpoints returning lists of items.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    public class PagedResponse<T> : ApiResponse<IEnumerable<T>>
    {
        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Creates a successful paginated response.
        /// </summary>
        public static PagedResponse<T> SuccessResponse(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            string? message = null)
        {
            return new PagedResponse<T>
            {
                Success = true,
                Message = message ?? "Operation successful",
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Errors = null
            };
        }
    }
}
