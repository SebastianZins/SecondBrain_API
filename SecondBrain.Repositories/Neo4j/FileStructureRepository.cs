using Neo4j.Driver;
using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class FileStructureRepository
    {
        private readonly IGraphClient _graph;

        public FileStructureRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }

        /// <summary>
        /// Get root folder
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode?> GetRootFolderAsync(Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(root:RootFolder)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .ReturnDistinct(root => root.As<FileStructureNode>());

                var result = await query.ResultsAsync;
                return result.Count() >= 1 ? result.Single() : null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Root folder not found.");
            }
        }

        /// <summary>
        /// Get Folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode> GetFolderAsync(Guid folderId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode folder) => folder.id == folderId)
                    .ReturnDistinct(folder => folder.As<FileStructureNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Folder not found.");
            }
        }

        /// <summary>
        /// Create root folder
        /// </summary>
        /// <param name="root"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode> CreateRootFolderAsync(FileStructureNode root, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .Merge("(root:RootFolder:FileStructure{id:$id})")
                    .OnCreate()
                    .Set("root = $root")
                    .With("user, root")
                    .Merge("(root)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(root)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new { root.id, root, now })
                    .Return(root => root.As<FileStructureNode>());

               return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Create root folder.");
            }
        }

        /// <summary>
        /// Get complete file structure
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<Tuple<FileStructureNode, FileStructureNode>>> GetFileStructureAsync(Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(folder:FileStructure)-[:ParentFolderOf]->(child:FileStructure)")
                    .Where((UserNode user) => user.id == userId)
                    .Return((folder, child) => new Tuple<FileStructureNode, FileStructureNode>(
                            folder.As<FileStructureNode>(),
                            child.As<FileStructureNode>()
                        ));

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Loading file structure failed.");
            }
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="newFolder"></param>
        /// <param name="parentFolderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task CreateFolderAsync(FileStructureNode newFolder, Guid? parentFolderId, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId);

                // add to root folder
                if (parentFolderId == null)
                {
                    query = query
                        .Match("(folder:RootFolder)-[:CreatedBy|UpdatedBy]->(user:User)");
                } else
                {
                    query = query
                        .Match("(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                        .Where((FileStructureNode folder) => folder.id == parentFolderId);
                }

                query = query
                    .Merge("(newFolder:FileStructure{id:$id})")
                    .OnCreate()
                    .Set("newFolder = $newFolder")
                    .With("user, newFolder, folder")
                    .Merge("(newFolder)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(newFolder)-[:UpdatedBy {updated:$now}]->(user)")
                    .Merge("(folder)-[:ParentFolderOf]->(newFolder)")
                    .WithParams(new { newFolder.id, newFolder, now });

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Creating new folder failed.");
            }
        }

        /// <summary>
        /// Update folder data
        /// </summary>
        /// <param name="newFolder"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateFolderAsync(FileStructureNode newFolder, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try
            {
                var query = _graph.Cypher
                    .Match("(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode folder) => folder.id == newFolder.id)
                    .Set("folder = $newFolder")
                    .Merge("(folder)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new { newFolder, now });

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Updating folder failed.");
            }
        }
        
        /// <summary>
        /// Delete a folder and all child folder and attached files of it and its children
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteFolderAsync(Guid folderId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode folder) => folder.id == folderId)
                    .Match("(folder)-[:ParentFolderOf*]->(childFolder:FileStructure)-[]->(file:File|Image|DataFile)")
                    .DetachDelete("file")
                    .DetachDelete("childFolder")
                    .DetachDelete("folder");

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Deleting folder failed.");
            }
        }

        /// <summary>
        /// Move folder under a different parent folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="newParentFolderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task MoveFolderAsync(Guid folderId, Guid newParentFolderId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(parentFolder:FileStructure)-[rel:ParentFolderOf]->(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode folder) => folder.id == folderId)
                    .Match("(newParentFolder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((FileStructureNode newParentFolder) => newParentFolder.id == newParentFolderId)
                    .Delete("rel")
                    .With("folder, newParentFolder")
                    .Merge("(newParentFolder)-[:ParentFolderOf]->(folder)");

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e);
                throw new Neo4jException("Error:", "Moving folder failed.");
            }
        }
    }
}
