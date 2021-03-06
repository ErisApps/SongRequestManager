﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSaverSharp;
using ChatCore.Interfaces;
using IPA.Utilities;
using SongCore;
using SongRequestManager.Extensions;
using SongRequestManager.Models;
using SongRequestManager.Settings;
using SongRequestManager.Settings.Partial;
using SongRequestManager.Utilities;

namespace SongRequestManager.Services
{
	public class SongQueueService
	{
		private readonly BeatSaverService _beatSaverService;
		private readonly StatTrackService _statTrackService;

		internal SongQueueService(BeatSaverService beatSaverService, StatTrackService statTrackService)
		{
			_beatSaverService = beatSaverService;
			_statTrackService = statTrackService;

			RequestQueue = new ObservableCollection<Request>(SRMRequests.Instance.QueueData.Select(x =>
			{
				x.BeatMap.SetProperty("Client", _beatSaverService.BeatSaverSharpInstance);

				return x;
			}));
			RequestQueue.CollectionChanged += RequestQueueOnCollectionChanged;
		}

		internal ObservableCollection<Request> RequestQueue { get; }

		public int QueuedRequestCount => RequestQueue?.Count(r => r.Status == RequestStatus.Queued) ?? 0;

		public bool QueueOpen
		{
			get => SRMConfig.Instance.GeneralSettings.QueueOpen;
			internal set => SRMConfig.Instance.GeneralSettings.QueueOpen = value;
		}

		public bool ToggleQueue() => QueueOpen = !QueueOpen;

		public async Task<(bool, string)> AddRequest(IChatUser requestor, Request request)
		{
			if (!QueueOpen)
			{
				Logger.Log("Q Closed");
				return (false, "Song was not added. Queue is closed.");
			}

			if (SRMConfig.Instance.GeneralSettings.MaxQueueSize <= GeneralSettings.MAX_QUEUE_SIZE_UPPER_LIMIT && RequestQueue.Count >= SRMConfig.Instance.GeneralSettings.MaxQueueSize)
			{
				Logger.Log("Q full");
				return (false, "Song was not added. Queue is full.");
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

			var filterResult = MatchesFilters(beatMap);
			if (!filterResult.Item1)
			{
				return (false, $"Song doesn't match filter. {filterResult.Item2}");
			}

			var currentRequestCount = _statTrackService.GetCurrentRequestCountForUser(requestor.Id);
			var maxConcurrentRequestCount = StatTrackService.GetMaxConcurrentRequestCountForUser(requestor);
			if (maxConcurrentRequestCount != null && currentRequestCount >= maxConcurrentRequestCount)
			{
				return (false, $"Request limit reached. You can only request {maxConcurrentRequestCount} song(s) in total.");
			}

			try
			{
				request.Status = RequestStatus.Queued;
				request.BeatMap = beatMap;
				request.Requestor = Models.User.Create(requestor.Id, requestor.GetPlatform(), requestor.DisplayName);

				using (SRMRequests.Instance.ChangeTransaction)
				{
					RequestQueue.Add(request);
					SRMRequests.Instance.QueueData.Add(request);
					SRMRequests.Instance.Changed();
				}

				_statTrackService.IncreaseRequestCountForUser(request.Requestor);

				Logger.Log("Added request");
			}
			catch (Exception e)
			{
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Error);
				return (false, $"An unknown error occured.");
			}

			return (true, $"{request.BeatMap.Metadata.SongName} - {request.BeatMap.Metadata.SongAuthorName} was added.");
		}

		private (bool, string) MatchesFilters(Beatmap beatmap)
		{
			var filters = SRMConfig.Instance.FilterSettings;
			if (filters.MinimumRating > beatmap.Stats.Rating * 100)
			{
				return (false, $"The rating of the requested song ({beatmap.Stats.Rating * 100:0.00}%) is lower than the minimum allowed rating of {filters.MinimumRating}%");
			}

			// Duration has fallback implemented by calculating average song duration according to the various difficulties
			if (filters.MaximumSongDuration <= FilterSettings.MAX_SONG_DURATION_UPPER_LIMIT
			    && (beatmap.Metadata.Duration > 0 && filters.MaximumSongDuration < beatmap.Metadata.Duration
			        || filters.MaximumSongDuration < beatmap.Metadata
				        .Characteristics
				        .Average(x => x.Difficulties
					        .Where(diff => diff.Value != null)
					        .Average(diff => diff.Value?.Length))))
			{
				return (false, $"The duration of the requested song ({beatmap.Metadata.Duration} seconds) is higher than the maximum allowed duration of {filters.MaximumSongDuration} seconds");
			}

			if (beatmap.Metadata.Characteristics
				.All(x => x.Difficulties
					.Where(diff => diff.Value != null)
					.All(diff => diff.Value?.NoteJumpSpeed < filters.MinimumNjs)))
			{
				return (false, $"The requested song didn't have a single difficulty that matched the minimum required NJS value of {filters.MinimumNjs}");
			}

			return (true, string.Empty);
		}

		public async Task Play(Request request, CancellationToken cancellationToken, IProgress<double>? downloadProgress = null)
		{
			if (!Collections.songWithHashPresent(request.BeatMap.Hash))
			{
				var songPath = await _beatSaverService.DownloadSong(request.BeatMap, cancellationToken, progress: downloadProgress).ConfigureAwait(false);
				if (songPath == null)
				{
					Logger.Log("Something went wrong while trying to download the song. SongPath is null");
					return;
				}

				// Start black magic to force wait on song refresh
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
					Loader.Instance.RefreshSongs(false);
					await semaphoreSlim.WaitAsync(CancellationToken.None);
				}
				catch (Exception e)
				{
					ReleaseSemaphore(null!, null!);
					Console.WriteLine(e);
					throw;
				}
				// Black magic ends here

				const string addedSongToSongCore = "Added song to SongCore";
				Logger.Log(addedSongToSongCore);
			}
			else
			{
				downloadProgress?.Report(1);
			}

			request.Status = RequestStatus.Played;

			MoveRequestToHistory(request);
		}

		public void Skip(Request request)
		{
			request.Status = RequestStatus.Skipped;

			MoveRequestToHistory(request);
		}


		private void MoveRequestToHistory(Request request)
		{
			using (SRMRequests.Instance.ChangeTransaction)
			{
				RequestQueue.Remove(request);
				SRMRequests.Instance.QueueData.Remove(request);
				SRMRequests.Instance.HistoryData.Insert(0, request);
				if (SRMRequests.Instance.HistoryData.Count > 50)
				{
					SRMRequests.Instance.HistoryData.RemoveRange(50, SRMRequests.Instance.HistoryData.Count - 50);
				}
			}

			_statTrackService.DecreaseRequestCountForUser(request.Requestor);
		}

		private void RequestQueueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Logger.Log($"Queue changed: {e.Action}");
		}
	}
}