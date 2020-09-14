using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using IPA.Utilities.Async;
using SongRequestManager.Converters;
using SongRequestManager.Events;
using SongRequestManager.Models;
using SongRequestManager.Services;
using SongRequestManager.Settings;
using SongRequestManager.Utilities;
using UnityEngine;
using Zenject;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	[HotReload(RelativePathToLayout = @"Views\SongRequestsListView.bsml")]
	[ViewDefinition("SongRequestManager.UI.Views.SongRequestsListView.bsml")]
	internal class SongRequestsListViewController : BSMLAutomaticViewController, IRequestQueueChangeReceiver
	{
		private SongQueueService _songQueueService = null!;
		private SongListUtils _songListUtils = null!;
		private LoadingProgressModal.LoadingProgressModal _loadingProgressModal = null!;

		private Request? _selectedRequest;

		private IDisposable? _receiverSubscription;

		public event Action DismissRequested = null!;

		[Inject]
		internal void Construct(SongQueueService songQueueService, SongListUtils songListUtils, LoadingProgressModal.LoadingProgressModal loadingProgressModal)
		{
			_loadingProgressModal = loadingProgressModal;
			_songQueueService = songQueueService;
			_songListUtils = songListUtils;
		}

		[UIComponent("requestsList")]
		public CustomListTableData? customListTableData;

		[UIAction("selectRequest")]
		internal void Select(TableView _, int row)
		{
			_selectedRequest = _songQueueService.RequestQueue[row];
			NotifyPropertyChanged(nameof(IsRequestSelected));
		}

		[UIValue("is-request-selected")]
		internal bool IsRequestSelected => _selectedRequest != null;

		[UIAction("skip-button-click")]
		internal void Skip()
		{
			if (_selectedRequest == null)
			{
				return;
			}

			_songQueueService.Skip(_selectedRequest);

			_selectedRequest = null!;
			NotifyPropertyChanged(nameof(IsRequestSelected));
		}

		[UIAction("play-button-click")]
		internal async Task Play()
		{
			if (_selectedRequest == null)
			{
				return;
			}

			try
			{
				// Prepare possible download task
				var cts = new CancellationTokenSource();
				cts.Token.ThrowIfCancellationRequested();
				var progress = new Progress<double>();
				_loadingProgressModal.ShowDialog(gameObject, progress, () => cts.Cancel());

				// Download song if required and mark as "played"
				await _songQueueService.Play(_selectedRequest, cts.Token, progress).ConfigureAwait(false);

				await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
				{
					// Select song in list
					StartCoroutine(_songListUtils.ScrollToLevel(_selectedRequest.BeatMap.Hash.ToUpper(), b => { }, true));

					// Navigate back
					DismissRequested!.Invoke();
				}, cts.Token).ConfigureAwait(false);

				_selectedRequest = null!;
				NotifyPropertyChanged(nameof(IsRequestSelected));
			}
			catch (OperationCanceledException)
			{
				_loadingProgressModal.HideDialog();
				// NOP, expected behaviour due to token cancellation... I hope...
			}
			catch (Exception e)
			{
				_loadingProgressModal.HideDialog();
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Error);
			}
		}

		[UIValue("queue-button-text")]
		internal string QueueButtonText { get; set; } = string.Empty;

		[UIValue("queue-button-color-face")]
		internal string QueueButtonColorFace { get; set; } = ButtonColorValueConverter.Convert(true); // #dd2266

		[UIValue("queue-button-color-glow")]
		internal string QueueButtonColorGlow { get; set; } = ButtonColorValueConverter.Convert(true); // #dd2266

		[UIValue("queue-button-color-stroke")]
		internal string QueueButtonColorStroke { get; set; } = ButtonColorValueConverter.Convert(true); // #dd2266

		[UIAction("queue-button-click")]
		internal void ToggleQueueState()
		{
			var newState = _songQueueService.ToggleQueue();
			Logger.Log($"Queue state button clicked. Open: {newState.ToString()}");

			SetQueueButtonState();
		}

		[UIAction("#post-parse")]
		public async Task Setup()
		{
			SetQueueButtonState();

			SRMConfig.Instance.ConfigChanged += OnConfigChanged;

			if (customListTableData == null)
			{
				Logger.Log("SETUP => SKIPPED");

				return;
			}

			customListTableData.data.Clear();
			var thumbnailLoadingTasks = new List<Task>(_songQueueService.RequestQueue.Count);
			foreach (var request in _songQueueService.RequestQueue)
			{
				var cellInfo = ConvertToCellInfo(request);
				customListTableData.data.Add(cellInfo);

				thumbnailLoadingTasks.Add(UnityMainThreadTaskScheduler.Factory.StartNew(async () => await LoadThumbnailAsync(cellInfo, request)));
			}

			_receiverSubscription?.Dispose();
			_receiverSubscription = _songQueueService.RegisterReceiver(this);

			await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.ReloadData());

			await Task.WhenAll(thumbnailLoadingTasks).ConfigureAwait(false);
			await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.RefreshCells(true, true));

			Logger.Log("SETUP => Finished");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SRMConfig.Instance.ConfigChanged -= OnConfigChanged;

			_receiverSubscription?.Dispose();
		}

		private void OnConfigChanged(object sender, EventArgs e)
		{
			SetQueueButtonState();
		}

		private void SetQueueButtonState()
		{
			var queueOpen = _songQueueService.QueueOpen;
			QueueButtonColorFace = QueueButtonColorGlow = QueueButtonColorStroke = ButtonColorValueConverter.Convert(queueOpen);
			QueueButtonText = queueOpen ? "Queue Open" : "Queue Closed";

			NotifyPropertyChanged(nameof(QueueButtonColorFace));
			NotifyPropertyChanged(nameof(QueueButtonColorGlow));
			NotifyPropertyChanged(nameof(QueueButtonColorStroke));
			NotifyPropertyChanged(nameof(QueueButtonText));
		}

		public async Task Handle(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this == null)
			{
				Logger.Log("This is null", IPA.Logging.Logger.Level.Error);
				return;
			}

			if (customListTableData == null)
			{
				Logger.Log("customListTableData is null", IPA.Logging.Logger.Level.Error);
				return;
			}

			try
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						var addRequest = _songQueueService.RequestQueue[e.NewStartingIndex];
						var addCellInfo = ConvertToCellInfo(addRequest);
						await LoadThumbnailAsync(addCellInfo, addRequest).ConfigureAwait(false);
						customListTableData.data.Insert(e.NewStartingIndex, addCellInfo);
						break;
					case NotifyCollectionChangedAction.Remove:
						customListTableData.data.RemoveAt(e.OldStartingIndex);
						break;
					case NotifyCollectionChangedAction.Replace:
						var replaceRequest = _songQueueService.RequestQueue[e.OldStartingIndex];
						var replaceCellInfo = ConvertToCellInfo(replaceRequest);
						await LoadThumbnailAsync(replaceCellInfo, replaceRequest).ConfigureAwait(false);
						customListTableData.data[e.OldStartingIndex] = replaceCellInfo;
						break;
					default:
						return;
				}

				await UnityMainThreadTaskScheduler.Factory.StartNew(() => customListTableData.tableView.ReloadData());
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
					await UnityMainThreadTaskScheduler.Factory.StartNew(async () => cellInfo.icon = await level.GetCoverImageTexture2DAsync(CancellationToken.None)).ConfigureAwait(false);
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

	// The hotreload attribute requires a build of BSML that includes https://github.com/monkeymanboy/BeatSaberMarkupLanguage/pull/43
}