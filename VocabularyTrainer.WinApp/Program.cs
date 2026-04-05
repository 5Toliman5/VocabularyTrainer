using Microsoft.Extensions.DependencyInjection;
using Serilog;
using VocabularyTrainer.WinApp.Infrastructure.AppStart;
using VocabularyTrainer.WinApp.Presenter;
using WinApp;

namespace VocabularyTrainer.WinApp
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += Application_ThreadException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			ApplicationConfiguration.Initialize();

			try
			{
				var services = DependencyResolver.ConstructServices();
				using var serviceProvider = services.BuildServiceProvider();
				var mainForm = serviceProvider.GetRequiredService<MainForm>();
				_ = serviceProvider.GetRequiredService<MainFormPresenter>();

				Application.Run(mainForm);
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Fatal error during application startup");
				Log.CloseAndFlush();
				MessageBox.Show(
					$"Failed to start the application.\n\nDetails: {ex.Message}",
					"Startup Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Log.Error(e.Exception, "Unhandled UI thread exception");
			ShowGlobalExceptionMessage(e.Exception);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;
			Log.Fatal(ex, "Unhandled AppDomain exception (terminating: {IsTerminating})", e.IsTerminating);
			Log.CloseAndFlush();
			ShowGlobalExceptionMessage(ex);
		}

		private static void ShowGlobalExceptionMessage(Exception? ex)
		{
			var message = "An unexpected error occurred. Please try reloading the application.";
			if (ex is not null)
				message += $"\n\nDetails: {ex.Message}";

			MessageBox.Show(message, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
