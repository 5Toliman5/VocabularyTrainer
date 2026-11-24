using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Common.Wrappers;
using Microsoft.Extensions.Options;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	public class VerificationCodeService : IVerificationCodeService
	{
		private const string InvalidCode = "The code is invalid or has expired.";

		private readonly IVerificationCodeRepository _repository;
		private readonly IReadOnlyDictionary<string, IVerificationCodeSender> _senders;
		private readonly AuthOptions _options;

		public VerificationCodeService(
			IVerificationCodeRepository repository,
			IEnumerable<IVerificationCodeSender> senders,
			IOptions<AuthOptions> options)
		{
			_repository = repository;
			_senders = senders.ToDictionary(s => s.Channel, StringComparer.Ordinal);
			_options = options.Value;
		}

		public bool IsChannelSupported(string channel) => channel is not null && _senders.ContainsKey(channel);

		public async Task<Result> SendAsync(string channel, string target, VerificationPurpose purpose)
		{
			if (channel is null || !_senders.TryGetValue(channel, out var sender))
			{
				return Result.Failure($"Codes cannot be sent via '{channel}'.", ResultErrorKind.Validation);
			}

			var now = DateTime.UtcNow;
			var latest = await _repository.GetLatestAsync(channel, target, purpose);

			if (latest is not null && now - latest.CreatedAt < _options.VerificationCodeResendCooldown)
			{
				return Result.Failure("Please wait a minute before requesting another code.", ResultErrorKind.TooManyRequests);
			}

			var sentLastHour = await _repository.CountCreatedSinceAsync(channel, target, now.AddHours(-1));

			if (sentLastHour >= _options.VerificationCodesPerHour)
			{
				return Result.Failure("Too many codes requested. Please try again later.", ResultErrorKind.TooManyRequests);
			}

			var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6", CultureInfo.InvariantCulture);

			var record = new NewVerificationCode(
				channel,
				target,
				purpose,
				HashCode(channel, target, purpose, code),
				now,
				now + _options.VerificationCodeLifetime
			);

			var codeId = await _repository.AddAsync(record);

			try
			{
				await sender.SendAsync(target, code, purpose, _options.VerificationCodeLifetime);
			}
			catch
			{
				// An undelivered code must not trigger the resend cooldown or count toward the hourly cap.
				await _repository.DeleteAsync(codeId);
				throw;
			}

			return Result.Success();
		}

		public async Task<Result> VerifyAsync(string channel, string target, VerificationPurpose purpose, string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				return Result.Failure("Please enter the code.", ResultErrorKind.Validation);
			}

			var latest = await _repository.GetLatestAsync(channel, target, purpose);

			if (latest is null || latest.ConsumedAt is not null || latest.ExpiresAt <= DateTime.UtcNow)
			{
				return Result.Failure(InvalidCode, ResultErrorKind.Validation);
			}

			if (latest.AttemptCount >= _options.VerificationCodeMaxAttempts)
			{
				return Result.Failure("Too many wrong attempts. Please request a new code.", ResultErrorKind.TooManyRequests);
			}

			var actualHash = HashCode(channel, target, purpose, code.Trim());

			if (!CryptographicOperations.FixedTimeEquals(actualHash, latest.CodeHash))
			{
				await _repository.IncrementAttemptsAsync(latest.Id);
				return Result.Failure(InvalidCode, ResultErrorKind.Validation);
			}

			if (!await _repository.ConsumeAsync(latest.Id))
			{
				return Result.Failure(InvalidCode, ResultErrorKind.Validation);
			}

			return Result.Success();
		}

		// Binding the hash to channel/target/purpose stops a code issued for one flow from being replayed in another.
		private static byte[] HashCode(string channel, string target, VerificationPurpose purpose, string code)
		{
			return SHA256.HashData(Encoding.UTF8.GetBytes($"{channel}:{target}:{purpose}:{code}"));
		}
	}
}
