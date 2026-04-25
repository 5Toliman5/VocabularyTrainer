using System.Net.Http.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    internal class UserApiRepository(HttpClient httpClient, IMapper mapper) : IUserRepository
    {
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
