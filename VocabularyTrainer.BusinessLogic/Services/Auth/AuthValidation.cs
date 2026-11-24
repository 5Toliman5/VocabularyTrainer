using Common.Wrappers;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	internal static class AuthValidation
	{
		public const int MinPasswordLength = 8;
		public const int MaxPasswordLength = 128;
		public const int MaxDisplayNameLength = 64;
		public const int MaxClientTypeLength = 20;
		public const int MaxDeviceNameLength = 128;

		public static Result ValidatePassword(string password)
		{
			if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
			{
				return Result.Failure(
					$"Password must be at least {MinPasswordLength} characters long.",
					ResultErrorKind.Validation
				);
			}

			if (password.Length > MaxPasswordLength)
			{
				return Result.Failure(
					$"Password must be at most {MaxPasswordLength} characters long.",
					ResultErrorKind.Validation
				);
			}

			return Result.Success();
		}

		public static Result ValidateDisplayName(string displayName)
		{
			if (string.IsNullOrWhiteSpace(displayName))
			{
				return Result.Failure("Name is required.", ResultErrorKind.Validation);
			}

			if (displayName.Trim().Length > MaxDisplayNameLength)
			{
				return Result.Failure(
					$"Name must be at most {MaxDisplayNameLength} characters long.",
					ResultErrorKind.Validation
				);
			}

			return Result.Success();
		}

		public static Result ValidateClient(ClientInfo client)
		{
			if (client is null || string.IsNullOrWhiteSpace(client.ClientType))
			{
				return Result.Failure("Client type is required.", ResultErrorKind.Validation);
			}

			if (client.ClientType.Length > MaxClientTypeLength)
			{
				return Result.Failure("Client type is too long.", ResultErrorKind.Validation);
			}

			if (client.DeviceName is not null && client.DeviceName.Length > MaxDeviceNameLength)
			{
				return Result.Failure("Device name is too long.", ResultErrorKind.Validation);
			}

			return Result.Success();
		}

		public static Result InvalidTarget(string channel)
		{
			return Result.Failure($"Please enter a valid {channel}.", ResultErrorKind.Validation);
		}
	}
}
