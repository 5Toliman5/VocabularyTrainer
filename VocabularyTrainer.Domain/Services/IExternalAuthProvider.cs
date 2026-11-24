using Common.Wrappers;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	// Telegram, Google, Apple, ... — validates a provider-issued credential (id_token, initData, ...)
	// and returns the stable provider-side user id. Failure should use ResultErrorKind.Unauthorized.
	public interface IExternalAuthProvider
	{
		string Code { get; }

		Task<Result<ExternalIdentity>> ValidateAsync(string credential);
	}
}
