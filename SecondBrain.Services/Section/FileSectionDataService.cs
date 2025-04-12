using SecondBrain.Repositories.Neo4j;

namespace SecondBrain.Services.Section
{
    public class FileSectionDataService
    {
        private readonly TagRepository _tagRepository;


        public FileSectionDataService(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
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
    }
}
