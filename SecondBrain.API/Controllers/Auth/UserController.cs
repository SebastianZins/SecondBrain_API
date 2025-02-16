using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.User;
using SecondBrain.Services.Auth;

namespace SecondBrain.API.Controllers.Auth
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create new user account
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> CreateAsync(UserCreateRequestDTO request)
        {
            try
            {
                bool success = await _userService.CreateAsync(request);
                if (success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get current user data 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserAsync()
        {
            try
            {
                UserResponseDTO result = await _userService.GetCurrentUserAsync(User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get current user data 
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateCurrentUserAsync(UserUpdateRequestDTO request)
        {
            try
            {
                bool success = await _userService.UpdateAsync(request, User);
                if (success)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get current user data 
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteCurrentUserAsync()
        {
            try
            {
                await _userService.DeleteCurrentUserAsync(User);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
