using Neo4j.Driver;
using SecondBrain.Core.Enums;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class FileStructureRepository : Neo4jRepository
    {
        public FileStructureRepository(Neo4jGraph graph) : base(graph) { }

        /// <summary>
        /// Get file structure item or root if no id provided
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode?> GetFileStructureItemAsync(Guid userId, Guid? id = null)
        {
            var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId);

            if (id == null)
            {
                query = query.Match("(item:RootFolder)-[:CreatedBy|UpdatedBy]->(user)");
            }
            else
            {
                query = query
                    .Match("(item:FileStructure)-[:CreatedBy|UpdatedBy]->(user)")
                    .Where((FileStructureNode item) => item.id == id);
            }

            var resultQuery = query
                .ReturnDistinct(item => item.As<FileStructureNode>());

            var result = await resultQuery.ResultsAsync;
            return result.Count() >= 1 ? result.Single() : null;
        }

        public async Task<int> GetChildCountAsync(Guid itemId, Guid userId)
        {
            var query = _graph.Cypher
                   .Match("(item:FileStructure)-[]->(user:User)")
                   .Where((UserNode user) => user.id == userId)
                   .AndWhere((FileStructureNode item) => item.id == itemId)
                   .Match("(item)-[:ParentFolderOf]->(sibling:FileStructure)")
                   .Where((FileStructureNode sibling) => sibling.id != itemId)
                   .ReturnDistinct(sibling => sibling.As<FileStructureNode>().id);

            return (await query.ResultsAsync).Count();
        }

        /// <summary>
        /// Create a root folder
        /// </summary>
        /// <param name="label"></param>
        /// <param name="type"></param>
        /// <param name="treeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode> CreateFileStructureRootAsync(string label, EFileType type, int treeId, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .Merge($"(item:RootFolder:FileStructure{{id:'{Guid.NewGuid()}'}})")
                    .OnCreate()
                    .Set($"item.type = {((int)type)}")
                    .Set($"item.label = '{label}'")
                    .Set($"item.treeId = '{treeId}'")
                    .Merge("(item)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(item)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParam("now", now)
                    .ReturnDistinct(item => item.As<FileStructureNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Create a new file structure item
        /// </summary>
        /// <param name="label"></param>
        /// <param name="type"></param>
        /// <param name="treeId"></param>
        /// <param name="userId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode> CreateFileStructureItemAsync(string label, EFileType type, int treeId, Guid parentId, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .Match("(parent:FileStructure)-[]->(user)")
                    .Where((FileStructureNode parent) => parent.id == parentId)
                    .Merge($"(item:FileStructure{{id:'{Guid.NewGuid()}'}})")
                    .OnCreate()
                    .Set($"item.type = {(int)type}")
                    .Set($"item.label = '{label}'")
                    .Set($"item.treeId = '{treeId}'")
                    .Merge("(item)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(item)-[:UpdatedBy {updated:$now}]->(user)")
                    .Merge("(parent)-[:ParentFolderOf]->(item)")
                    .WithParam("now", now)
                    .ReturnDistinct(item => item.As<FileStructureNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Get all first level children of a folder
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<FileStructureNode>> GetChildFileStructureItemsAsync(Guid parentFolderId, Guid userId)
        {
            var query = _graph.Cypher
                     .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(parent:FileStructure)")
                     .Where((UserNode user) => user.id == userId)
                     .AndWhere((FileStructureNode parent) => parent.id == parentFolderId)
                     .Match("(parent)-[:ParentFolderOf]->(child:FileStructure)")
                     .ReturnDistinct(child => child.As<FileStructureNode>());

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Update the tree id of an item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="treeId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateItemTreeIdAsync(Guid id, int treeId, Guid userId)
        {
            var query = _graph.Cypher
                   .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(item:FileStructure)")
                   .Where((UserNode user) => user.id == userId)
                   .AndWhere((FileStructureNode item) => item.id == id)
                   .Set($"item.treeId = {treeId}");

            await query.ExecuteWithoutResultsAsync();
        }


        /// <summary>
        /// Get all file structure items for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<FileStructureNode>> GetFileStructuresAsync(Guid userId)
        {
            var query = _graph.Cypher
                    .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(item:FileStructure)")
                    .Where((UserNode user) => user.id == userId)
                    .ReturnDistinct(item => item.As<FileStructureNode>());

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Get connections between file structure items of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<Tuple<Guid, Guid>>> GetFileStructureConnectionsAsync(Guid userId)
        {
            var query = _graph.Cypher
                     .Match("(user:User)")
                     .Where((UserNode user) => user.id == userId)
                     .Match("(user)<-[:CreatedBy|UpdatedBy]-(folder:FileStructure)-[:ParentFolderOf]->(child:FileStructure)-[:CreatedBy|UpdatedBy]->(user)")
                     .ReturnDistinct((folder, child) => new Tuple<Guid, Guid>(
                             folder.As<FileStructureNode>().id,
                             child.As<FileStructureNode>().id
                         ));

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Update file structure data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateFileStructureItemDataAsync(Guid id, string label, EFileType type, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = _graph.Cypher
                    .Match("(item:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode item) => item.id == id)
                    .Set($"item.label = {label}")
                    .Set($"item.type = {type}")
                    .Merge("(item)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new { now });

            await query.ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Delete a folder and all child folder and attached files of it and its children
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteFileStructureItemAsync(Guid folderId, Guid userId)
        {
            var query = _graph.Cypher
                     .Match("(folder:FileStructure)-[:CreatedBy|UpdatedBy]->(user:User)")
                     .Where((UserNode user) => user.id == userId)
                     .AndWhere((FileStructureNode folder) => folder.id == folderId)
                     .OptionalMatch("(folder:FileStructure)-[]->(file:File|Image|DataFile)")
                     .OptionalMatch("(folder)-[:ParentFolderOf*]->(childFolder:FileStructure)")
                     .OptionalMatch("(childFolder:FileStructure)-[]->(file:File)-[:Contains]->(section:FileSection)-[]-(attachement:Attachement)")
                     .DetachDelete("attachement")
                     .DetachDelete("section")
                     .DetachDelete("file")
                     .DetachDelete("childFolder")
                     .DetachDelete("folder");

            await query.ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Get parent folder
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileStructureNode> GetParentFolderAsync(Guid itemId, Guid userId)
        {
            var query = _graph.Cypher
                     .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(folder:FileStructure)<-[:ParentFolderOf]-(parent:FileStructure)")
                     .Where((UserNode user) => user.id == userId)
                     .AndWhere((FileStructureNode folder) => folder.id == itemId)
                     .ReturnDistinct(parent => parent.As<FileStructureNode>());

            return (await query.ResultsAsync).Single();
        }

        public async Task MoveFielStructureItemAsync(Guid id, Guid parentId, Guid userId)
        {
            var query = _graph.Cypher
                    .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(item:FileStructure)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileStructureNode item) => item.id == id)
                    .Match("(parent:FileStructure)-[rel:ParentFolderOf]->(item)")
                    .DetachDelete("rel")
                    .With("item")
                    .Match("(newParent:FileStructure)")
                    .Where((FileStructureNode newParent) => newParent.id == parentId)
                    .Merge("(newParent)-[:ParentFolderOf]->(item)");

            await query.ExecuteWithoutResultsAsync();
        }
    }
}
