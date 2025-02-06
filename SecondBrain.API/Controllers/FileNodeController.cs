using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs;
using SecondBrain.Models.DTOs.FileNode;
using SecondBrain.Services;

namespace SecondBrain.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileNodeController : ControllerBase
    {
        private readonly FileNodeService _fileNodeService;

        public FileNodeController(FileNodeService fileNodeService)
        {
            _fileNodeService = fileNodeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
