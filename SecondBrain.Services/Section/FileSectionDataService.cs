using SecondBrain.Core;
using SecondBrain.Database.Neo4j;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;

namespace SecondBrain.Services.Section
{
    public class FileSectionDataService : Service
    {
        private readonly TagRepository _tagRepository;


        public FileSectionDataService(Neo4jGraph graph) : base(Constants.ERROR_SECTION_FILE_SECTION_DATA)
        {
            _tagRepository = new TagRepository(graph);
        }

        /// <summary>
        /// Update tags of file section
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="tags"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected async Task UpdateTagsAsync(Guid sectionId, List<string> tags, Guid userId)
        {
            try
            {
                List<string> existingTags = await _tagRepository.GetTagsOfFileSectionAsync(sectionId, userId);

                List<string> notExistingNodes = tags.Where(t => !existingTags.Contains(t)).ToList();
                List<string> removeConnections = existingTags.Where(t => !tags.Contains(t)).ToList();

                // remove connections 
                if (removeConnections.Count > 0)
                {
                    await _tagRepository.RemoveFileSectionTagsAsync(removeConnections, sectionId, userId);
                }

                // add connections (create tags if nessecary)
                if (notExistingNodes.Count > 0)
                {
                    await _tagRepository.UpdateFileSectionTagsAsync(notExistingNodes, sectionId, userId);
                }

                // remove tags without connections
                if (removeConnections.Count > 0)
                {
                    await _tagRepository.DeleteOrphanTagsAsync(removeConnections);
                }
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_UPDATE_FAILED);
            }
        }
    }
}
