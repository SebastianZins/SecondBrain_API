using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.Auth;
using SecondBrain.Models.DTOs.User;
using SecondBrain.Services;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO request)
        {        
            UserNode user;
            try
            {
                user = await _userService.GetUserNodeMailAsync(request.Email);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }


            if (!PasswordCryptHelper.VerifyHashString(user.password, user.passwordSalt, request.Password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.GivenName, user.firstName),
                new Claim(ClaimTypes.Surname, user.lastName)
            };

            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(5),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignupAsync(UserCreateRequestDTO request)
        {
            try
            {
                bool success = await _authService.SignupAsync(request);
                if (success) { return Ok(); } else {  return BadRequest(); }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error", e);
                return StatusCode(500);
            }
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        public async Task<IActionResult> RefreshAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                RefreshTokenRequestDTO result = await _authService.RefreshTokenAsync(request.token, request.refreshToken);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("token/revoke")]
        public async Task<IActionResult> RevokeAsync()
        {
            try
            {
                await _authService.RevokeTokenAsync(User);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                return StatusCode(500);
            }
        }

    }
}
