using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using SteamAccountSwitcher.Utils;

namespace SteamAccountSwitcher.Steam
{
	internal static class SteamClientHelper
	{
		public static string GetLoginUsersConfigPath()
		{
			return Path.Combine(GetPath(), @"config\loginusers.vdf");
		}

		public static bool DoesLoginUsersConfigExist()
		{
			return File.Exists(GetLoginUsersConfigPath());
		}

		public static string GetSteamConfigPath()
		{
			return Path.Combine(GetPath(), @"config\config.vdf");
		}

		public static bool DoesSteamConfigExist()
		{
			return File.Exists(GetSteamConfigPath());
		}

		public static IEnumerable<string> GetAllLibraries()
		{
			yield return GetPath();
			foreach (var library in VdfHelper.GetKeyPairs(File.ReadAllLines(GetSteamConfigPath()), @"BaseInstallFolder_").Select(libraryPath => libraryPath.Value))
			{
				yield return Path.GetFullPath(library);
			}
		}

		public static IEnumerable<string> GetAllGames(string libraryPath)
		{
			return Directory.GetFiles(Path.Combine(libraryPath, @"steamapps"), @"*.acf");
		}

		private static IEnumerable<string> GetAllWorkshop()
		{
			return GetAllLibraries().Select(lib => Path.Combine(lib, @"steamapps", @"workshop", @"content")).Where(Directory.Exists);
		}

		private static IEnumerable<int> GetAllAppidInWorkshop(string workshopPath)
		{
			var paths = Directory.GetDirectories(workshopPath);
			var list = new List<int>();
			foreach (var item in paths)
			{
				if (int.TryParse(Path.GetFileNameWithoutExtension(item), out var result))
				{
					list.Add(result);
				}
			}
			return list;
		}

		public static IEnumerable<KeyValuePair<int, string>> GetAllAppidInWorkshop()
		{
			return from dir in GetAllWorkshop() from appid in GetAllAppidInWorkshop(dir) select new KeyValuePair<int, string>(appid, dir);
		}

		public static bool IsSteamRunning()
		{
			var user = GetActiveUserSteamId3();
			if (user != null && user != 0)
			{
				return true;
			}
			return false;
		}

		public static uint? GetActiveUserSteamId3()
		{
			uint? steamId3 = null;
			try
			{
				using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam\ActiveProcess"))
				{
					var str = registryKey?.GetValue(@"ActiveUser").ToString();
					if (uint.TryParse(str, out var tempUint))
					{
						steamId3 = tempUint;
					}
				}
			}
			catch
			{
				// ignored
			}

			return steamId3;
		}

		private static string GetStringFromSteamRegistry(string name)
		{
			string res;
			try
			{
				using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
				{
					res = registryKey?.GetValue(name).ToString();
				}
			}
			catch
			{
				res = string.Empty;
			}

			return string.IsNullOrWhiteSpace(res) ? string.Empty : res;
		}

		public static string GetPath()
		{
			var path = GetStringFromSteamRegistry(@"SteamPath");
			return string.IsNullOrWhiteSpace(path) ? path : Path.GetFullPath(path);
		}

		public static string GetExePath()
		{
			var path = GetStringFromSteamRegistry(@"SteamExe");
			return string.IsNullOrWhiteSpace(path) ? path : Path.GetFullPath(path);
		}

		public static string GetAutoLoginUser()
		{
			return GetStringFromSteamRegistry(@"AutoLoginUser");
		}

		public static void SetAutoLoginUser(string username)
		{
			Reg.Set(@"HKEY_CURRENT_USER\Software\Valve\Steam\", @"AutoLoginUser", username, RegistryValueKind.String);
		}

		public static void SetRememberPassword(bool isRemember)
		{
			Reg.Set(@"HKEY_CURRENT_USER\Software\Valve\Steam\", @"RememberPassword", isRemember ? 1 : 0, RegistryValueKind.DWord);
		}
	}
}
