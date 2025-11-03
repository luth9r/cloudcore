using CloudCore.Common.Models;

namespace CloudCore.Services.Interfaces
{
    /// <summary>
    /// Service for managing user account settings and profile operations
    /// </summary>
    public interface IUserService
    {
        #region Profile Management

        /// <summary>
        /// Changes user's username
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="newUsername">New username to set</param>
        /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
        /// <returns>True if username changed successfully, false if username already taken</returns>
        Task<bool> ChangeUsernameAsync(int userId, string newUsername, CancellationToken cancellationToken);

        /// <summary>
        /// Changes user's password after validating current password
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="oldPassword">Current password for verification</param>
        /// <param name="newPassword">New password to set</param>
        /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
        /// <returns>True if password changed successfully, false if current password is incorrect</returns>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken cancellationToken);

        #endregion

        #region Email Management

        /// <summary>
        /// Sends email verification link to user's new email address
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="newEmail">New email address to verify</param>
        /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
        /// <returns>True if verification email sent successfully, false if email already taken</returns>
        Task<bool> SendEmailVerificationAsync(int userId, string newEmail, CancellationToken cancellationToken);

        /// <summary>
        /// Confirms email change using verification token from email
        /// </summary>
        /// <param name="token">Email verification JWT token</param>
        /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
        /// <returns>True if email successfully changed and verified, false if token invalid or expired</returns>
        Task<bool> ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken);

        #endregion

        #region Subscription Management

        /// <summary>
        /// Upgrades user's subscription plan
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="subscriptionPlan">New subscription plan to set</param>
        /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
        /// <returns>True if plan upgraded successfully, false otherwise</returns>
        Task<bool> UpgradePlanAsync(int userId, SubscriptionPlan subscriptionPlan, CancellationToken cancellationToken);

        #endregion
    }
}
