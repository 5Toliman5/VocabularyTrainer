using System.Net;
using System.Net.Http.Json;
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
			var response = await httpClient.GetFromJsonAsync<List<CDictionaryResponse>>("api/dictionaries");
			return mapper.Map<List<DictionaryDto>>(response);
		}

		public async Task<DictionaryDto?> GetByIdAsync(int dictionaryId, int userId)
		{
			var response = await httpClient.GetAsync($"api/dictionaries/{dictionaryId}");

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}

			response.EnsureSuccessStatusCode();

			var dto = await response.Content.ReadFromJsonAsync<CDictionaryResponse>();

			return dto is null
				? null
				: mapper.Map<DictionaryDto>(dto);
		}

		public async Task<int> AddAsync(AddDictionaryRequest request, int _)
		{
			var response = await httpClient.PostAsJsonAsync("api/dictionaries", mapper.Map<CAddDictionaryRequest>(request));

			if (response.StatusCode == HttpStatusCode.Conflict)
			{
				throw new DuplicateKeyException(
					await ApiFailures.ReadDetailAsync(response) ?? "A dictionary with this name already exists.");
			}

			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				throw new DomainValidationException(await ApiFailures.ReadDetailAsync(response) ?? "Validation failed.");
			}

			response.EnsureSuccessStatusCode();

			var created = await response.Content.ReadFromJsonAsync<CDictionaryResponse>();
			return created!.Id;
		}

		public async Task<int> UpdateAsync(UpdateDictionaryRequest request, int _)
		{
			var response = await httpClient.PutAsJsonAsync(
				$"api/dictionaries/{request.DictionaryId}",
				mapper.Map<CUpdateDictionaryRequest>(request)
			);

			if (response.StatusCode == HttpStatusCode.Conflict)
			{
				throw new DuplicateKeyException(
					await ApiFailures.ReadDetailAsync(response) ?? "A dictionary with this name already exists.");
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				throw new EntityNotFoundException(
					await ApiFailures.ReadDetailAsync(response) ?? $"Dictionary {request.DictionaryId} was not found.");
			}

			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				throw new DomainValidationException(await ApiFailures.ReadDetailAsync(response) ?? "Validation failed.");
			}

			response.EnsureSuccessStatusCode();
			return 1;
		}

		public async Task DeleteAsync(int dictionaryId, int userId)
		{
			var response = await httpClient.DeleteAsync($"api/dictionaries/{dictionaryId}");
			response.EnsureSuccessStatusCode();
		}
	}
}
