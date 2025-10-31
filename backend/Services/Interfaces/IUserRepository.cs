using CloudCore.Domain.Entities;

namespace CloudCore.Services.Interfaces
{
    /// <summary>
    /// Repository for managing user data in the database
    /// </summary>
    public interface IUserRepository
    {
        #region Retrieval

        /// <summary>
        /// Retrieves a user by their unique ID
        /// </summary>
        /// <param name="id">User ID to search for</param>
        /// <returns>User object if found, null otherwise</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves a user by their username
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User object if found, null otherwise</returns>
        Task<User?> GetUserByNameAsync(string username);

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <returns>User object if found, null otherwise</returns>
        Task<User?> GetUserByEmailAsync(string email);

        #endregion

        #region Existence Checks

        /// <summary>
        /// Checks if a user exists with the specified username or email
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <param name="email">Email address to check</param>
        /// <returns>True if user with either username or email exists, false otherwise</returns>
        Task<bool> CheckUserExistsAsync(string username, string email);

        /// <summary>
        /// Checks if a user exists with the specified email address
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns>True if user with email exists, false otherwise</returns>
        Task<bool> CheckUserExistsAsync(string email);

        #endregion

        #region Create/Update

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        /// <param name="user">User entity to add</param>
        Task AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user in the database
        /// </summary>
        /// <param name="user">User entity with updated data</param>
        /// <returns>True if update successful, false otherwise</returns>
        Task<bool> UpdateUserAsync(User user);

        #endregion
    }
}
