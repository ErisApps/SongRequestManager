using System;
using System.Linq;

namespace SongRequestManager.Utilities
{
	public static class SongCoreUtils
	{
		public static CustomPreviewBeatmapLevel? CustomLevelForHash(string hash)
		{
			// get level id from hash
			var levelIds = SongCore.Collections.levelIDsForHash(hash);
			if (levelIds.Count == 0)
			{
				return null;
			}

			// lookup song from level id
			return SongCore.Loader.CustomLevels.FirstOrDefault(s => string.Equals(s.Value.levelID, levelIds.First(), StringComparison.OrdinalIgnoreCase)).Value ?? null;
		}
	}
}