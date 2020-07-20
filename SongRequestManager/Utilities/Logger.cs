using System.IO;
using System.Runtime.CompilerServices;
using SongRequestManager.Settings;
using static IPA.Logging.Logger;

namespace SongRequestManager.Utilities
{
	internal static class Logger
	{
		internal static IPA.Logging.Logger? LogInstance { get; set; }

		internal static void Log(string text, Level level = Level.Info, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			if (SRMConfig.Instance.GeneralSettings.MinimumLogLevel.HasFlag((LogLevel) level))
			{
				LogInstance?.Info($"{Path.GetFileName(file)}->{member}({line}): {text}");
			}
		}
	}
}