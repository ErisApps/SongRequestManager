using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using SongRequestManager.Settings;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.UI
{
	// The hotreload attribute requires a build of BSML that includes https://github.com/monkeymanboy/BeatSaberMarkupLanguage/pull/43
	[HotReload(RelativePathToLayout = @"Views\SongRequestsSettingsView.bsml")]
	[ViewDefinition("SongRequestManager.UI.Views.SongRequestsSettingsView.bsml")]
	public class SongRequestsSettingsViewController : BSMLAutomaticViewController
	{
		// ============== General ============= //
		[UIValue("command-prefix")]
		public string Prefix
		{
			get => SRMConfig.Instance.GeneralSettings.Prefix;
			set => SRMConfig.Instance.GeneralSettings.Prefix = value;
		}

		[UIValue("max-queue-size")]
		public int MaxQueueSize
		{
			get => SRMConfig.Instance.GeneralSettings.MaxQueueSize;
			set => SRMConfig.Instance.GeneralSettings.MaxQueueSize = value;
		}

		[UIValue("twitch-integration-enabled")]
		public bool TwitchIntegrationEnabled
		{
			get => SRMConfig.Instance.TwitchSettings.Enabled;
			set => SRMConfig.Instance.TwitchSettings.Enabled = value;
		}


		// ============== Twitch ============== //
		[UIValue("user-request-limit")]
		public int UserRequestLimit
		{
			get => SRMConfig.Instance.TwitchSettings.UserRequestLimit;
			set => SRMConfig.Instance.TwitchSettings.UserRequestLimit = value;
		}

		[UIValue("sub-request-limit")]
		public int SubRequestLimit
		{
			get => SRMConfig.Instance.TwitchSettings.SubRequestLimit;
			set => SRMConfig.Instance.TwitchSettings.SubRequestLimit = value;
		}

		[UIValue("mod-request-limit")]
		public int ModRequestLimit
		{
			get => SRMConfig.Instance.TwitchSettings.ModRequestLimit;
			set => SRMConfig.Instance.TwitchSettings.ModRequestLimit = value;
		}

		[UIValue("vip-bonus-limit")]
		public int VipBonusLimit
		{
			get => SRMConfig.Instance.TwitchSettings.VipBonusLimit;
			set => SRMConfig.Instance.TwitchSettings.VipBonusLimit = value;
		}


		// ============== Filters ============= //
		[UIValue("minimum-rating")]
		public int MinimumRating
		{
			get => SRMConfig.Instance.FilterSettings.MinimumRating;
			set => SRMConfig.Instance.FilterSettings.MinimumRating = value;
		}

		[UIValue("maximum-song-duration")]
		public int MaximumSongDuration
		{
			get => SRMConfig.Instance.FilterSettings.MaximumSongDuration;
			set => SRMConfig.Instance.FilterSettings.MaximumSongDuration = value;
		}

		[UIValue("minimum-njs")]
		public int MinimumNjs
		{
			get => SRMConfig.Instance.FilterSettings.MinimumNjs;
			set => SRMConfig.Instance.FilterSettings.MinimumNjs = value;
		}


		// =============== About ============== //
		[UIAction("open-github")]
		public void OpenGitHub()
		{
			Application.OpenURL("https://github.com/ErisApps/SongRequestManager");
		}

		[UIValue("mod-version")]
		public string Version => $"v{Plugin.Version}";


		[UIAction("#post-parse")]
		public void Setup()
		{
			SRMConfig.Instance.ConfigChanged += OnConfigChanged;
		}

		private void OnConfigChanged(object sender, EventArgs e)
		{
			Logger.Log("Config changed externally, hopefully refreshing");
			NotifyPropertyChanged(nameof(Prefix));
			NotifyPropertyChanged(nameof(MaxQueueSize));
			NotifyPropertyChanged(nameof(TwitchIntegrationEnabled));
			NotifyPropertyChanged(nameof(UserRequestLimit));
			NotifyPropertyChanged(nameof(SubRequestLimit));
			NotifyPropertyChanged(nameof(ModRequestLimit));
			NotifyPropertyChanged(nameof(VipBonusLimit));
			NotifyPropertyChanged(nameof(MinimumRating));
			NotifyPropertyChanged(nameof(MaximumSongDuration));
			NotifyPropertyChanged(nameof(MinimumNjs));
		}
	}
}