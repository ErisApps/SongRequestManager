using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using SongRequestManager.Settings;
using SongRequestManager.Settings.Partial;
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

		[UIValue("max-queue-size-upper-limit")]
		public int MaxQueueSizeUpperLimit => GeneralSettings.MAX_QUEUE_SIZE_UPPER_LIMIT + 1;

		[UIAction("max-queue-size-formatter")]
		public string MaxQueueSizeFormat(int queueSize)
		{
			return FormatSliderUpperLimit(MaxQueueSizeUpperLimit, queueSize);
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

		[UIValue("user-request-upper-limit")]
		public int UserRequestUpperLimit => TwitchSettings.USER_REQUEST_UPPER_LIMIT + 1;

		[UIAction("user-request-formatter")]
		public string UserRequestFormat(int limit)
		{
			return FormatSliderUpperLimit(UserRequestUpperLimit, limit);
		}


		[UIValue("sub-request-limit")]
		public int SubRequestLimit
		{
			get => SRMConfig.Instance.TwitchSettings.SubRequestLimit;
			set => SRMConfig.Instance.TwitchSettings.SubRequestLimit = value;
		}

		[UIValue("sub-request-upper-limit")]
		public int SubRequestUpperLimit => TwitchSettings.SUB_REQUEST_UPPER_LIMIT + 1;

		[UIAction("sub-request-formatter")]
		public string SubRequestFormat(int limit)
		{
			return FormatSliderUpperLimit(SubRequestUpperLimit, limit);
		}


		[UIValue("mod-request-limit")]
		public int ModRequestLimit
		{
			get => SRMConfig.Instance.TwitchSettings.ModRequestLimit;
			set => SRMConfig.Instance.TwitchSettings.ModRequestLimit = value;
		}

		[UIValue("mod-request-upper-limit")]
		public int ModRequestUpperLimit => TwitchSettings.MOD_REQUEST_UPPER_LIMIT + 1;

		[UIAction("mod-request-formatter")]
		public string ModRequestFormat(int limit)
		{
			return FormatSliderUpperLimit(ModRequestUpperLimit, limit);
		}


		[UIValue("vip-bonus-limit")]
		public int VipBonusLimit
		{
			get => SRMConfig.Instance.TwitchSettings.VipBonusLimit;
			set => SRMConfig.Instance.TwitchSettings.VipBonusLimit = value;
		}

		[UIValue("vip-bonus-upper-limit")]
		public int VipBonusUpperLimit => TwitchSettings.VIP_BONUS_UPPER_LIMIT + 1;

		[UIAction("vip-bonus-formatter")]
		public string VipBonusFormat(int limit)
		{
			return FormatSliderUpperLimit(VipBonusUpperLimit, limit);
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

		[UIValue("max-song-duration-upper-limit")]
		public int MaximumSongDurationUpperLimit => FilterSettings.MAX_SONG_DURATION_UPPER_LIMIT + 1;

		[UIAction("max-song-duration-formatter")]
		public string MaximumSongDurationFormat(int maxDuration)
		{
			return FormatSliderUpperLimit(MaximumSongDurationUpperLimit, maxDuration);
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


		private static string FormatSliderUpperLimit(int upperLimit, int value)
		{
			return value >= upperLimit ? "Unlimited" : value.ToString();
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