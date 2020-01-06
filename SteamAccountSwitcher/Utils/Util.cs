using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using SteamAccountSwitcher.Collection;
using SteamAccountSwitcher.Steam;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SteamAccountSwitcher.Utils
{
	public static class Util
	{
		public static int GetDeterministicHashCode(this string str)
		{
			unchecked
			{
				var hash1 = (5381 << 16) + 5381;
				var hash2 = hash1;

				for (var i = 0; i < str.Length; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ str[i];
					if (i == str.Length - 1)
						break;
					hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
				}

				return hash1 + hash2 * 1566083941;
			}
		}

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

		public static string GetDllPath()
		{
			return Assembly.GetExecutingAssembly().Location;
		}

		public static string GetExecutablePath()
		{
			var p = Process.GetCurrentProcess();
			if (p.MainModule != null)
			{
				var res = p.MainModule.FileName;
				return res;
			}

			var dllPath = GetDllPath();
			return Path.Combine(Path.GetDirectoryName(dllPath) ?? throw new InvalidOperationException(),
					$@"{Path.GetFileNameWithoutExtension(dllPath)}.exe");
		}
	}
}
