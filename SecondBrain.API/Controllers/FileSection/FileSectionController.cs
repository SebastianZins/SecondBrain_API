using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Services.FileStructure;
using SecondBrain.Services.Section;

namespace SecondBrain.API.Controllers.FileSection
{
    [ApiController]
    [Route("section/metaData")]
    public class FileSectionController : ControllerBase
    {
        private readonly FileSectionMetaDataService _fileSectionService;

        public FileSectionController(FileService fileService, Neo4jGraph graph, FileSectionContext fileSectionContext)
        {
            _fileSectionService = new FileSectionMetaDataService(fileService, graph, fileSectionContext);
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
        /// Create File section
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> CreateSectionAsync([FromBody] FileSectionCreateRequestDTO requestData)
        {
            try
            {
                return Ok(await _fileSectionService.CreateSectionAsync(requestData, User));
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
        public async Task<IActionResult> UpdateFileSectionMetaDataAsync([FromBody] FileSectionMetaDataUpdateRequestDTO requestData)
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

        /// <summary>
        /// Delete File section
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteFileSectionMetaDataAsync([FromQuery] Guid sectionId)
        {
            try
            {
                await _fileSectionService.DeleteFileSectionMetaDataAsync(sectionId, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
