using Common.Wrappers;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	public interface IVerificationCodeService
	{
		bool IsChannelSupported(string channel);

		// Target must already be normalized for the channel.
		Task<Result> SendAsync(string channel, string target, VerificationPurpose purpose);

		Task<Result> VerifyAsync(string channel, string target, VerificationPurpose purpose, string code);
	}
}
