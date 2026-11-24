using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using VocabularyTrainer.WinApp.ApiClient.Auth;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	// The refresh token is encrypted with the Windows user's DPAPI key, so other accounts on the PC cannot read it.
	// The file is shared by every running instance of the app. Storage failures are logged, never thrown:
	// the session keeps working for this run, it just will not survive a restart.
	internal sealed class DpapiTokenStore(ILogger<DpapiTokenStore> logger) : ITokenStore
	{
		private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("VocabularyTrainer.RefreshToken");

		private static readonly string FilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"VocabularyTrainer",
			"session.dat"
		);

		public string? Load()
		{
			if (!File.Exists(FilePath)) return null;

			try
			{
				var protectedBytes = File.ReadAllBytes(FilePath);
				var bytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
				return Encoding.UTF8.GetString(bytes);
			}
			catch (Exception ex) when (IsStorageError(ex))
			{
				logger.LogWarning(ex, "Stored session could not be read");
				return null;
			}
		}

		public void Save(string refreshToken)
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

				var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(refreshToken), Entropy, DataProtectionScope.CurrentUser);

				// Write-then-rename so another instance reading concurrently never sees a half-written file.
				var tempPath = FilePath + ".tmp";
				File.WriteAllBytes(tempPath, bytes);
				File.Move(tempPath, FilePath, overwrite: true);
			}
			catch (Exception ex) when (IsStorageError(ex))
			{
				logger.LogWarning(ex, "Session could not be saved; sign-in will be required after restart");
			}
		}

		public void Clear()
		{
			try
			{
				File.Delete(FilePath);
			}
			catch (Exception ex) when (IsStorageError(ex))
			{
				logger.LogWarning(ex, "Stored session could not be deleted");
			}
		}

		private static bool IsStorageError(Exception exception) =>
			exception is IOException or UnauthorizedAccessException or CryptographicException;
	}
}
