using System.Net.Http.Json;
using System.Text;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CAddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using CDeleteWordRequest = VocabularyTrainer.Api.Contract.Words.DeleteWordRequest;
using CUpdateWeightRequest = VocabularyTrainer.Api.Contract.Words.UpdateWordWeightRequest;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
    internal class WordApiRepository(HttpClient httpClient, IMapper mapper) : IWordRepository
    {
        public async Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
        {
            var url = $"api/words?userId={userId}";
            if (dictionaryId.HasValue)
                url += $"&dictionaryId={dictionaryId}";

            var response = await httpClient.GetFromJsonAsync<List<CWordResponse>>(url);
            return mapper.Map<List<WordDto>>(response);
        }

        public async Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
        {
            var url = BuildPagedUrl(request);
            var response = await httpClient.GetFromJsonAsync<PagedResult<CWordPageItem>>(url);
            if (response is null)
                return new PagedResult<WordDto>([], 0, request.Page, request.PageSize);

            var items = mapper.Map<IReadOnlyList<WordDto>>(response.Items);
            return new PagedResult<WordDto>(items, response.TotalCount, response.Page, response.PageSize);
        }

        public async Task AddAsync(AddWordRequest request)
        {
            var response = await httpClient.PostAsJsonAsync("api/words", mapper.Map<CAddWordRequest>(request));
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(UserWordKey key)
        {
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "api/words")
            {
                Content = JsonContent.Create(mapper.Map<CDeleteWordRequest>(key))
            });
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateWeightAsync(UpdateWordWeightRequest request)
        {
            var response = await httpClient.PatchAsJsonAsync("api/words/weight", mapper.Map<CUpdateWeightRequest>(request));
            response.EnsureSuccessStatusCode();
        }

        private static string BuildPagedUrl(GetWordsPagedRequest r)
        {
            var sb = new StringBuilder($"api/words/paged?userId={r.UserId}&page={r.Page}&pageSize={r.PageSize}");
            if (r.DictionaryId.HasValue) sb.Append($"&dictionaryId={r.DictionaryId}");
            if (r.Language != null)     sb.Append($"&language={Uri.EscapeDataString(r.Language)}");
            if (r.Search != null)       sb.Append($"&search={Uri.EscapeDataString(r.Search)}");
            if (r.DateFrom.HasValue)    sb.Append($"&dateFrom={r.DateFrom.Value:O}");
            if (r.DateTo.HasValue)      sb.Append($"&dateTo={r.DateTo.Value:O}");
            sb.Append($"&sortBy={r.SortBy}&sortDesc={r.SortDesc}");
            return sb.ToString();
        }
    }
}
