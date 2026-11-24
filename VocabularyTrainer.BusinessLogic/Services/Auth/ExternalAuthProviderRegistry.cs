using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	public class ExternalAuthProviderRegistry(IEnumerable<IExternalAuthProvider> providers) : IExternalAuthProviderRegistry
	{
		private readonly IReadOnlyDictionary<string, IExternalAuthProvider> _byCode =
			providers.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

		public IExternalAuthProvider? Find(string code)
		{
			if (code is null) return null;

			return _byCode.TryGetValue(code, out var provider)
				? provider
				: null;
		}
	}
}
