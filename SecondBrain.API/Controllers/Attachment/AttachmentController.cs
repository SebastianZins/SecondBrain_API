using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.Auth;
using SecondBrain.Services.Auth;

namespace SecondBrain.API.Controllers.Attachment
{
    [ApiController]
    [Route("[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly AuthService _authService;

        public AttachmentController(AuthService authService, UserService userService)
        {
            _authService = authService;
        }

    }
}
