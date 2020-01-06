using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using SteamAccountSwitcher.Collection;
using SteamAccountSwitcher.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SteamAccountSwitcher.Utils
{
	public static class Util
	{
		public static DateTime GetTime(string timeStamp)
		{
			var dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
			var lTime = long.Parse($@"{timeStamp}0000000");
			var toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}

		public static string GetLoginUsersConfig()
		{
			if (SteamClientHelper.DoesLoginUsersConfigExist())
			{
				return File.ReadAllText(SteamClientHelper.GetLoginUsersConfigPath());
			}
			else
			{
				throw new IOException(@"Cannot find config.");
			}
		}

		public static IEnumerable<LoginUsers> GetLoginUsers()
		{
			dynamic volvo = VdfConvert.Deserialize(GetLoginUsersConfig());
			VToken v2 = volvo.Value;

			return v2.Children().Select(child => new LoginUsers(child)).Where(user => user.RememberPassword).ToList();
		}
	}
}
