using System;
using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	public class SongRequestsFlowCoordinator : FlowCoordinator
	{
		private SongRequestsListViewController? _songRequestsListView;
		private LevelSelectionFlowCoordinator _levelSelectionFlowCoordinator;

		[Inject]
		protected void Construct(SongRequestsListViewController songRequestsListViewController, SoloFreePlayFlowCoordinator levelSelectionFlowCoordinator)
		{
			_levelSelectionFlowCoordinator = levelSelectionFlowCoordinator;
			_songRequestsListView = songRequestsListViewController;
		}

		private void Start()
		{
			if (_songRequestsListView == null)
			{
				Logger.Log($"{nameof(_songRequestsListView)} is null... which is... erm... not allowed I guess?", IPA.Logging.Logger.Level.Warning);
				return;
			}

			_songRequestsListView.DismissRequested -= SongRequestsListViewOnDismissRequested;
			_songRequestsListView.DismissRequested += SongRequestsListViewOnDismissRequested;
		}

		protected override void DidActivate(bool firstActivation, ActivationType activationType)
		{
			try
			{
				if (firstActivation)
				{
					title = "Song requests";
					showBackButton = true;
					ProvideInitialViewControllers(_songRequestsListView);
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString(), IPA.Logging.Logger.Level.Error);
			}
		}

		protected override void BackButtonWasPressed(ViewController topViewController)
		{
			// Dismiss ourselves
			_levelSelectionFlowCoordinator.DismissFlowCoordinator(this);
		}

		private void OnDestroy()
		{
			if (_songRequestsListView == null)
			{
				Logger.Log($"{nameof(_songRequestsListView)} is null anyways, so yeah... what do you expect me to do?", IPA.Logging.Logger.Level.Warning);
				return;
			}

			_songRequestsListView.DismissRequested -= SongRequestsListViewOnDismissRequested;
		}

		private void SongRequestsListViewOnDismissRequested()
		{
			BackButtonWasPressed(null!);
		}
	}
}