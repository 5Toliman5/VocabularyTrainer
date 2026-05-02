using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CDictionaryResponse = VocabularyTrainer.Api.Contract.Dictionaries.DictionaryResponse;
using CAddDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.AddDictionaryRequest;
using CUpdateDictionaryRequest = VocabularyTrainer.Api.Contract.Dictionaries.UpdateDictionaryRequest;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    internal class DictionaryApiRepository(HttpClient httpClient, IMapper mapper) : IDictionaryRepository
    {
        public async Task<List<DictionaryDto>> GetAllAsync(int userId)
        {
            var response = await httpClient.GetFromJsonAsync<List<CDictionaryResponse>>($"api/dictionaries?userId={userId}");
            return mapper.Map<List<DictionaryDto>>(response);
        }

        public async Task<Result<int>> AddAsync(AddDictionaryRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("api/dictionaries", mapper.Map<CAddDictionaryRequest>(request));

            if (response.StatusCode == HttpStatusCode.Conflict)
                return Result<int>.Failure(await ReadDetailAsync(response) ?? "A dictionary with this name already exists.");

            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<CDictionaryResponse>();
            return Result<int>.Success(created!.Id);
        }

        public async Task<Result> UpdateAsync(UpdateDictionaryRequest request)
        {
            var response = await httpClient.PutAsJsonAsync(
                $"api/dictionaries/{request.DictionaryId}", mapper.Map<CUpdateDictionaryRequest>(request));

            if (response.StatusCode == HttpStatusCode.Conflict)
                return Result.Failure(await ReadDetailAsync(response) ?? "A dictionary with this name already exists.");

            response.EnsureSuccessStatusCode();
            return Result.Success();
        }

        public async Task DeleteAsync(int dictionaryId, int userId)
        {
            var response = await httpClient.DeleteAsync($"api/dictionaries/{dictionaryId}?userId={userId}");
            response.EnsureSuccessStatusCode();
        }

        private static async Task<string?> ReadDetailAsync(HttpResponseMessage response)
        {
            try
            {
                var doc = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (doc.TryGetProperty("detail", out var prop) && prop.ValueKind == JsonValueKind.String)
                    return prop.GetString();
            }
            catch { }

            return null;
        }
    }
}
