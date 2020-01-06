using System.Windows;

namespace SteamAccountSwitcher.Utils
{
	public static class ViewUtils
	{
		public static void ShowWindow(this Window window, bool notClosing = true)
		{
			if (notClosing)
			{
				window.Visibility = Visibility.Visible;
			}

			Win32.UnMinimize(window);
			if (!window.Topmost)
			{
				window.Topmost = true;
				window.Topmost = false;
			}
		}
	}
}
