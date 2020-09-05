namespace SongRequestManager.Settings.Partial
{
	internal class TwitchSettings
	{
		public virtual bool Enabled { get; set; } = true;
		public virtual int UserRequestLimit { get; set; }
		public virtual int SubRequestLimit { get; set; }
		public virtual int ModRequestLimit { get; set; }
		public virtual int VipBonusLimit { get; set; }
	}
}