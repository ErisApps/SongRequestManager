using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using IPA.Logging;

namespace SongRequestManager.Settings.Partial
{
	internal class GeneralSettings
	{
		public virtual string Prefix { get; set; } = "!";
		public virtual bool QueueOpen { get; set; } = true;
		[UseConverter(typeof(EnumConverter<Logger.LogLevel>))]
		public virtual Logger.LogLevel MinimumLogLevel { get; set; } = Logger.LogLevel.All;
	}
}