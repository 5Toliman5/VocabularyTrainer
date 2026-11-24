using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Common.Wrappers;

namespace VocabularyTrainer.WinApp.ApiClient
{
	public static class ApiFailures
	{
		// HttpClient reports its own timeout as TaskCanceledException, not HttpRequestException.
		public static bool IsUnreachable(Exception exception) => exception is HttpRequestException or TaskCanceledException;

		// Null for 2xx; a Result for expected API failures; throws for anything unexpected.
		internal static async Task<Result?> ToFailureAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode) return null;

			ResultErrorKind? kind = response.StatusCode switch
			{
				HttpStatusCode.BadRequest => ResultErrorKind.Validation,
				HttpStatusCode.Unauthorized => ResultErrorKind.Unauthorized,
				HttpStatusCode.NotFound => ResultErrorKind.NotFound,
				HttpStatusCode.Conflict => ResultErrorKind.Conflict,
				HttpStatusCode.TooManyRequests => ResultErrorKind.TooManyRequests,
				_ => null,
			};

			if (kind is null)
			{
				response.EnsureSuccessStatusCode();
			}

			var detail = await ReadDetailAsync(response);

			return Result.Failure(detail ?? DefaultMessage(kind!.Value), kind!.Value);
		}

		// The "detail" field of a ProblemDetails body, if there is one.
		internal static async Task<string?> ReadDetailAsync(HttpResponseMessage response)
		{
			try
			{
				var document = await response.Content.ReadFromJsonAsync<JsonElement>();

				if (document.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
				{
					return detail.GetString();
				}
			}
			catch (JsonException)
			{
			}

			return null;
		}

		private static string DefaultMessage(ResultErrorKind kind)
		{
			return kind switch
			{
				ResultErrorKind.TooManyRequests => "Too many attempts. Please wait a minute and try again.",
				ResultErrorKind.Unauthorized => "Invalid login or password.",
				_ => "Please check the entered data.",
			};
		}
	}
}
