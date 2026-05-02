using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    internal class UserApiRepository(HttpClient httpClient, IMapper mapper) : IUserRepository
    {
        public async Task<Result<UserModel>> GetUserAsync(string userName)
        {
            var response = await httpClient.GetAsync($"api/users/{Uri.EscapeDataString(userName)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result<UserModel>.Failure($"User '{userName}' was not found.");

            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<CUserResponse>();
            return Result<UserModel>.Success(mapper.Map<UserModel>(user!));
        }
    }
}
