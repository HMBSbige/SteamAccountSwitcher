using System.Collections.Generic;
using System.Linq;

namespace SteamAccountSwitcher.Steam
{
	internal static class VdfHelper
	{
		public static IEnumerable<KeyValuePair<string, string>> GetKeyPairs(IEnumerable<string> data, string keyName)
		{
			return from line in data
				   where line.Contains(keyName)
				   select line.Split('\"', '\"')
				   into output
				   select new KeyValuePair<string, string>(output[1], output[3]);
		}

		public static IEnumerable<string> SetKeyPair(IEnumerable<string> data, KeyValuePair<string, string> keyPair)
		{
			var newList = new List<string>();
			foreach (var line in data)
			{
				var newLine = line;
				if (line.Contains(keyPair.Key))
				{
					var output = line.Split('\"', '\"');
					newLine = line.Replace(output[1], keyPair.Key).Replace(output[3], keyPair.Value);
				}

				newList.Add(newLine);
			}

			return newList;
		}
	}
}
