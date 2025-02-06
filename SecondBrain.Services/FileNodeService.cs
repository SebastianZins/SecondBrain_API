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

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        public async Task<FileNodeResponseDTO> GetByIdAsync(Guid id)
        {
            FileNode node = await _fileNodeRepository.GetByIdAsync(id);

            return new FileNodeResponseDTO(node);
        }

        public async Task<List<FileNodeResponseDTO>> GetByNameAsync(string name)
        {
            List<FileNode> nodes = await _fileNodeRepository.GetByNameAsync(name);

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        public async Task<List<FileNodeResponseDTO>> GetByPathAsync(string path)
        {
            List<FileNode> nodes = await _fileNodeRepository.GetByPathAsync(path);

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        public async Task UpsertAsync(FileNodeResponseDTO file)
        {
            FileNode node = file.ToModel();

            await _fileNodeRepository.UpsertAsync(node);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _fileNodeRepository.DeleteAsync(id);
        }
    }
}
