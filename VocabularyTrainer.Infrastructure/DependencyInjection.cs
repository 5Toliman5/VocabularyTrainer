using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.Infrastructure.Email;

namespace VocabularyTrainer.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
			services.AddSingleton<IEmailSender, SmtpEmailSender>();

			return services;
		}
	}
}
