﻿using System;
using System.Collections;
using System.Linq;
using HMUI;
using IPA.Utilities;
using UnityEngine;

namespace SongRequestManager.Utilities
{
	/// <summary>
	/// Pretty much yeeted from the original SRM
	/// https://github.com/angturil/SongRequestManager/blob/dev/EnhancedTwitchIntegration/UI/SongListUtils.cs
	///
	/// Marked as "Performance critical sections" so might need some further optimizations?
	/// </summary>
	public static class SongListUtils
	{
		public static IEnumerator ScrollToLevel(string levelId, Action<bool> callback, bool animated)
		{
			var levelCollectionViewController = Resources.FindObjectsOfTypeAll<LevelCollectionViewController>().FirstOrDefault();

			if (levelCollectionViewController)
			{
				Logger.Log($"Scrolling to {levelId}!");

				// Disable SongBrowser filters as that might prevent us from finding the song in the list
				if (PluginUtils.SongBrowserEnabled)
				{
					PluginUtils.DisableSongBrowserFilters();
				}

				// Make sure our custom songPack is selected
				SelectCustomSongPack();

				yield return null;

				// get the table view
				var levelsTableView = levelCollectionViewController.GetField<LevelCollectionTableView, LevelCollectionViewController>("_levelCollectionTableView");

				yield return null;

				// get the table view
				var tableView = levelsTableView.GetField<TableView, LevelCollectionTableView>("_tableView");

				// get list of beatMaps, this is pre-sorted, etc
				var beatMaps = levelsTableView.GetField<IPreviewBeatmapLevel[], LevelCollectionTableView>("_previewBeatmapLevels").ToList();

				// get the row number for the song we want
				var songIndex = beatMaps.FindIndex(x => (x.levelID.Split('_')[2] == levelId));

				// bail if song is not found, shouldn't happen
				if (songIndex >= 0)
				{
					// if header is being shown, increment row
					if (levelsTableView.GetField<bool, LevelCollectionTableView>("_showLevelPackHeader"))
					{
						songIndex++;
					}

					Logger.Log($"Selecting row {songIndex}");

					// scroll to song
					tableView.ScrollToCellWithIdx(songIndex, TableViewScroller.ScrollPositionType.Beginning, animated);

					// select song, and fire the event
					tableView.SelectCellWithIdx(songIndex, true);

					Logger.Log("Selected song with index " + songIndex);
					callback?.Invoke(true);

					yield break;
				}
			}

			Logger.Log($"Failed to scroll to {levelId}!");
			callback?.Invoke(false);
		}

		private static void SelectCustomSongPack()
		{
			// get the Level Filtering Nav Controller, the top bar
			var levelFilteringNavigationController = Resources.FindObjectsOfTypeAll<LevelFilteringNavigationController>().First();

			// get the tab bar
			var tabBarViewController = levelFilteringNavigationController.GetField<TabBarViewController, LevelFilteringNavigationController>("_tabBarViewController");

			if (tabBarViewController.selectedCellNumber != 3)
			{
				// select the 4th item, which is custom songs
				tabBarViewController.SelectItem(3);

				// trigger a switch and reload
				levelFilteringNavigationController.InvokeMethod<object, LevelFilteringNavigationController>("TabBarDidSwitch");
			}
			else
			{
				// get the annotated view controller
				var annotatedBeatMapLevelCollectionsViewController =
					levelFilteringNavigationController.GetField<AnnotatedBeatmapLevelCollectionsViewController, LevelFilteringNavigationController>("_annotatedBeatmapLevelCollectionsViewController");

				// check if the first element is selected (whichi is custom maps)
				if (annotatedBeatMapLevelCollectionsViewController.selectedItemIndex == 0)
				{
					return;
				}

				// get the table view
				var playlistsTableView =
					annotatedBeatMapLevelCollectionsViewController.GetField<AnnotatedBeatmapLevelCollectionsTableView, AnnotatedBeatmapLevelCollectionsViewController>("_playlistsTableView");

				// get the tableview to select custom songs
				var tableView = playlistsTableView.GetField<TableView, AnnotatedBeatmapLevelCollectionsTableView>("_tableView");
				tableView.ScrollToCellWithIdx(0, TableViewScroller.ScrollPositionType.Center, false);
				tableView.SelectCellWithIdx(0, true);
			}
		}
	}
}