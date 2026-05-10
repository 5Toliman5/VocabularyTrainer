using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Algorithms
{
	public class AlgorithmsSeeder(ITrainingAlgorithmRegistry registry, IAlgorithmRepository algorithms)
	{
		public async Task SeedAsync()
		{
			foreach (var algorithm in registry.All)
			{
				await algorithms.EnsureExistsAsync(algorithm.Info.Code);
			}
		}
	}
}
