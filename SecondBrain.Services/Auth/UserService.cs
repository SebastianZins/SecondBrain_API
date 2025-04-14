using SecondBrain.Core;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.User;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.Auth
{
    public class UserService : Service
    {
        private readonly UserRepository _userRepository;

        public UserService(Neo4jGraph graph) : base(Constants.ERROR_USER)
        {
            _userRepository = new UserRepository(graph);
        }

        /// <summary>
        /// Get Current User
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<UserResponseDTO> GetCurrentUserAsync(ClaimsPrincipal claims)
        {
            try
            {
                Guid currentUserId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
                UserNode user = await _userRepository.GetByIdAsync(currentUserId);
                return new UserResponseDTO(user);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserResponseDTO> GetByIdAsync(Guid id)
        {
            try
            {
                UserNode user = await _userRepository.GetByIdAsync(id);
                return new UserResponseDTO(user);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Get User by mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<UserNode> GetUserNodeMailAsync(string mail)
        {
            try
            {
                return await _userRepository.GetByMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(UserCreateRequestDTO user)
        {
            try
            {
                await _userRepository.GetByMailAsync(user.Email);
                return false;
            }
            catch
            {
                try
                {
                    UserNode node = user.ToModel();
                    await _userRepository.CreateAsync(node);
                    return true;
                }
                catch (Exception ex)
                {
                    throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_CREATE_FAILED);
                }
            }
        }

        /// <summary>
        /// Update User iff it concerns the currently logged in user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(UserUpdateRequestDTO user, ClaimsPrincipal claims)
        {
            try
            {
                Guid currentUserId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
                if (currentUserId != user.Id)
                {
                    return false;
                }

                UserNode node = await _userRepository.GetByIdAsync(currentUserId);
                await _userRepository.UpdateAsync(user.WriteToModel(node));
                return false;
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_UPDATE_FAILED);
            }
        }

        /// <summary>
        /// Delete current user account
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task DeleteCurrentUserAsync(ClaimsPrincipal claims)
        {
            try
            {
                Guid currentUserId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
                await _userRepository.DeleteAsync(currentUserId);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_DELETE_FAILED);
            }
        }
    }
}
