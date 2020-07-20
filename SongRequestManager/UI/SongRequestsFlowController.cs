using System;
using System.Linq;
using BeatSaberMarkupLanguage;
using HMUI;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	public class SongRequestsFlowController : FlowCoordinator
	{
		private SongRequestsListViewController? _songRequestsListView;

		public void Awake()
		{
			if (_songRequestsListView == null)
			{
				_songRequestsListView = BeatSaberUI.CreateViewController<SongRequestsListViewController>();
			}
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
			Resources.FindObjectsOfTypeAll<LevelSelectionFlowCoordinator>()
				.First()
				.DismissFlowCoordinator(this);
		}

		public void Dismiss() => BackButtonWasPressed(null!);
	}
}