using IPA.Config.Stores.Attributes;
using IPA.Logging;
using SongRequestManager.Settings.Converters;

namespace SongRequestManager.Settings
{
	internal class GeneralSettings
	{
		public virtual string Prefix { get; set; } = "!";
		public virtual bool QueueOpen { get; set; } = true;
		[UseConverter(typeof(EnumStringConfigConverter<Logger.LogLevel>))]
		public virtual Logger.LogLevel MinimumLogLevel { get; set; } = Logger.LogLevel.All;
	}
}