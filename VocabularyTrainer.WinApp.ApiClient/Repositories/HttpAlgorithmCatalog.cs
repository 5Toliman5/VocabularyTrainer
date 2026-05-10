using System.Net.Http.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;
using CAlgorithmInfoResponse = VocabularyTrainer.Api.Contract.Algorithms.AlgorithmInfoResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
	internal class HttpAlgorithmCatalog(HttpClient httpClient, IMapper mapper) : IAlgorithmCatalog
	{
		public async Task<IReadOnlyList<AlgorithmInfo>> GetAllAsync()
		{
			var response = await httpClient.GetFromJsonAsync<List<CAlgorithmInfoResponse>>("api/algorithms") ?? [];
			return mapper.Map<List<AlgorithmInfo>>(response);
		}
	}
}
