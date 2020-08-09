using System;
using System.Linq;
using IPA.Loader;
using IPA.Utilities;
using Zenject;

namespace SongRequestManager.Utilities
{
	public class PluginUtils : IInitializable, IDisposable
	{
		internal bool SongBrowserEnabled { get; set; }

		public void Initialize()
		{
			RegisterPluginChangeListeners();

			SongBrowserEnabled = PluginManager.EnabledPlugins.Any(x => x.Id == "SongBrowser");
		}

		public void Dispose()
		{
			UnregisterPluginChangeListeners();
		}

		private void RegisterPluginChangeListeners()
		{
			PluginManager.PluginEnabled -= OnPluginEnabled;
			PluginManager.PluginEnabled += OnPluginEnabled;

			PluginManager.PluginDisabled -= OnPluginDisabled;
			PluginManager.PluginDisabled += OnPluginDisabled;
		}

		private void UnregisterPluginChangeListeners()
		{
			PluginManager.PluginEnabled -= OnPluginEnabled;

			PluginManager.PluginDisabled -= OnPluginDisabled;
		}

		private void OnPluginEnabled(PluginMetadata plugin, bool needsRestart)
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

		private void OnPluginDisabled(PluginMetadata plugin, bool needsRestart)
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

		internal void DisableSongBrowserFilters()
		{
			if (SongBrowserEnabled)
			{
				DisableSongBrowserFiltersInternal();
			}
		}

		/// <remarks>
		///	Check whether SongBrowser is enabled before invoking this method.
		/// If not, and SongBrowser isn't installed, it will result in a FileNotFoundException.
		/// </remarks>
		private static void DisableSongBrowserFiltersInternal()
		{
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