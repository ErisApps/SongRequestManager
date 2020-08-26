using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Notify;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.Settings.UI
{
	public class SettingsController : INotifiableHost
	{
		[UIValue("prefix-string")]
		public string Prefix
		{
			get => SRMConfig.Instance.GeneralSettings.Prefix;
			set => SRMConfig.Instance.GeneralSettings.Prefix = value;
		}

		// ============== Twitch ============== //

		[UIValue("twitchenabled-bool")]
		public bool TwitchEnabled
		{
			get => SRMConfig.Instance.TwitchSettings.Enabled;
			set => SRMConfig.Instance.TwitchSettings.Enabled = value;
		}

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

		[UIAction("#apply")]
		public void OnApply() => Logger.Log($"prefix-string applied, now: {Prefix}");

		public event PropertyChangedEventHandler PropertyChanged;
	}
}