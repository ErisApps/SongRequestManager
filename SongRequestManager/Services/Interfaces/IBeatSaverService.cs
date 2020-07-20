using System;
using System.Threading;
using System.Threading.Tasks;
using BeatSaverSharp;

namespace SongRequestManager.Services.Interfaces
{
	public interface IBeatSaverService : IDisposable
	{
		Task<Beatmap> DownloadSongInfo(string songKey);
		Task<string?> DownloadSong(Beatmap beatMap, CancellationToken? token = null, bool direct = false, IProgress<double> progress = null);
		BeatSaver BeatSaverSharpInstance { get; }
	}
}