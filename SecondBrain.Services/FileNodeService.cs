using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileNode;
using SecondBrain.Repositories.Neo4j;

namespace SecondBrain.Services
{
    public class FileNodeService
    {
        private readonly FileNodeRepository _fileNodeRepository;

        public FileNodeService(FileNodeRepository fileNodeRepository)
        {
            _fileNodeRepository = fileNodeRepository;
        }

        public async Task<List<FileNodeResponseDTO>> GetAllAsync()
        {
            List<FileNode> nodes = await _fileNodeRepository.GetAllAsync();

            return nodes.Select(node =>
            {
                FileNodeResponseDTO response = new FileNodeResponseDTO();
                return response.FromModel(node);
            }).ToList();
        }
    }
}
