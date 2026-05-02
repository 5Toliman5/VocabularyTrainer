using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Exceptions;
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

        public async Task<int> AddAsync(AddDictionaryRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("api/dictionaries", mapper.Map<CAddDictionaryRequest>(request));
            await EnsureSuccessAsync(response);
            var created = await response.Content.ReadFromJsonAsync<CDictionaryResponse>();
            return created!.Id;
        }

        public async Task UpdateAsync(UpdateDictionaryRequest request)
        {
            var response = await httpClient.PutAsJsonAsync(
                $"api/dictionaries/{request.DictionaryId}", mapper.Map<CUpdateDictionaryRequest>(request));
            await EnsureSuccessAsync(response);
        }

        public async Task DeleteAsync(int dictionaryId, int userId)
        {
            var response = await httpClient.DeleteAsync($"api/dictionaries/{dictionaryId}?userId={userId}");
            await EnsureSuccessAsync(response);
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string? detail = null;
            try
            {
                var doc = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (doc.TryGetProperty("detail", out var prop) && prop.ValueKind == JsonValueKind.String)
                    detail = prop.GetString();
            }
            catch { }

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new DuplicateNameException(new Exception(detail ?? "Duplicate name."));

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new DatabaseException(detail ?? "Database error.", new Exception(detail));

            throw new Exception(detail ?? $"Request failed ({(int)response.StatusCode}).");
        }
    }
}
