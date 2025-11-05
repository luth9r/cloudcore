using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudCore.Domain.Entities;
using CloudCore.Services.Interfaces;
using CloudCore.Services.Interfaces.IRepositories;
using Microsoft.IdentityModel.Tokens;

namespace CloudCore.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TokenService> _logger;
        private readonly JwtSettings _jwtSettings;
        public TokenService(IUserRepository userRepository, ILogger<TokenService> logger, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        }

        private SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
        }

        public string GenerateJwtToken(User user)
        {
            _logger.LogInformation($"Generating JWT token for user ID: {user.Id}, Username: {user.Username}.");
            var key = GetSymmetricSecurityKey();
            var creds = GetSigningCredentials();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtSettings.JwtTokenExpirationDays),
                signingCredentials: creds);

            _logger.LogInformation($"JWT token generated successfully for user ID: {user.Id}.");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateEmailVerificationToken(User user)
        {

            var key = GetSymmetricSecurityKey();
            var creds = GetSigningCredentials();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.EmailTokenExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateEmailChangeToken(User user, string newEmail)
        {
            var key = GetSymmetricSecurityKey();
            var creds = GetSigningCredentials();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("new_email", newEmail)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.EmailTokenExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> VerifyEmailTokenAsync(string token, CancellationToken cancellationToken)
        {
            var key = GetSymmetricSecurityKey();

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Issuer,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true
                }, out SecurityToken validatedToken);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return false;

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return false;

                var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
                if (user == null)
                    return false;

                user.IsEmailVerified = true;
                await _userRepository.UpdateUserAsync(user, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Email verification token validation failed.");
                return false;
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var key = GetSymmetricSecurityKey();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Issuer,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
        public int? GetUserIdFromToken(string token)
        {
            var key = GetSymmetricSecurityKey();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Issuer,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                }, out _);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    return userId;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public string GeneratePasswordResetToken(User user)
        {
            _logger.LogInformation($"Generating password reset token for user ID: {user.Id}, Email: {user.Email}");

            var key = GetSymmetricSecurityKey();
            var creds = GetSigningCredentials();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("purpose", "password_reset"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            _logger.LogInformation($"Password reset token generated successfully for user ID: {user.Id}");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<int?> VerifyPasswordResetTokenAsync(string token, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Verifying password reset token");

            var key = GetSymmetricSecurityKey();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true, // Validate token lifetime
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Check token type
                var purposeClaim = principal.FindFirst("purpose");
                if (purposeClaim?.Value != "password_reset")
                {
                    _logger.LogWarning("Token is not a password reset token");
                    return null;
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found in token");
                    return null;
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("Invalid user ID format in token");
                    return null;
                }

                var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning($"User not found for ID: {userId}");
                    return null;
                }

                _logger.LogInformation($"Password reset token verified successfully for user ID: {userId}");
                return userId;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "Password reset token has expired");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Password reset token validation failed");
                return null;
            }
        }
    }
}
