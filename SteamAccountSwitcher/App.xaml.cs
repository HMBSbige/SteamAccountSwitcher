using SteamAccountSwitcher.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SteamAccountSwitcher
{
	public partial class App
	{
		private static int _exited;

		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Util.GetExecutablePath()) ?? throw new InvalidOperationException());
			var identifier = $@"Global\SteamAccountSwitcher_{Directory.GetCurrentDirectory().GetDeterministicHashCode()}";
			var singleInstance = new SingleInstance.SingleInstance(identifier);
			if (!singleInstance.IsFirstInstance)
			{
				singleInstance.PassArgumentsToFirstInstance(e.Args.Append(Constants.ParameterShow));
				Current.Shutdown();
				return;
			}

			singleInstance.ArgumentsReceived.Subscribe(SingleInstance_ArgumentsReceived);
			singleInstance.ListenForArgumentsFromSuccessiveInstances();
			Current.DispatcherUnhandledException += (o, args) =>
			{
				if (Interlocked.Increment(ref _exited) == 1)
				{
					MessageBox.Show($@"未捕获异常：{args.Exception}", @"SteamAccountSwitcher", MessageBoxButton.OK, MessageBoxImage.Error);
					singleInstance.Dispose();
					Environment.Exit(1);
				}
			};
			Current.Exit += (o, args) => singleInstance.Dispose();

			MainWindow = new MainWindow(string.Join(@" ", e.Args));
			MainWindow.Show();
		}

		private void SingleInstance_ArgumentsReceived(string[] args)
		{
			if (args.Contains(Constants.ParameterShow))
			{
				Dispatcher?.InvokeAsync(() => MainWindow?.ShowWindow());
			}
		}
	}
}
