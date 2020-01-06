using System;

namespace SteamAccountSwitcher.SingleInstance
{
	public class ArgumentsReceivedEventArgs : EventArgs
	{
		public string[] Args { get; set; }
	}
}
