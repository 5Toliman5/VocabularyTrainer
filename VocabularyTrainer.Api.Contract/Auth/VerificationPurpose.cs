using System.Text.Json.Serialization;

namespace VocabularyTrainer.Api.Contract.Auth
{
	[JsonConverter(typeof(JsonStringEnumConverter<VerificationPurpose>))]
	public enum VerificationPurpose
	{
		Register,
		ResetPassword,
	}
}
