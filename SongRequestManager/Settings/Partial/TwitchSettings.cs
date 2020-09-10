namespace SongRequestManager.Settings.Partial
{
	internal class TwitchSettings
	{
		public virtual bool Enabled { get; set; } = true;
		public virtual int UserRequestLimit { get; set; } = 10;
		public virtual int SubRequestLimit { get; set; } = 20;
		public virtual int ModRequestLimit { get; set; } = 25;
		public virtual int VipBonusLimit { get; set; } = 3;
	}
}