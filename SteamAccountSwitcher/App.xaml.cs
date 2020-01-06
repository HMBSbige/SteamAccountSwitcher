using System.Text;
using System.Windows;

namespace SteamAccountSwitcher
{
	public partial class App
	{
		public static string StartupArgs;
		protected override void OnStartup(StartupEventArgs e)
		{
			var sb = new StringBuilder();

			foreach (var arg in e.Args)
			{
				sb.Append(' ');
				sb.Append(arg);
			}

			StartupArgs = sb.ToString();
		}
	}
}
