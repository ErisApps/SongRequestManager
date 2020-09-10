using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChatCore.Interfaces;
using ChatCore.Models.Twitch;
using SongRequestManager.Models;
using SongRequestManager.Settings;
using SongRequestManager.Settings.Partial;

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

		internal static int? GetMaxConcurrentRequestCountForUser(IChatUser user)
		{
			return user switch
			{
				TwitchUser twitchUser => GetMaxCurrentRequestCountForTwitchUser(twitchUser),
				_ => 0
			};
		}

		private static int? CheckIfUnlimited(int upperLimit, int originalRequestLimit)
		{
			return originalRequestLimit >= upperLimit ? null! : (int?)originalRequestLimit;
		}

		private static int? GetMaxCurrentRequestCountForTwitchUser(TwitchUser twitchUser)
		{
			int? requestLimit;
			if (twitchUser.IsModerator)
			{
				requestLimit = CheckIfUnlimited(TwitchSettings.USER_REQUEST_UPPER_LIMIT, SRMConfig.Instance.TwitchSettings.ModRequestLimit);
			}
			else if (twitchUser.IsSubscriber)
			{
				requestLimit = CheckIfUnlimited(TwitchSettings.SUB_REQUEST_UPPER_LIMIT, SRMConfig.Instance.TwitchSettings.SubRequestLimit);
			}
			else
			{
				requestLimit = CheckIfUnlimited(TwitchSettings.MOD_REQUEST_UPPER_LIMIT, SRMConfig.Instance.TwitchSettings.UserRequestLimit);
			}

			if (requestLimit != null && twitchUser.IsVip)
			{
				requestLimit += CheckIfUnlimited(TwitchSettings.VIP_BONUS_UPPER_LIMIT, SRMConfig.Instance.TwitchSettings.VipBonusLimit);
			}

			return requestLimit;
		}
	}
}