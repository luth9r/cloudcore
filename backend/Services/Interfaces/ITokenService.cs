using CloudCore.Domain.Entities;
using System.Security.Claims;

namespace CloudCore.Services.Interfaces
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface ITokenService
    {
        #region JWT Token Generation

        /// <summary>
        /// Generates a JWT access token for authenticated user
        /// </summary>
        /// <param name="user">User for whom to create the token</param>
        /// <returns>JWT token string containing user claims (ID, username, email)</returns>
        string GenerateJwtToken(User user);

        #endregion

        #region Email Verification Tokens

        /// <summary>
        /// Generates an email verification token for new user registration
        /// </summary>
        /// <param name="user">User who needs email verification</param>
        /// <returns>JWT token with short expiration (typically 10 minutes)</returns>
        string GenerateEmailVerificationToken(User user);

        /// <summary>
        /// Generates a token for email address change confirmation
        /// </summary>
        /// <param name="user">User requesting email change</param>
        /// <param name="newEmail">New email address to verify</param>
        /// <returns>JWT token containing new email claim</returns>
        string GenerateEmailChangeToken(User user, string newEmail);

        /// <summary>
        /// Verifies email verification token and marks user email as verified
        /// </summary>
        /// <param name="token">Email verification JWT token</param>
        /// <returns>True if token is valid and email is successfully verified, false otherwise</returns>
        Task<bool> VerifyEmailTokenAsync(string token);

        #endregion

        #region Password Reset Tokens

        /// <summary>
        /// Generates a password reset token for user
        /// </summary>
        /// <param name="user">User requesting password reset</param>
        /// <returns>JWT token with 1 hour expiration containing password reset purpose claim</returns>
        string GeneratePasswordResetToken(User user);

        /// <summary>
        /// Verifies password reset token and extracts user ID
        /// </summary>
        /// <param name="token">Password reset JWT token</param>
        /// <returns>User ID if token is valid and not expired, null otherwise</returns>
        Task<int?> VerifyPasswordResetTokenAsync(string token);

        #endregion

        #region Token Validation

        /// <summary>
        /// Validates JWT token signature and expiration
        /// </summary>
        /// <param name="token">JWT token string to validate</param>
        /// <returns>ClaimsPrincipal containing token claims if valid, null if invalid or expired</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Extracts user ID from JWT token
        /// </summary>
        /// <param name="token">JWT token string</param>
        /// <returns>User ID if token is valid and contains NameIdentifier claim, null otherwise</returns>
        int? GetUserIdFromToken(string token);

        #endregion
    }
}
