using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CWordPageItem = VocabularyTrainer.Api.Contract.Words.WordPageItem;
using CAddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
	internal class WordApiRepository(HttpClient httpClient, IMapper mapper) : IWordRepository
	{
		public async Task<List<WordDto>> GetAllAsync(int userId, int? dictionaryId = null)
		{
			var url = $"api/words?userId={userId}";

			if (dictionaryId.HasValue)
			{
				url += $"&dictionaryId={dictionaryId}";
			}

			var response = await httpClient.GetFromJsonAsync<List<CWordResponse>>(url);
			return mapper.Map<List<WordDto>>(response);
		}

		public async Task<PagedResult<WordDto>> GetPagedAsync(GetWordsPagedRequest request)
		{
			var url = BuildPagedUrl(request);
			var response = await httpClient.GetFromJsonAsync<PagedResult<CWordPageItem>>(url);

			if (response is null)
			{
				return new PagedResult<WordDto>([], 0, request.Page, request.PageSize);
			}

			var items = mapper.Map<IReadOnlyList<WordDto>>(response.Items);
			return new PagedResult<WordDto>(items, response.TotalCount, response.Page, response.PageSize);
		}

		public Task<List<WordDto>> GetByIdsAsync(int userId, IReadOnlyCollection<int> wordIds)
		{
			throw new NotSupportedException("Hydration by ids is server-only; not available in the API client.");
		}

		public async Task<int> AddAsync(AddWordRequest request, string normalizedText, string languageCode)
		{
			_ = normalizedText;
			_ = languageCode;

			var response = await httpClient.PostAsJsonAsync("api/words", mapper.Map<CAddWordRequest>(request));

			if (response.StatusCode == HttpStatusCode.Conflict)
			{
				throw new DuplicateKeyException(
					await ReadDetailAsync(response) ?? "A word with this text already exists in this dictionary.");
			}

			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				throw new DomainValidationException(await ReadDetailAsync(response) ?? "Validation failed.");
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new EntityNotFoundException(await ReadDetailAsync(response) ?? "Dictionary was not found.");
			}

			response.EnsureSuccessStatusCode();

			var created = await response.Content.ReadFromJsonAsync<JsonElement>();

			if (created.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out var id))
			{
				return id;
			}

			return 0;
		}

		public async Task<int> DeleteAsync(int wordId, int userId)
		{
			var response = await httpClient.DeleteAsync($"api/words/{wordId}?userId={userId}");

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return 0;
			}

			response.EnsureSuccessStatusCode();
			return 1;
		}

		private static string BuildPagedUrl(GetWordsPagedRequest request)
		{
			var sb = new StringBuilder($"api/words/paged?userId={request.UserId}&page={request.Page}&pageSize={request.PageSize}");

			if (request.DictionaryId.HasValue)
			{
				sb.Append($"&dictionaryId={request.DictionaryId}");
			}

			if (request.Language != null)
			{
				sb.Append($"&language={Uri.EscapeDataString(request.Language)}");
			}

			if (request.Search != null)
			{
				sb.Append($"&search={Uri.EscapeDataString(request.Search)}");
			}

			if (request.DateFrom.HasValue)
			{
				sb.Append($"&dateFrom={request.DateFrom.Value:O}");
			}

			if (request.DateTo.HasValue)
			{
				sb.Append($"&dateTo={request.DateTo.Value:O}");
			}

			sb.Append($"&sortBy={request.SortBy}&sortDesc={request.SortDesc}");
			return sb.ToString();
		}

		private static async Task<string?> ReadDetailAsync(HttpResponseMessage response)
		{
			try
			{
				var doc = await response.Content.ReadFromJsonAsync<JsonElement>();

				if (doc.TryGetProperty("detail", out var prop) && prop.ValueKind == JsonValueKind.String)
				{
					return prop.GetString();
				}
			}
			catch
			{
			}

			return null;
		}
	}
}
