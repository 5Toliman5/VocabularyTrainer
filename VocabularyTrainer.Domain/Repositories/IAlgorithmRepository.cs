namespace VocabularyTrainer.Domain.Repositories
{
	public interface IAlgorithmRepository
	{
		Task<int?> GetIdByCodeAsync(string code);
		Task EnsureExistsAsync(string code);
	}
}
