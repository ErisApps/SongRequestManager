using System;
using BeatSaberMarkupLanguage;
using HMUI;
using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.UI
{
	internal class SongRequestsFlowCoordinator : FlowCoordinator
	{
		private SongRequestsListViewController? _songRequestsListViewController;
		private SongRequestsSettingsViewController? _songRequestsSettingsViewController;
		private LevelSelectionFlowCoordinator? _levelSelectionFlowCoordinator;

		[Inject]
		internal void Construct(SongRequestsListViewController songRequestsListViewController, SongRequestsSettingsViewController songRequestsSettingsViewController, SoloFreePlayFlowCoordinator levelSelectionFlowCoordinator)
		{
			_songRequestsSettingsViewController = songRequestsSettingsViewController;
			_songRequestsListViewController = songRequestsListViewController;
			_levelSelectionFlowCoordinator = levelSelectionFlowCoordinator;
		}

		private void Start()
		{
			if (_songRequestsListViewController == null)
			{
				Logger.Log($"{nameof(_songRequestsListViewController)} is null... which is... erm... not allowed I guess?", IPA.Logging.Logger.Level.Warning);
				return;
			}

			_songRequestsListViewController.DismissRequested -= SongRequestsListViewControllerOnDismissRequested;
			_songRequestsListViewController.DismissRequested += SongRequestsListViewControllerOnDismissRequested;
		}

		protected override void DidActivate(bool firstActivation, ActivationType activationType)
		{
			try
			{
				if (firstActivation)
				{
					title = "Song requests";
					showBackButton = true;
					ProvideInitialViewControllers(_songRequestsListViewController, _songRequestsSettingsViewController);
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString(), IPA.Logging.Logger.Level.Error);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			// Dismiss ourselves
			_levelSelectionFlowCoordinator.DismissFlowCoordinator(this);
		}

		private void OnDestroy()
		{
			if (_songRequestsListViewController == null)
			{
				Logger.Log($"{nameof(_songRequestsListViewController)} is null anyways, so yeah... what do you expect me to do?", IPA.Logging.Logger.Level.Warning);
				return;
			}

			_songRequestsListViewController.DismissRequested -= SongRequestsListViewControllerOnDismissRequested;
		}

		private void SongRequestsListViewControllerOnDismissRequested()
		{
			BackButtonWasPressed(null!);
		}
	}
}