using CloudCore.Contracts.Requests;
using CloudCore.Contracts.Responses;
using CloudCore.Domain.Entities;
using CloudCore.Services.Interfaces;
using CloudCore.Services.Interfaces.IRepositories;

namespace CloudCore.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IEmailSendService _emailSendService;


    public AuthService(IUserRepository userRepository, IEmailSendService emailSendService, ILogger<AuthService> logger, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _emailSendService = emailSendService;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByNameAsync(request.Username, cancellationToken);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(request.Password, user.PasswordHash) || user.IsEmailVerified == false)
            return null;

        var token = _tokenService.GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await _userRepository.CheckUserExistsAsync(request.Username, request.Email, cancellationToken))
            return null;

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            IsEmailVerified = false
        };

        await _userRepository.AddUserAsync(user, cancellationToken);

        try
        {
            var emailToken = _tokenService.GenerateEmailVerificationToken(user);
            var verifyUrl = $"https://localhost:3443/verify-email.html?token={emailToken}";

            await _emailSendService.SendEmailVerificationAsync(
                user.Email,
                verifyUrl,
                "Welcome to CloudCore - Verify your email",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email");
        }

        return new AuthResponse
        {
            Token = null,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    public string HashPassword(string password)
    {
        //return BCrypt.Net.BCrypt.HashPassword(password);
        return password;
    }

    public bool VerifyPassword(string password, string storedPassword) //FIXME transfer to validation service
    {
        //return BCrypt.Net.BCrypt.Verify(password, storedPassword);
        return password == storedPassword;
    }

    public async Task<string?> ConfirmEmailAndGenerateTokenAsync(string token, CancellationToken cancellationToken)
    {
        var isValid = await _tokenService.VerifyEmailTokenAsync(token, cancellationToken);
        if (!isValid)
            return null;

        var userId = _tokenService.GetUserIdFromToken(token);
        if (userId == null)
            return null;

        var user = await _userRepository.GetUserByIdAsync(userId.Value, cancellationToken);
        if (user == null)
            return null;

        user.IsEmailVerified = true;
        await _userRepository.UpdateUserAsync(user, cancellationToken);

        return _tokenService.GenerateJwtToken(user);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Password reset requested for email: {email}");

        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning($"Password reset requested for non-existent email: {email}");
            return true;
        }
        var resetToken = _tokenService.GeneratePasswordResetToken(user);

        try
        {
            var resetUrl = $"https://localhost:3443/reset-password.html?token={resetToken}";

            await _emailSendService.SendPasswordResetAsync(
                user.Email,
                resetUrl,
                "CloudCore - Password Reset Request",
                cancellationToken
            );

            _logger.LogInformation($"Password reset email sent to: {email}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send password reset email to: {email}");
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting password reset");

        var userId = await _tokenService.VerifyPasswordResetTokenAsync(token, cancellationToken);

        if (userId == null)
        {
            _logger.LogWarning("Invalid or expired password reset token");
            return false;
        }

        var user = await _userRepository.GetUserByIdAsync(userId.Value, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning($"User not found for ID: {userId}");
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);

        await _userRepository.UpdateUserAsync(user, cancellationToken);

        _logger.LogInformation($"Password reset successful for user: {user.Username}");
        return true;
    }
}
