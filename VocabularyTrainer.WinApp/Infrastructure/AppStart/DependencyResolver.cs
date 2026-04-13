using Microsoft.Extensions.DependencyInjection;
using VocabularyTrainer.WinApp.Presenter;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Infrastructure.AppStart
{
	internal static class DependencyResolver
	{
		/// <summary>Builds and returns the application <see cref="IServiceCollection"/> with all services, the view, and the presenter registered.</summary>
		public static IServiceCollection ConstructServices()
		{
			var config = ConfigLoader.Load();
			var services = new ServiceCollection();

			services.AddVocabularyTrainerServices(config);

			services.AddSingleton<IMainFormView, MainForm>();
			services.AddSingleton(sp => (MainForm)sp.GetRequiredService<IMainFormView>());
			services.AddSingleton<MainFormPresenter>();

			return services;
		}
	}
}
