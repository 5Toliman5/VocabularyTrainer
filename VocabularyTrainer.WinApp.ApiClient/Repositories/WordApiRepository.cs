using System.Net.Http.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CAddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CDeleteWordRequest = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using CUpdateWeightRequest = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    /// <summary>Implements <see cref="IWordRepository"/> by calling the VocabularyTrainer HTTP API.</summary>
    internal class WordApiRepository(HttpClient httpClient, IMapper mapper) : IWordRepository
    {
        /// <inheritdoc/>
        public async Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
        {
            var url = $"api/words?userId={userId}";
            if (dictionaryId.HasValue)
                url += $"&dictionaryId={dictionaryId}";

            var response = await httpClient.GetFromJsonAsync<List<CWordResponse>>(url);
            return mapper.Map<List<WordDto>>(response);
        }

        /// <inheritdoc/>
        public async Task AddAsync(AddWordRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("api/words", mapper.Map<CAddWordRequest>(request));
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(UserWordKey key)
        {
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/words")
            {
                Content = JsonContent.Create(mapper.Map<CDeleteWordRequest>(key))
            });
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public async Task UpdateWeightAsync(UpdateWordWeightRequest request)
        {
            var response = await httpClient.PatchAsJsonAsync("api/words/weight", mapper.Map<CUpdateWeightRequest>(request));
            response.EnsureSuccessStatusCode();
        }
    }
}
