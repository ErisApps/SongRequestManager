using IPA.Config.Stores.Attributes;

namespace SongRequestManager.Settings.Partial
{
	internal class TwitchSettings
	{
		[Ignore]
		public const int USER_REQUEST_UPPER_LIMIT = 50;

		[Ignore]
		public const int SUB_REQUEST_UPPER_LIMIT = 50;

		[Ignore]
		public const int MOD_REQUEST_UPPER_LIMIT = 100;

		[Ignore]
		public const int VIP_BONUS_UPPER_LIMIT = 10;

		public virtual bool Enabled { get; set; } = true;
		public virtual int UserRequestLimit { get; set; } = 10;
		public virtual int SubRequestLimit { get; set; } = 20;
		public virtual int ModRequestLimit { get; set; } = 25;
		public virtual int VipBonusLimit { get; set; } = 3;
	}
}