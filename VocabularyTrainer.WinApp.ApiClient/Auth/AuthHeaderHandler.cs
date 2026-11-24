using System.Net;
using System.Net.Http.Headers;

namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	// Attaches the access token to every API call and transparently retries once after a 401 with a refreshed token.
	internal sealed class AuthHeaderHandler(IAccessTokenProvider tokenProvider) : DelegatingHandler
	{
		private const string Scheme = "Bearer";

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// A sent HttpRequestMessage cannot be sent again, so the body is buffered up front for a possible retry.
			if (request.Content is not null)
			{
				await request.Content.LoadIntoBufferAsync();
			}

			var accessToken = await tokenProvider.GetAccessTokenAsync(cancellationToken)
				?? throw new SessionExpiredException();

			request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, accessToken);

			var response = await base.SendAsync(request, cancellationToken);

			if (response.StatusCode != HttpStatusCode.Unauthorized)
			{
				return response;
			}

			response.Dispose();

			var refreshedToken = await tokenProvider.RefreshAccessTokenAsync(accessToken, cancellationToken)
				?? throw new SessionExpiredException();

			var retry = await CloneAsync(request);
			retry.Headers.Authorization = new AuthenticationHeaderValue(Scheme, refreshedToken);

			return await base.SendAsync(retry, cancellationToken);
		}

		private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
		{
			var clone = new HttpRequestMessage(request.Method, request.RequestUri)
			{
				Version = request.Version,
				VersionPolicy = request.VersionPolicy,
			};

			foreach (var header in request.Headers)
			{
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			if (request.Content is not null)
			{
				var body = await request.Content.ReadAsByteArrayAsync();
				clone.Content = new ByteArrayContent(body);

				foreach (var header in request.Content.Headers)
				{
					clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
			}

			return clone;
		}
	}
}
