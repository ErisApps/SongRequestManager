using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IPA.Utilities;
using SongCore;
using SongRequestManager.Models;
using SongRequestManager.Services.Interfaces;
using SongRequestManager.Settings;
using SongRequestManager.Utilities;

namespace SongRequestManager.Services
{
	public class SongQueueService : ISongQueueService
	{
		private readonly IBeatSaverService _beatSaverService;

		public SongQueueService(IBeatSaverService beatSaverService)
		{
			_beatSaverService = beatSaverService;
			RequestQueue = new ObservableCollection<Request>(SRMConfig.Instance.QueueData.Select(x =>
			{
				x.BeatMap.SetProperty("Client", _beatSaverService.BeatSaverSharpInstance);

				return x;
			}));
			RequestQueue.CollectionChanged += RequestQueueOnCollectionChanged;
		}

		public ObservableCollection<Request> RequestQueue { get; }

		public int QueuedRequestCount => RequestQueue.Count(r => r.Status == RequestStatus.Queued);

		public bool QueueOpen
		{
			get => SRMConfig.Instance.GeneralSettings.QueueOpen;
			private set => SRMConfig.Instance.GeneralSettings.QueueOpen = value;
		}

		public bool ToggleQueue() => QueueOpen = !QueueOpen;

		public async Task<ValueTuple<bool, string>> AddRequest(Request request)
		{
			if (!QueueOpen)
			{
				Logger.Log("Q Closed");
				return (false, "Song was not added. Queue is closed.");
			}

			if (RequestQueue.Any(x => x.BeatSaverKey == request.BeatSaverKey))
			{
				Logger.Log("Song with id already in Q");
				return (false, "Song was not added. Song with this key is already in the queue.");
			}

			var beatMap = await _beatSaverService.DownloadSongInfo(request.BeatSaverKey);
			if (beatMap == null)
			{
				const string invalidBsrMessage = "Song was not added. Couldn't find it on BeatSaver. Make sure to copy/use the correct key";
				Logger.Log(invalidBsrMessage);

				return (false, invalidBsrMessage);
			}

			try
			{
				request.Status = RequestStatus.Queued;
				request.BeatMap = beatMap;

				RequestQueue.Add(request);
				SRMConfig.Instance.QueueData.Add(request);
				SRMConfig.Instance.Changed();

				Logger.Log("Added request");
			}
			catch (Exception e)
			{
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Error);
				return (false, $"An unknown error occured.");
			}

			return (true, $"{request.BeatMap.Metadata.SongName} - {request.BeatMap.Metadata.SongAuthorName} was added.");
		}

		public async Task Play(Request request, CancellationToken cancellationToken, IProgress<double> downloadProgress = null)
		{
			if (!Collections.songWithHashPresent(request.BeatMap.Hash))
			{
				var songPath = await _beatSaverService.DownloadSong(request.BeatMap, cancellationToken, progress: downloadProgress).ConfigureAwait(false);
				if (songPath == null)
				{
					Logger.Log("Something went wrong while trying to download the song. SongPath is null");
					return;
				}

				var semaphoreSlim = new SemaphoreSlim(0, 1);

				void ReleaseSemaphore(Loader _, Dictionary<string, CustomPreviewBeatmapLevel> __)
				{
					Loader.SongsLoadedEvent -= ReleaseSemaphore;
					semaphoreSlim?.Release();
				}

				try
				{
					Loader.SongsLoadedEvent += ReleaseSemaphore;

					Collections.AddSong(request.BeatMap.ID, songPath);
					Loader.Instance.RefreshSongs();
					await semaphoreSlim.WaitAsync(CancellationToken.None);
				}
				catch (Exception e)
				{
					ReleaseSemaphore(null!, null!);
					Console.WriteLine(e);
					throw;
				}

				const string addedSongToSongCore = "Added song to SongCore";
				// chatService.SendTextMessage(addedSongToSongCore, chatMessage.Channel);
				Logger.Log(addedSongToSongCore);
			}
			else
			{
				downloadProgress.Report(1);
			}

			request.Status = RequestStatus.Played;

			RequestQueue.Remove(request);
			SRMConfig.Instance.QueueData.Remove(request);
			SRMConfig.Instance.HistoryData.Insert(0, request);
			if (SRMConfig.Instance.HistoryData.Count > 50)
			{
				SRMConfig.Instance.HistoryData.RemoveRange(50, SRMConfig.Instance.HistoryData.Count - 50);
			}

			SRMConfig.Instance.Changed();
		}

		private void RequestQueueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Logger.Log($"Queue changed: {e.Action}");
		}
	}
}