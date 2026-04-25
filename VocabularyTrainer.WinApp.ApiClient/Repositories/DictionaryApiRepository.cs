using System.Net.Http.Json;
using AutoMapper;
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
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<CDictionaryResponse>();
            return created!.Id;
        }

        public async Task UpdateAsync(UpdateDictionaryRequest request)
        {
            var response = await httpClient.PutAsJsonAsync(
                $"api/dictionaries/{request.DictionaryId}", mapper.Map<CUpdateDictionaryRequest>(request));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int dictionaryId, int userId)
        {
            var response = await httpClient.DeleteAsync($"api/dictionaries/{dictionaryId}?userId={userId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
