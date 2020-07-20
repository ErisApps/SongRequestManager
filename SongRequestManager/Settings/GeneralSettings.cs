using IPA.Logging;

namespace SongRequestManager.Settings
{
	internal class GeneralSettings
	{
		public virtual string Prefix { get; set; } = "!";
		public virtual bool QueueOpen { get; set; } = true;
		public virtual Logger.LogLevel MinimumLogLevel { get; set; } = Logger.LogLevel.All;
	}
}