using Gameloop.Vdf.Linq;
using System;

namespace SteamAccountSwitcher.Collection
{
	public class LoginUsers
	{
		public LoginUsers(VProperty volvo)
		{
			try
			{
				SteamId64 = ulong.Parse(volvo.Key);
				AccountName = volvo.Value[@"AccountName"]!.ToString();
				PersonaName = volvo.Value[@"PersonaName"]!.ToString();
				RememberPassword = int.Parse(volvo.Value[@"RememberPassword"]!.ToString()) == 1;
				MostRecent = int.Parse(volvo.Value[@"MostRecent"]!.ToString()) == 1;
				LastLoginTime = Utils.Util.GetTime(volvo.Value[@"Timestamp"]!.ToString());
			}
			catch
			{
				RememberPassword = false;
			}
		}

		public ulong SteamId64 { get; }

		public string AccountName { get; }

		public string PersonaName { get; }

		public bool RememberPassword { get; }

		public bool MostRecent { get; }

		public DateTime LastLoginTime { get; }
	}
}
