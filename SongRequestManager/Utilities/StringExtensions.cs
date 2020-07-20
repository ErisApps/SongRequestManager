using System.IO;

namespace SongRequestManager.Utilities
{
	public static class StringExtensions
	{
		public static string SanitizePathForFileSystemUse(this string filename)
		{
			return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
		}
	}
}