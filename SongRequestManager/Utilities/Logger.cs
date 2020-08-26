using System.IO;
using System.Runtime.CompilerServices;
using static IPA.Logging.Logger;

namespace SongRequestManager.Utilities
{
	internal static class Logger
	{
		internal static IPA.Logging.Logger? LogInstance { get; set; }

		internal static void Log(string text, Level level = Level.Info, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			LogInstance?.Log(level, $"{Path.GetFileName(file)}->{member}({line}): {text}");
		}
	}
}