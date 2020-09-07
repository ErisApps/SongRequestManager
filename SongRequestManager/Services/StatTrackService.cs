using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChatCore.Interfaces;
using ChatCore.Models.Twitch;
using SongRequestManager.Models;
using SongRequestManager.Settings;

namespace SongRequestManager.Services
{
	public class StatTrackService
	{
		private ConcurrentDictionary<string, int> _userCurrentRequestCounters;

		public StatTrackService()
		{
			_userCurrentRequestCounters = new ConcurrentDictionary<string, int>(SRMRequests.Instance.QueueData
				.Where(x => x.Requestor?.Id != null)
				.GroupBy(x => x.Requestor.Id)
				.Select(group => new KeyValuePair<string, int>(group.Key, group.Count())));
			LeaderboardEntries = new ObservableCollection<StatTrackEntry>(SRMStatTrack.Instance.LeaderboardEntries
				.OrderByDescending(x => x.NumberOfRequests)
				.ThenBy(x => x.DisplayName, StringComparer.InvariantCultureIgnoreCase));
		}

		internal ObservableCollection<StatTrackEntry> LeaderboardEntries { get; }

		internal int GetCurrentRequestCountForUser(string userId)
		{
			return _userCurrentRequestCounters.TryGetValue(userId, out var requestCount) ? requestCount : 0;
		}

		internal void IncreaseRequestCountForUser(User user)
		{
			if (user?.Id == null)
			{
				return;
			}

			void MoveToCorrectSpot(StatTrackEntry entry, int oldIndex)
			{
				if (oldIndex == 0)
				{
					return;
				}

				var newIndex = oldIndex;
				for (var i = oldIndex - 1; i >= 0; i--)
				{
					var sortResult = (int)entry.NumberOfRequests - LeaderboardEntries[i].NumberOfRequests;
					if (sortResult == 0)
					{
						sortResult = string.Compare(LeaderboardEntries[i].DisplayName, entry.DisplayName, StringComparison.InvariantCultureIgnoreCase);
					}

					if (sortResult == 0)
					{
						newIndex = i;
						break;
					}

					if (sortResult < 0)
					{
						newIndex = i + 1;
						break;
					}
				}

				if (newIndex == oldIndex)
				{
					return;
				}

				LeaderboardEntries.Move(oldIndex, newIndex);
			}

			using (SRMStatTrack.Instance.ChangeTransaction)
			{
				_userCurrentRequestCounters.AddOrUpdate(user.Id, 1, (userId, requestCount) => ++requestCount);

				StatTrackEntry statTrackEntry = null!;
				for (var i = 0; i < LeaderboardEntries.Count; i++)
				{
					if (LeaderboardEntries[i].Id == user.Id)
					{
						statTrackEntry = LeaderboardEntries[i];
						statTrackEntry.NumberOfRequests += 1;
						statTrackEntry.LastModified = DateTime.Now;
						LeaderboardEntries[i] = statTrackEntry;
						MoveToCorrectSpot(statTrackEntry, i);
					}
				}

				if (statTrackEntry == null)
				{
					statTrackEntry = StatTrackEntry.Create(user, 1);
					LeaderboardEntries.Add(statTrackEntry);
					MoveToCorrectSpot(statTrackEntry, LeaderboardEntries.Count - 1);
				}

				SRMStatTrack.Instance.LeaderboardEntries = LeaderboardEntries.ToList();
			}
		}

		internal void DecreaseRequestCountForUser(User user)
		{
			if (user?.Id == null)
			{
				return;
			}

			_userCurrentRequestCounters.AddOrUpdate(user.Id, 0, (userId, requestCount) => --requestCount);
		}

		internal void ClearLeaderboard()
		{
			using (SRMStatTrack.Instance.ChangeTransaction)
			{
				LeaderboardEntries.Clear();
			}
		}

		internal static int GetMaxConcurrentRequestCountForUser(IChatUser user)
		{
			var requestLimit = 0;
			switch (user)
			{
				case TwitchUser twitchUser:
					if (twitchUser.IsModerator)
					{
						requestLimit = SRMConfig.Instance.TwitchSettings.ModRequestLimit;
					}
					else if (twitchUser.IsSubscriber)
					{
						requestLimit = SRMConfig.Instance.TwitchSettings.SubRequestLimit;
					}
					else
					{
						requestLimit = SRMConfig.Instance.TwitchSettings.UserRequestLimit;
					}

					if (twitchUser.IsVip)
					{
						requestLimit += SRMConfig.Instance.TwitchSettings.VipBonusLimit;
					}

					break;
			}

			return requestLimit;
		}
	}
}