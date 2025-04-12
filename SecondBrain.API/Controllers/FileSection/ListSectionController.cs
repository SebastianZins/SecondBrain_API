using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.FileSection.ListSelection;
using SecondBrain.Services.Section;

namespace SecondBrain.API.Controllers.FileSection
{
    [ApiController]
    [Route("section/list")]
    public class ListSectionController : ControllerBase
    {
        private readonly ListSectionDataService _listSectionService;

        public ListSectionController(ListSectionDataService listSectionService)
        {
            _listSectionService = listSectionService;
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
                return Ok(await _listSectionService.GetSectionByIdAsync(sectionId, User));
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
                return Ok(await _listSectionService.GetBySectionFileIdAsync(fileId, User));
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
        public async Task<IActionResult> UpdateDataAsync([FromBody] ListSectionUpdateRequestDTO requestData)
        {
            try
            {
                await _listSectionService.UpdateDataAsync(requestData, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
