using IPA.Config.Stores.Attributes;

namespace SongRequestManager.Settings.Partial
{
	internal class GeneralSettings
	{
		[Ignore]
		public const int MAX_QUEUE_SIZE_UPPER_LIMIT = 150;

		public virtual string Prefix { get; set; } = "!";
		public virtual bool QueueOpen { get; set; } = true;
		public virtual int MaxQueueSize { get; set; } = 50;
	}
}