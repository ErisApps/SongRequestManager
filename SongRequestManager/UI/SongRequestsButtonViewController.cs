using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Config.Stores.Attributes;
using IPA.Utilities.Async;
using SongRequestManager.Converters;
using SongRequestManager.Events;
using SongRequestManager.Services;
using UnityEngine;
using Zenject;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	[NotifyPropertyChanges]
	internal class SongRequestsButtonViewController : BSMLAutomaticViewController, IRequestQueueChangeReceiver
	{
		private StandardLevelDetailViewController _standardLevelDetailViewController = null!;
		private LevelSelectionFlowCoordinator _levelSelectionFlowCoordinator = null!;
		private SongQueueService _songQueueService = null!;
		private SongRequestsFlowCoordinator _songRequestsFlowCoordinator = null!;

		[UIValue("glowy-color")]
		public string GlowColor { get; set; } = "#ff0d72";

		[UIValue("interactable")]
		public bool Interactable { get; set; } = true;

		[UIComponent("srm-button")]
		private Transform _srmButtonTransform = null!;

		private IDisposable? _receiverSubscription;

		[UIAction("button-click")]
		internal void OpenRequestsView()
		{
			if (_songRequestsFlowCoordinator == null)
			{
				return;
			}

			_levelSelectionFlowCoordinator.PresentFlowCoordinator(_songRequestsFlowCoordinator);
		}

		[Inject]
		internal void Construct(StandardLevelDetailViewController standardLevelDetailViewController, SoloFreePlayFlowCoordinator levelSelectionFlowCoordinator, SongQueueService songQueueService,
			SongRequestsFlowCoordinator songRequestsFlowCoordinator)
		{
			Logger.Log($"SRMBUTTON construct invoked - Hashcode: {GetHashCode()}");
			_standardLevelDetailViewController = standardLevelDetailViewController;
			_levelSelectionFlowCoordinator = levelSelectionFlowCoordinator;
			_songRequestsFlowCoordinator = songRequestsFlowCoordinator;
			_songQueueService = songQueueService;
		}

		public void Start()
		{
			Logger.Log($"SRMBUTTON init invoked - Hashcode: {GetHashCode()}");
			BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "SongRequestManager.UI.Views.SongRequestsButtonView.bsml"),
				_standardLevelDetailViewController.gameObject, this);
			_srmButtonTransform.localScale *= 0.7f; //no scale property in bsml as of now so manually scaling it

			_receiverSubscription?.Dispose();
			_receiverSubscription = _songQueueService.RegisterReceiver(this);
			UpdateSrmButtonColor();
		}

		protected override void OnDestroy()
		{
			Logger.Log("Destroying SRM button. So yeah... this instance is broken from now on...", IPA.Logging.Logger.Level.Warning);
			_receiverSubscription?.Dispose();
			_receiverSubscription = null;
		}

		public Task Handle(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
					UpdateSrmButtonColor();
					break;
			}

			return Task.CompletedTask;
		}

		private void UpdateSrmButtonColor()
		{
			if (_songQueueService == null)
			{
				Logger.Log("SongQueueService instance null", IPA.Logging.Logger.Level.Warning);
				return;
			}

			GlowColor = ButtonColorValueConverter.Convert(_songQueueService.QueuedRequestCount > 0);
			UnityMainThreadTaskScheduler.Factory.StartNew(() => NotifyPropertyChanged(nameof(GlowColor)));
		}
	}
}