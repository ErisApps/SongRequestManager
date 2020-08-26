using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using BeatSaverSharp;
using SongRequestManager.Utilities;

namespace SongRequestManager.Services
{
	public sealed class BeatSaverService
	{
		public BeatSaver BeatSaverSharpInstance { get; private set; }

		public BeatSaverService()
		{
			var version = Plugin.Version;

			// BeatSaverSharp init
			var options = new HttpOptions
			{
				ApplicationName = Plugin.Name,
				Version = new Version(version.Major, version.Minor, version.Patch),
				HandleRateLimits = true
			};

			// Use this to interact with the API
			BeatSaverSharpInstance = new BeatSaver(options);
		}

		public Task<Beatmap> DownloadSongInfo(string songKey)
		{
			return BeatSaverSharpInstance.Key(songKey);
		}

		public async Task<string?> DownloadSong(Beatmap beatMap, CancellationToken? token = null, bool direct = false, IProgress<double> progress = null)
		{
			// TODO: Will probably still error?
			var beatMapZipStream = await (token.HasValue ? beatMap.DownloadZip(direct, token.Value, progress) : beatMap.DownloadZip(direct, progress)).ConfigureAwait(false);

			try
			{
				using var memoryStream = new MemoryStream(beatMapZipStream);
				using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
				var beatMapFolder = $"{beatMap.Key} ({beatMap.Metadata.SongName} - {beatMap.Metadata.LevelAuthorName}".SanitizePathForFileSystemUse();
				var path = Path.Combine(CustomLevelPathHelper.customLevelsDirectoryPath, beatMapFolder);
				if (Directory.Exists(path))
				{
					// What the heck am I even doing here then...
					return null;
				}

				Directory.CreateDirectory(path);
				
				await Task.Factory.StartNew(() => zipArchive.ExtractToDirectory(path));

				return path;
			}
			catch (Exception e)
			{
				Logger.Log($"Unable to extract ZIP! Exception: {e}");
				return null;
			}
		}

		public void Dispose()
		{
			BeatSaverSharpInstance = null!;
		}
	}
}