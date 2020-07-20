using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.Settings
{
	public class SettingsController : MonoBehaviour
	{
		public static SettingsController Instance;

		public void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);
			}
		}

		[UIValue("prefix-string")]
		public string Prefix
		{
			get => SRMConfig.Instance.GeneralSettings.Prefix;
			set => SRMConfig.Instance.GeneralSettings.Prefix = value;
		}

		[UIValue("loglevel-options")]
		public List<object> LogLevelOptions = new object[]
		{
			IPA.Logging.Logger.LogLevel.All,
			IPA.Logging.Logger.LogLevel.DebugUp,
			IPA.Logging.Logger.LogLevel.InfoUp,
			IPA.Logging.Logger.LogLevel.NoticeUp,
			IPA.Logging.Logger.LogLevel.WarningUp,
			IPA.Logging.Logger.LogLevel.ErrorUp,
			IPA.Logging.Logger.LogLevel.CriticalOnly
		}.ToList();

		[UIValue("loglevel-choice")]
		public IPA.Logging.Logger.LogLevel MinimumLogLevel
		{
			get => SRMConfig.Instance.GeneralSettings.MinimumLogLevel;
			set
			{
				SRMConfig.Instance.GeneralSettings.MinimumLogLevel = value;
			}
		}

		[UIValue("slider-int")]
		public int SliderValue { get; set; }

		[UIValue("checkbox-bool")]
		public bool CheckboxValue { get; set; }

		[UIValue("color-string")]
		public Color ColorPickerValue { get; set; }

		// ============== Twitch ============== //

		[UIValue("TwitchEnabled-bool")]
		public bool TwitchEnabled { get; set; }

		[UIValue("user-request-limit")]
		public int UserRequestLimit { get; set; }

		[UIValue("sub-request-limit")]
		public int SubRequestLimit { get; set; }

		[UIValue("mod-request-limit")]
		public int ModRequestLimit { get; set; }

		[UIValue("vip-bonus-requests")]
		public int VipBonusRequests { get; set; }

		[UIValue("mod-full-rights")]
		public bool TwitchModFullRights { get; set; }

		// ============== Mixer ============== //

		[UIValue("MixerEnabled-bool")]
		public bool MixerEnabled { get; set; }

		[UIAction("#apply")]
		public void OnApply() => Logger.Log($"prefix-string applied, now: {Prefix}");
	}
}