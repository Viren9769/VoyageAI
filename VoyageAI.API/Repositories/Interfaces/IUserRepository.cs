using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for User entity data access operations.
    /// 
    /// This interface abstracts all database operations related to the User entity.
    /// The concrete implementation (UserRepository) uses Entity Framework Core,
    /// but the service layer depends only on this interface.
    /// 
    /// Design Pattern: Repository Pattern
    /// This pattern provides the following benefits:
    /// - Abstraction of data access logic
    /// - Testability (can mock in unit tests)
    /// - Decoupling from EF Core implementation
    /// - Single Responsibility Principle (all DB logic in one place)
    /// - Dependency Inversion (depend on abstractions, not concrete classes)
    /// 
    /// Usage in Services:
    /// The AuthService injects this interface and calls its methods without
    /// knowing or caring about the underlying database technology (EF Core, SQL, etc.).
    /// 
    /// Example:
    /// public class AuthService
    /// {
    ///     private readonly IUserRepository _userRepository;
    ///     
    ///     public async Task RegisterAsync(RegisterRequest request)
    ///     {
    ///         var user = new User { FirstName = request.FirstName, ... };
    ///         await _userRepository.CreateAsync(user);
    ///         await _userRepository.SaveChangesAsync();
    ///     }
    /// }
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user from the database by email address.
        /// 
        /// Purpose:
        /// - Used during login to find the user attempting to authenticate
        /// - Used during registration to check if email already exists
        /// 
        /// Parameters:
        /// - email: The email address to search for (case-insensitive)
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - User object if found
        /// - null if user doesn't exist
        /// 
        /// Database Query:
        /// SELECT * FROM "Users" WHERE "Email" = @email (case-insensitive comparison)
        /// 
        /// Example:
        /// var user = await _userRepository.GetByEmailAsync("john@example.com", cancellationToken);
        /// if (user == null)
        /// {
        ///     throw new Exception("User not found");
        /// }
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>User entity if found; null if not found</returns>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user in the database.
        /// 
        /// Purpose:
        /// - Inserts a new user record during registration
        /// - Does NOT automatically save changes; SaveChangesAsync() must be called
        /// 
        /// Parameters:
        /// - user: The User entity to add
        /// 
        /// Details:
        /// - This method adds the user to the DbContext but does NOT persist to database
        /// - Caller must call SaveChangesAsync() to commit the transaction
        /// - The user's UserId is auto-generated (Guid)
        /// - CreatedAt and UpdatedAt are set to UTC now by the entity
        /// 
        /// Example:
        /// var user = new User
        /// {
        ///     FirstName = "John",
        ///     LastName = "Doe",
        ///     Email = "john@example.com",
        ///     PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
        ///     IsActive = true
        /// };
        /// await _userRepository.CreateAsync(user);
        /// await _userRepository.SaveChangesAsync();
        /// </summary>
        /// <param name="user">The User entity to add to the database</param>
        /// <returns>A completed task</returns>
        Task CreateAsync(User user);

        /// <summary>
        /// Checks if a user with the given email already exists in the database.
        /// 
        /// Purpose:
        /// - Prevent duplicate email registrations (emails must be unique)
        /// - Called during registration validation
        /// - Used to return 409 Conflict if email already taken
        /// 
        /// Parameters:
        /// - email: The email address to check (case-insensitive)
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - true if a user with this email exists
        /// - false if no user exists with this email
        /// 
        /// Database Query:
        /// SELECT COUNT(1) FROM "Users" WHERE "Email" = @email
        /// 
        /// Example:
        /// var emailExists = await _userRepository.ExistsAsync("john@example.com", cancellationToken);
        /// if (emailExists)
        /// {
        ///     return Conflict("Email already registered");
        /// }
        /// </summary>
        /// <param name="email">The email address to check for existence</param>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>true if user exists; false otherwise</returns>
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all pending changes to the database.
        /// 
        /// Purpose:
        /// - Commits all DbContext changes to the database
        /// - Executes INSERT, UPDATE, DELETE operations
        /// - Called after CreateAsync, UpdateAsync, or DeleteAsync operations
        /// 
        /// Details:
        /// - This uses Entity Framework Core's SaveChangesAsync()
        /// - Changes are tracked by the DbContext before this is called
        /// - If an exception occurs, no changes are persisted (transaction safety)
        /// - Should be called at the service layer decision point
        /// 
        /// Example:
        /// var user = new User { ... };
        /// await _userRepository.CreateAsync(user);
        /// try
        /// {
        ///     await _userRepository.SaveChangesAsync();
        /// }
        /// catch (DbUpdateException ex)
        /// {
        ///     throw new Exception("Failed to register user", ex);
        /// }
        /// </summary>
        /// <returns>The number of records affected</returns>
        Task<int> SaveChangesAsync();
    }
}
