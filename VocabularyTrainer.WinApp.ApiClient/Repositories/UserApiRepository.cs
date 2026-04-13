using System.Net.Http.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    /// <summary>Implements <see cref="IUserRepository"/> by calling the VocabularyTrainer HTTP API.</summary>
    internal class UserApiRepository(HttpClient httpClient, IMapper mapper) : IUserRepository
    {
        /// <inheritdoc/>
        public async Task<UserModel?> GetUserAsync(string userName)
        {
            var response = await httpClient.GetAsync($"api/users/{Uri.EscapeDataString(userName)}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<CUserResponse>();
            return mapper.Map<UserModel>(user);
        }
    }
}
