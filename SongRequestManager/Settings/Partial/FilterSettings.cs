namespace SongRequestManager.Settings.Partial
{
	public class FilterSettings
	{
		public virtual int MinimumRating { get; set; } = 0;
		public virtual int MaximumSongDuration { get; set; } = 0;
		public virtual int MinimumNjs { get; set; } = 0;
	}
}