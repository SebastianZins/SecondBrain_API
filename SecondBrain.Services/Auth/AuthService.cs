
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.Auth;
using SecondBrain.Models.DTOs.User;
using SecondBrain.Models.InternalModels;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SecondBrain.Services.Auth
{
    public class AuthService
    {
        private JwtSettings settings;
        private readonly UserRepository _userRepository;

        public AuthService(IOptions<JwtSettings> settings, Neo4jGraph graph)
        {
            _userRepository = new UserRepository(graph);
            this.settings = settings.Value;
        }

        /// <summary>
        /// Login to user account
        /// Check if user exist and given password is valid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(LoginRequestDTO request)
        {
            UserNode user;
            try
            {
                user = await _userRepository.GetByMailAsync(request.Email);
            }
            catch (Exception e)
            {
                return false;
            }


            if (!PasswordCryptHelper.VerifyHashString(user.password, user.passwordSalt, request.Password))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Signup as new User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> SignupAsync(UserCreateRequestDTO request)
        {
            try
            {
                UserNode user = await _userRepository.GetByMailAsync(request.Email);
            }
            catch
            {
                UserNode node = request.ToModel();
                (string, string) generatedPW = PasswordCryptHelper.GenerateHashString(request.password);
                node.password = generatedPW.Item1;
                node.passwordSalt = generatedPW.Item2;
                await _userRepository.CreateAsync(node);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Refresh the jwt token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<RefreshTokenRequestDTO> RefreshTokenAsync(string token, string refreshToken)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token);
            Guid id = ClaimsPrincipalHelper.GetCurrentUserId(principal);
            UserNode user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.refreshToken != refreshToken)
            {
                Console.WriteLine("Error: Could not validate refresh token");
                throw new Exception("Error: Could not validate refresh token");
            }

            string newToken = GenerateAccessToken(principal.Claims);
            string newRefreshToken = GenerateRefreshToken();

            user.refreshToken = newRefreshToken;

            await _userRepository.UpdateAsync(user);

            return new RefreshTokenRequestDTO()
            {
                token = newToken,
                refreshToken = newRefreshToken
            };

        }

        /// <summary>
        /// Revoke the token
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RevokeTokenAsync(ClaimsPrincipal principal)
        {
            Guid id = ClaimsPrincipalHelper.GetCurrentUserId(principal);
            UserNode user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                Console.WriteLine("Error: Could not revoke token");
                throw new Exception("Error: Could not revoke token");
            }

            user.refreshToken = null;

            await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Generate new access token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var token = new JwtSecurityToken(
              issuer: settings.Issuer,
              audience: settings.Audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenDurationInMinutes),
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// Generate new refresh token
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var number = new byte[32];

            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(number);

                return Convert.ToBase64String(number);
            }

        }

        /// <summary>
        /// Get principals from expired token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false,
            };

            var handler = new JwtSecurityTokenHandler();

            SecurityToken securityToken;

            var principal = handler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;

        }

    }
}
