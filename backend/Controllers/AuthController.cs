
using System.Security.Claims;
using CloudCore.Common.Errors;
using CloudCore.Common.Models;
using CloudCore.Contracts.Requests;
using CloudCore.Contracts.Responses;
using CloudCore.Domain.Entities;
using CloudCore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CloudCore.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
    /// <returns>JWT token and user info or Unauthorized</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Login attempt for user: {request.Username}.");
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning($"Failed login attempt for user: {request.Username}.");
            return Unauthorized(ApiResponse.Error("Invalid username or password", "INVALID_CREDENTIALS"));
        }
        _logger.LogInformation($"User {request.Username} (ID: {result.UserId}) logged in successfully.");
        return Ok(result);
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
    /// <returns>JWT token and user info or BadRequest</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Register attempt for user: {request.Username}, (Email: {request.Email}).");
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning($"Failed register attempt for user: {request.Username}, (Email: {request.Email})");
            return BadRequest(ApiResponse.Error("Username or email already exists", "USER_ALREADY_EXISTS"));
        }
        _logger.LogInformation($"User {request.Username} (ID: {result.UserId}) registered successfully.");
        return Ok(result);
    }

    /// <summary>
    /// Email verification endpoint
    /// </summary>
    /// <param name="token">JWT token from email link</param>
    /// <param name="cancellationToken">Token to cancel the operation if the client disconnects</param>
    /// <returns>Result of verification</returns>
    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromBody] TokenRequest token, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email verification attempt.");
        _logger.LogInformation($"Verification token: {token.Token}");
        var jwtToken = await _authService.ConfirmEmailAndGenerateTokenAsync(token.Token, cancellationToken);
        if (jwtToken == null)
        {
            _logger.LogWarning("Email verification failed or token invalid.");
            return BadRequest(ApiResponse.Error("Invalid or expired token", "INVALID_TOKEN"));
        }

        return Ok(new AuthResponse
        {
            Token = jwtToken
        });
    }


    /// <summary>
    /// Request password reset email
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Password reset requested for: {request.Email}");

        await _authService.SendPasswordResetEmailAsync(request.Email, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset attempt");

        var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);

        if (!result)
        {
            return BadRequest(ApiResponse.Error(
                "Invalid or expired reset token",
                ErrorCodes.INVALID_RESET_TOKEN
            ));
        }

        return Ok();
    }
}
