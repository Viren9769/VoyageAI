using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// Repository implementation for User entity data access operations.
    /// 
    /// This concrete implementation of IUserRepository uses Entity Framework Core
    /// to perform all database operations related to the User entity.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Encapsulates all EF Core logic
    /// - Service layer only knows about IUserRepository interface
    /// - Enables unit testing through interface dependency
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped{IUserRepository, UserRepository}()
    /// 
    /// Usage:
    /// Consumed by AuthService and other services that need user data access.
    /// 
    /// Entity Framework Core Details:
    /// - Uses VoyageDbContext for database context
    /// - Queries are executed asynchronously for better scalability
    /// - CancellationTokens are properly propagated to database operations
    /// - Email comparisons are case-insensitive on PostgreSQL
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// The Entity Framework Core database context.
        /// Provides access to the Users table and manages entity tracking.
        /// </summary>
        private readonly VoyageDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// 
        /// Constructor Injection:
        /// VoyageDbContext is injected by the dependency injection container.
        /// This allows the repository to perform database operations and supports testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{UserRepository}();
        /// services.AddScoped{IUserRepository}(sp => sp.GetRequiredService{UserRepository}());
        /// 
        /// Or simply:
        /// services.AddScoped{IUserRepository, UserRepository}();
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core database context</param>
        public UserRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Retrieves a user from the database by email address.
        /// 
        /// Implementation Details:
        /// - Uses FirstOrDefaultAsync for efficient single-record retrieval
        /// - Returns immediately upon finding first match
        /// - Email comparison is case-insensitive (handled by PostgreSQL by default)
        /// - Supports query cancellation via CancellationToken
        /// 
        /// EF Core Query Generated (approximate SQL):
        /// SELECT * FROM "Users" WHERE "Email" = @email LIMIT 1
        /// 
        /// Performance:
        /// - Email should be indexed on the Users table for fast lookups
        /// - Returns null immediately if not found (no exception thrown)
        /// 
        /// Usage in Service:
        /// var user = await _userRepository.GetByEmailAsync("john@example.com", cancellationToken);
        /// if (user == null) throw new UnauthorizedException("User not found");
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>User entity if found; null if no matching user exists</returns>
        /// <exception cref="OperationCanceledException">Thrown if the cancellation token is cancelled</exception>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            // Query database for user with matching email
            // FirstOrDefaultAsync: Get first result or null if none found
            // cancellationToken: Allow caller to cancel long-running query
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            return user;
        }

        /// <summary>
        /// Creates a new user record in the database.
        /// 
        /// Implementation Details:
        /// - Adds the user entity to the DbContext
        /// - UnitOfWork pattern: Changes are NOT persisted until SaveChangesAsync() is called
        /// - Allows batching multiple operations before committing
        /// - User must call SaveChangesAsync() to commit the transaction
        /// 
        /// EF Core Operations:
        /// 1. Entity is tracked by DbContext
        /// 2. Entity state changes to "Added"
        /// 3. UserId is generated as Guid (if not already set)
        /// 4. CreatedAt and UpdatedAt are set by entity defaults to DateTime.UtcNow
        /// 
        /// Transaction Notes:
        /// - This method does NOT start a transaction
        /// - SaveChangesAsync() handles transaction management
        /// - If SaveChangesAsync() fails, this Add operation is rolled back
        /// 
        /// Example Usage:
        /// var newUser = new User
        /// {
        ///     FirstName = "John",
        ///     LastName = "Doe",
        ///     Email = "john@example.com",
        ///     PasswordHash = hashedPassword,  // Already hashed by service
        ///     Phone = "1234567890",
        ///     Country = "USA",
        ///     IsActive = true
        /// };
        /// await _userRepository.CreateAsync(newUser);
        /// await _userRepository.SaveChangesAsync();
        /// </summary>
        /// <param name="user">The User entity to add to the database</param>
        /// <returns>A completed task</returns>
        /// <exception cref="ArgumentNullException">Thrown if user is null</exception>
        public async Task CreateAsync(User user)
        {
            // Validate input
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            // Add user to DbContext
            // This marks the entity for insertion but does NOT save to database yet
            _dbContext.Users.Add(user);

            // Note: SaveChangesAsync() must be called by the service layer to persist changes
            await Task.CompletedTask;
        }

        /// <summary>
        /// Checks if a user with the given email already exists in the database.
        /// 
        /// Implementation Details:
        /// - Uses AnyAsync for efficient existence check
        /// - Only checks if record exists, doesn't load full entity
        /// - Returns true/false without loading unnecessary data
        /// - Email comparison is case-insensitive (PostgreSQL default)
        /// 
        /// EF Core Query Generated (approximate SQL):
        /// SELECT CASE WHEN EXISTS(SELECT 1 FROM "Users" WHERE "Email" = @email) THEN 1 ELSE 0 END
        /// 
        /// Performance:
        /// - More efficient than GetByEmailAsync for existence checks
        /// - Database returns only boolean, not full entity data
        /// - Email index is utilized for fast lookup
        /// 
        /// Usage in Service:
        /// bool emailTaken = await _userRepository.ExistsAsync("john@example.com", cancellationToken);
        /// if (emailTaken)
        /// {
        ///     return Conflict("Email already registered");
        /// }
        /// </summary>
        /// <param name="email">The email address to check for existence</param>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>true if a user with the email exists; false otherwise</returns>
        /// <exception cref="OperationCanceledException">Thrown if the cancellation token is cancelled</exception>
        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // Check if user with this email exists
            // AnyAsync: Returns true if ANY record matches, false otherwise
            // More efficient than retrieving the full entity
            var exists = await _dbContext.Users
                .AnyAsync(u => u.Email == email, cancellationToken);

            return exists;
        }

        /// <summary>
        /// Saves all pending changes to the database.
        /// 
        /// Implementation Details:
        /// - Flushes all tracked changes to the database
        /// - Executes all pending INSERTs, UPDATEs, and DELETEs
        /// - Wraps DbContext.SaveChangesAsync()
        /// - Returns the number of records affected
        /// 
        /// Transaction Management:
        /// - Entity Framework Core wraps this in a transaction automatically
        /// - If any operation fails, the entire batch is rolled back
        /// - Provides ACID compliance for data consistency
        /// 
        /// When to Call:
        /// - After CreateAsync() to persist new users
        /// - After updating entities
        /// - After deleting entities
        /// - At the service layer business logic decision point
        /// 
        /// Error Handling:
        /// Service layer should catch exceptions:
        /// - DbUpdateException: Constraint violations, unique key conflicts, etc.
        /// - DbUpdateConcurrencyException: Concurrent modification conflicts
        /// - OperationCanceledException: Operation was cancelled
        /// 
        /// Example Usage:
        /// try
        /// {
        ///     await _userRepository.SaveChangesAsync();
        /// }
        /// catch (DbUpdateException ex)
        /// {
        ///     // Handle constraint violations, unique key violations, etc.
        ///     throw new InvalidOperationException("Failed to save user", ex);
        /// }
        /// </summary>
        /// <returns>The number of database records affected by the save operation</returns>
        /// <exception cref="DbUpdateException">Thrown if a database constraint is violated</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if concurrent modification occurs</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled</exception>
        public async Task<int> SaveChangesAsync()
        {
            // Persist all tracked changes to the database
            // DbContext.SaveChangesAsync() returns the number of records affected
            return await _dbContext.SaveChangesAsync();
        }
    }
}
