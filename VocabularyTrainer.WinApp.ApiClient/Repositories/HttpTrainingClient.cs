using System.Net.Http.Json;
using AutoMapper;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;
using CWordResponse = VocabularyTrainer.Api.Contract.Words.WordResponse;
using CReviewWordRequest = VocabularyTrainer.Api.Contract.Words.ReviewWordRequest;
using CReviewGrade = VocabularyTrainer.Api.Contract.Words.ReviewGrade;

namespace VocabularyTrainer.WinApp.ApiClient.Repositories
{
	internal class HttpTrainingClient(HttpClient httpClient, IMapper mapper) : ITrainingClient
	{
		public async Task<List<WordDto>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit)
		{
			var url = $"api/training/candidates?userId={userId}&scope={scope}&limit={limit}";
			var response = await httpClient.GetFromJsonAsync<List<CWordResponse>>(url) ?? [];
			return mapper.Map<List<WordDto>>(response);
		}

		public async Task ApplyReviewAsync(ReviewWordRequest request)
		{
			var contractGrade = mapper.Map<CReviewGrade>(request.Grade);
			var contractReq = new CReviewWordRequest(request.UserId, request.DictionaryId, contractGrade);

			var response = await httpClient.PostAsJsonAsync($"api/training/review/{request.WordId}", contractReq);
			response.EnsureSuccessStatusCode();
		}

		public async Task<IReadOnlyList<ReviewGrade>> GetSupportedGradesAsync(int dictionaryId, int userId)
		{
			var url = $"api/training/supported-grades?dictionaryId={dictionaryId}&userId={userId}";
			var response = await httpClient.GetFromJsonAsync<List<CReviewGrade>>(url) ?? [];
			return response.Select(g => mapper.Map<ReviewGrade>(g)).ToList();
		}
	}
}
