using SteamAccountSwitcher.Collection;
using SteamAccountSwitcher.Steam;
using SteamAccountSwitcher.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SteamAccountSwitcher
{
	public partial class MainWindow
	{
		private string _steamStartupArgs;
		private string _steamExePath;
		private List<LoginUsers> _users;

		public MainWindow(string args)
		{
			try
			{
				Init(args);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message, @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(1);
			}

			InitializeComponent();
		}

		private void Init(string args)
		{
			_steamStartupArgs = args;

			_steamExePath = SteamClientHelper.GetExePath();
			if (!File.Exists(_steamExePath))
			{
				throw new IOException(@"Cannot find Steam!");
			}

			_users = Util.GetLoginUsers().ToList();
			if (_users.Count == 0)
			{
				throw new IOException(@"Cannot find saved users!");
			}
		}

		private void Window0_Initialized(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(_steamStartupArgs))
			{
				LaunchOptionsTextBox.Text = _steamStartupArgs.Trim();
			}

			foreach (var user in _users)
			{
				UsersBox.Items.Add(user.AccountName);
				if (user.MostRecent)
				{
					UsersBox.SelectedIndex = UsersBox.Items.Count - 1;
				}
			}
		}

		private void UsersBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (UsersBox.SelectedIndex > -1)
			{
				PersonaNameBox.Text = _users[UsersBox.SelectedIndex].PersonaName;
				LastLoginTimeBox.Text = _users[UsersBox.SelectedIndex].LastLoginTime.ToString(CultureInfo.CurrentCulture);
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			IsEnabled = false;
			await Task.Run(() => Dispatcher?.InvokeAsync(() =>
			{
				var username = _users[UsersBox.SelectedIndex].AccountName;
				SteamClientHelper.SetAutoLoginUser(username);
				SteamClientHelper.SetRememberPassword(true);
				WinProcess.Restart(_steamExePath, LaunchOptionsTextBox.Text);
			}));
			IsEnabled = true;
		}
	}
}
