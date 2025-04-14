using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DTOs.FileSection.ChecklistSection;
using SecondBrain.Services.FileStructure;
using SecondBrain.Services.Section;

namespace SecondBrain.API.Controllers.FileSection
{
    [ApiController]
    [Route("section/checklist")]
    public class ChecklistSectionController : ControllerBase
    {
        private readonly ChecklistSectionDataService _checklistSectionService;

        public ChecklistSectionController(FileService fileService, Neo4jGraph graph, FileSectionContext fileSectionContext)
        {
            _checklistSectionService = new ChecklistSectionDataService(fileService, graph, fileSectionContext);
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
                return Ok(await _checklistSectionService.GetSectionByIdAsync(sectionId, User));
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
                return Ok(await _checklistSectionService.GetBySectionFileIdAsync(fileId, User));
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
        public async Task<IActionResult> UpdateDataAsync([FromBody] ChecklistSectionUpdateRequestDTO requestData)
        {
            try
            {
                await _checklistSectionService.UpdateDataAsync(requestData, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
