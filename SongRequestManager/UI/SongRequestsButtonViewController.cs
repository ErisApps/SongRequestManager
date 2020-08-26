using System.Collections.Specialized;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Converters;
using SongRequestManager.Services;
using UnityEngine;
using Zenject;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	[NotifyPropertyChanges]
	public class SongRequestsButtonViewController : BSMLAutomaticViewController
	{
		[UIValue("glowy-color")]
		public string GlowColor { get; set; } = "#ff0d72";

		[UIValue("interactable")]
		public bool Interactable { get; set; } = true;

		[UIComponent("srm-button")]
		private Transform _srmButtonTransform;

		private StandardLevelDetailViewController _standardLevelDetailViewController;
		private LevelSelectionFlowCoordinator _levelSelectionFlowCoordinator;
		private SongQueueService _songQueueService;
		private SongRequestsFlowCoordinator? _songRequestsFlowCoordinator;

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
		protected void Construct(StandardLevelDetailViewController standardLevelDetailViewController, SoloFreePlayFlowCoordinator levelSelectionFlowCoordinator, SongQueueService songQueueService,
			SongRequestsFlowCoordinator songRequestsFlowCoordinator)
		{
			Logger.Log("SRMBUTTON construct invoked");
			_standardLevelDetailViewController = standardLevelDetailViewController;
			_levelSelectionFlowCoordinator = levelSelectionFlowCoordinator;
			_songRequestsFlowCoordinator = songRequestsFlowCoordinator;
			_songQueueService = songQueueService;
		}

		public void Start()
		{
			Logger.Log("SRMBUTTON init invoked");
			var standardLevel = _standardLevelDetailViewController;
			BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "SongRequestManager.UI.Views.SongRequestsButtonView.bsml"),
				standardLevel.transform.Find("LevelDetail").gameObject, this);
			_srmButtonTransform.localScale *= 0.7f; //no scale property in bsml as of now so manually scaling it

			_songQueueService.RequestQueue.CollectionChanged += OnRequestQueueChanged;
			UpdateSrmButtonColor();
		}

		protected override void OnDestroy()
		{
			Logger.Log("GET REKT!!!");
			if (_songQueueService != null)
			{
				_songQueueService.RequestQueue.CollectionChanged -= OnRequestQueueChanged;
			}
		}

		public void Dispose()
		{
		}

		private void OnRequestQueueChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
					UpdateSrmButtonColor();
					break;
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
				default:
					break;
			}
		}

		private void UpdateSrmButtonColor()
		{
			if (_songQueueService == null)
			{
				Logger.Log("SongQueueService instance null", IPA.Logging.Logger.Level.Warning);
				return;
			}

			if (_songQueueService.RequestQueue == null)
			{
				Logger.Log("RequestQueue instance null", IPA.Logging.Logger.Level.Warning);
				return;
			}

			GlowColor = ButtonColorValueConverter.Convert(_songQueueService.QueuedRequestCount > 0);
			NotifyPropertyChanged(nameof(GlowColor));
		}
	}
}