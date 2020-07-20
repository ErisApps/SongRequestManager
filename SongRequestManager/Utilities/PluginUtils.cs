using System.Linq;
using IPA.Loader;
using IPA.Utilities;

namespace SongRequestManager.Utilities
{
	internal static class PluginUtils
	{
		public static bool SongBrowserEnabled { get; set; }

		internal static void Setup()
		{
			RegisterPluginChangeListeners();

			SongBrowserEnabled = PluginManager.EnabledPlugins.Any(x => x.Id == "SongBrowser");
		}

		internal static void Cleanup()
		{
			UnregisterPluginChangeListeners();
		}

		internal static void RegisterPluginChangeListeners()
		{
			PluginManager.PluginEnabled -= OnPluginEnabled;
			PluginManager.PluginEnabled += OnPluginEnabled;

			PluginManager.PluginDisabled -= OnPluginDisabled;
			PluginManager.PluginDisabled += OnPluginDisabled;
		}

		internal static void UnregisterPluginChangeListeners()
		{
			PluginManager.PluginEnabled -= OnPluginEnabled;

			PluginManager.PluginDisabled -= OnPluginDisabled;
		}

		private static void OnPluginEnabled(PluginMetadata plugin, bool needsRestart)
		{
			if (needsRestart)
			{
				return;
			}

			switch (plugin.Id)
			{
				case "SongBrowser":
					SongBrowserEnabled = true;
					return;
			}
		}

		private static void OnPluginDisabled(PluginMetadata plugin, bool needsRestart)
		{
			if (needsRestart)
			{
				return;
			}

			switch (plugin.Id)
			{
				case "SongBrowser":
					SongBrowserEnabled = false;
					return;
			}
		}

		public static void DisableSongBrowserFilters()
		{
			if (!SongBrowserEnabled)
			{
				return;
			}

			var songBrowserUi = SongBrowser.SongBrowserApplication.Instance.GetField<SongBrowser.UI.SongBrowserUI, SongBrowser.SongBrowserApplication>("_songBrowserUI");
			if (songBrowserUi)
			{
				if (songBrowserUi.Model.Settings.filterMode != SongBrowser.DataAccess.SongFilterMode.None &&
				    songBrowserUi.Model.Settings.sortMode != SongBrowser.DataAccess.SongSortMode.Original)
				{
					songBrowserUi.CancelFilter();
				}
			}
			else
			{
				Logger.Log("There was a problem obtaining SongBrowserUI object, unable to reset filters", IPA.Logging.Logger.Level.Error);
			}
		}
	}
}