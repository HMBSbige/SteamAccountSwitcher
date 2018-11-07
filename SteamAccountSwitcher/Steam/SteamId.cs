using System;

namespace SteamAccountSwitcher.Steam
{
	//https://developer.valvesoftware.com/wiki/SteamID
	internal class SteamId
	{
		public ulong SteamId64 { get; }
		public uint SteamId3 { get; }
		public char Letter { get; }
		public string SteamID3 => $@"[{Letter}:1:{SteamId3}]";
		//STEAM_X:Y:Z
		public byte X { get; }
		public byte Y { get; }
		public uint Z { get; }
		public string SteamID => $@"STEAM_{X}:{Y}:{Z}";

		private const uint Individual = 0x01100001;
		private const uint Clan = 0x01700000;

		public SteamId(ulong steamId64)
		{
			SteamId64 = steamId64;
			SteamId3 = Convert.ToUInt32(steamId64 & 0xFFFFFFFF);
			Y = Convert.ToByte(SteamId3 & 0x1);
			Z = SteamId3 >> 1;
			X = Convert.ToByte(steamId64 >> 56);
			var identifier = Convert.ToUInt32(steamId64 >> 32);
			if (identifier == Individual)
			{
				Letter = 'U';
			}
			else if (identifier == Clan)
			{
				Letter = 'g';
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public SteamId(uint steamId3) : this(steamId3 + 0x0110000100000000ul) { }

		public SteamId(byte y, uint z) : this((z << 1) + y) { }
	}
}
