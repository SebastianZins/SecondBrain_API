using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DTOs.FileSection.TextSection;
using SecondBrain.Services.FileStructure;
using SecondBrain.Services.Section;

namespace SecondBrain.API.Controllers.FileSection
{
    [ApiController]
    [Route("section/text")]
    public class TextSectionController : ControllerBase
    {
        private readonly TextSectionDataService _textSectionService;

        public TextSectionController(FileService fileService, Neo4jGraph graph, FileSectionContext fileSectionContext)
        {
            _textSectionService = new TextSectionDataService(fileService, graph, fileSectionContext);
        }

        /// <summary>
        /// Get File section by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSectionByIdAsync([FromQuery] Guid sectionId)
        {
            try
            {
                return Ok(await _textSectionService.GetSectionByIdAsync(sectionId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get File section by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("byFile")]
        public async Task<IActionResult> GetBySectionFileIdAsync([FromQuery] Guid fileId)
        {
            try
            {
                return Ok(await _textSectionService.GetBySectionFileIdAsync(fileId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update File section data
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> UpdateDataAsync([FromBody] TextSectionUpdateRequestDTO requestData)
        {
            try
            {
                await _textSectionService.UpdateDataAsync(requestData, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
