using IPA.Config.Stores.Attributes;

namespace SongRequestManager.Settings.Partial
{
	public class FilterSettings
	{
		[Ignore]
		public const int MAX_SONG_DURATION_UPPER_LIMIT = 999;

		public virtual int MinimumRating { get; set; } = 0;
		public virtual int MaximumSongDuration { get; set; } = 1000;
		public virtual int MinimumNjs { get; set; } = 0;
	}
}