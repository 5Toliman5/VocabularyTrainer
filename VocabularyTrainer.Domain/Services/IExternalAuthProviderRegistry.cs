namespace VocabularyTrainer.Domain.Services
{
	public interface IExternalAuthProviderRegistry
	{
		IExternalAuthProvider? Find(string code);
	}
}
