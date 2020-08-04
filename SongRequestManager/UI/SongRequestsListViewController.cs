using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities.Async;
using SongRequestManager.Converters;
using SongRequestManager.Models;
using SongRequestManager.Utilities;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	[HotReload]
	public class SongRequestsListViewController : BSMLAutomaticViewController
	{
		private Request SelectedRequest;

		public SongRequestsListViewController()
		{
			SetQueueButtonState();
		}

		[UIComponent("requestsList")]
		public CustomListTableData? customListTableData;

		[UIAction("selectRequest")]
		public void Select(TableView _, int row)
		{
			SelectedRequest = SongRequestManager.Instance.SongQueueService.RequestQueue[row];
			NotifyPropertyChanged(nameof(IsRequestSelected));
		}

		[UIValue("is-request-selected")]
		public bool IsRequestSelected => SelectedRequest != null;

		[UIAction("skip-button-click")]
		internal void Skip()
		{
			SongRequestManager.Instance.SongQueueService.Skip(SelectedRequest);
		}

		[UIAction("play-button-click")]
		internal async Task Play()
		{
			if (SelectedRequest == null)
			{
				return;
			}

			try
			{
				// Prepare possible download task
				var cts = new CancellationTokenSource();
				cts.Token.ThrowIfCancellationRequested();
				var progress = new Progress<double>();
				LoadingProgressModal.LoadingProgressModal.instance.ShowDialog(gameObject, progress, () => cts.Cancel());

				// Download song if required and mark as "played"
				await SongRequestManager.Instance.SongQueueService.Play(SelectedRequest, cts.Token, progress).ConfigureAwait(false);

				await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
				{
					// Select song in list
					StartCoroutine(SongListUtils.ScrollToLevel(SelectedRequest.BeatMap.Hash.ToUpper(), b => { }, true));

					// Navigate back
					Resources.FindObjectsOfTypeAll<SongRequestsFlowController>().FirstOrDefault().Dismiss();
				}, cts.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				LoadingProgressModal.LoadingProgressModal.instance.HideDialog();
				// NOP, expected behaviour due to token cancellation... I hope...
			}
			catch (Exception e)
			{
				LoadingProgressModal.LoadingProgressModal.instance.HideDialog();
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Error);
			}
		}

		[UIValue("queue-button-text")]
		internal string QueueButtonText { get; set; }

		[UIValue("queue-button-color-face")]
		internal string QueueButtonColorFace { get; set; } // #dd2266

		[UIValue("queue-button-color-glow")]
		internal string QueueButtonColorGlow { get; set; } // #dd2266

		[UIValue("queue-button-color-stroke")]
		internal string QueueButtonColorStroke { get; set; } // #dd2266

		[UIAction("queue-button-click")]
		internal void ToggleQueueState()
		{
			var newState = SongRequestManager.Instance.SongQueueService.ToggleQueue();
			Logger.Log($"Queue state button clicked. Open: {newState.ToString()}");

			SetQueueButtonState();
		}

		[UIAction("#post-parse")]
		public async Task Setup()
		{
			if (customListTableData == null)
			{
				Logger.Log("SETUP => SKIPPED");

				return;
			}

			customListTableData.data.Clear();
			var thumbnailLoadingTasks = new List<Task>(SongRequestManager.Instance.SongQueueService.RequestQueue.Count);
			foreach (var request in SongRequestManager.Instance.SongQueueService.RequestQueue)
			{
				var cellInfo = ConvertToCellInfo(request);
				customListTableData.data.Add(cellInfo);

				thumbnailLoadingTasks.Add(await UnityMainThreadTaskScheduler.Factory.StartNew(() => LoadThumbnailAsync(cellInfo, request)));
			}

			SongRequestManager.Instance.SongQueueService.RequestQueue.CollectionChanged += RequestQueueOnCollectionChanged;

			await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.ReloadData());

			await Task.WhenAll(thumbnailLoadingTasks).ConfigureAwait(false);
			await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.RefreshCells(true, true));

			Logger.Log("SETUP => Finished");
		}

		private void SetQueueButtonState()
		{
			var queueOpen = SongRequestManager.Instance.SongQueueService.QueueOpen;
			QueueButtonColorFace = QueueButtonColorGlow = QueueButtonColorStroke = ButtonColorValueConverter.Convert(queueOpen);
			QueueButtonText = queueOpen ? "Queue Open" : "Queue Closed";

			NotifyPropertyChanged(nameof(QueueButtonColorFace));
			NotifyPropertyChanged(nameof(QueueButtonColorGlow));
			NotifyPropertyChanged(nameof(QueueButtonColorStroke));
			NotifyPropertyChanged(nameof(QueueButtonText));
		}

		private async void RequestQueueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (customListTableData == null)
			{
				Logger.Log("Whoops, I did a fucky wucky :tehe:", IPA.Logging.Logger.Level.Error);
				return;
			}

			try
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						var addRequest = SongRequestManager.Instance.SongQueueService.RequestQueue[e.NewStartingIndex];
						var addCellInfo = ConvertToCellInfo(addRequest);
						await LoadThumbnailAsync(addCellInfo, addRequest).ConfigureAwait(false);
						customListTableData.data.Insert(e.NewStartingIndex, addCellInfo);
						break;
					case NotifyCollectionChangedAction.Remove:
						customListTableData.data.RemoveAt(e.OldStartingIndex);
						break;
					case NotifyCollectionChangedAction.Replace:
						var replaceRequest = SongRequestManager.Instance.SongQueueService.RequestQueue[e.OldStartingIndex];
						var replaceCellInfo = ConvertToCellInfo(replaceRequest);
						await LoadThumbnailAsync(replaceCellInfo, replaceRequest).ConfigureAwait(false);
						customListTableData.data[e.OldStartingIndex] = replaceCellInfo;
						break;
					case NotifyCollectionChangedAction.Move:
						return;
					case NotifyCollectionChangedAction.Reset:
						return;
					default:
						return;
				}

				await UnityMainThreadTaskScheduler.Factory.StartNew(() => { customListTableData.tableView.ReloadData(); });
			}
			catch (Exception exception)
			{
				Logger.Log(exception.ToString(), IPA.Logging.Logger.Level.Error);
			}
		}

		private readonly Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

		private async Task LoadThumbnailAsync(CustomListTableData.CustomCellInfo cellInfo, Request request)
		{
			if (SongCore.Loader.AreSongsLoaded)
			{
				var level = SongCoreUtils.CustomLevelForHash(request.BeatMap.Hash);
				if (level != null)
				{
					await UnityMainThreadTaskScheduler.Factory.StartNew(async () => cellInfo.icon = await level.GetCoverImageTexture2DAsync(CancellationToken.None));
					return;
				}
			}

			if (!_cachedTextures.TryGetValue(request.BeatMap.Hash, out var texture))
			{
				try
				{
					Logger.Log($"Downloading cover for {request.BeatMap.Key}", IPA.Logging.Logger.Level.Trace);
					var coverImageByteData = await request.BeatMap.FetchCoverImage().ConfigureAwait(false);
					Logger.Log($"Downloaded cover for {request.BeatMap.Key}", IPA.Logging.Logger.Level.Trace);

					await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
					{
						texture = new Texture2D(2, 2);
						texture.LoadImage(coverImageByteData);
					});

					_cachedTextures.Add(request.BeatMap.Hash, texture);
				}
				catch (Exception exception)
				{
					// NOP
					Logger.Log(exception.ToString(), IPA.Logging.Logger.Level.Error);
				}
			}

			cellInfo.icon = texture;
		}

		private static CustomListTableData.CustomCellInfo ConvertToCellInfo(Request request)
			=> new CustomListTableData.CustomCellInfo(request.BeatMap.Metadata.SongName, request.BeatMap.Metadata.SongAuthorName);
	}
}