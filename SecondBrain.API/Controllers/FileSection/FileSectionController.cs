using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Services.Section;

namespace SecondBrain.API.Controllers.FileSection
{
    [ApiController]
    [Route("section/metaData")]
    public class FileSectionController : ControllerBase
    {
        private readonly FileSectionService _fileSectionService;

        public FileSectionController(FileSectionService fileSectionService)
        {
            _fileSectionService = fileSectionService;
        }

        /// <summary>
        /// Get File section meta data by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFileSectionMetaDataById([FromQuery] Guid sectionId)
        {
            try
            {
                return Ok(await _fileSectionService.GetFileSectionMetaDataById(sectionId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get File section meta data by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("byFile")]
        public async Task<IActionResult> GetFileSectionMetaDataByFileId([FromQuery] Guid fileId)
        {
            try
            {
                return Ok(await _fileSectionService.GetFileSectionMetaDataByFileId(fileId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update File section meta data
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> UpdateFileSectionMetaDataAsync([FromBody] FileSectionUpdateRequestDTO requestData)
        {
            try
            {
                await _fileSectionService.UpdateFileSectionMetaDataAsync(requestData, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
