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
			LeaderboardEntries = new ObservableCollection<StatTrackEntry>(SRMStatTrack.Instance.LeaderboardEntries);
		}

		internal ObservableCollection<StatTrackEntry> LeaderboardEntries { get; set; }

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

			using (SRMStatTrack.Instance.ChangeTransaction)
			{
				_userCurrentRequestCounters.AddOrUpdate(user.Id, 1, (userId, requestCount) => ++requestCount);
				for (var i = 0; i < LeaderboardEntries.Count; i++)
				{
					var statTrackEntry = LeaderboardEntries[i];
					if (statTrackEntry.Id == user.Id)
					{
						statTrackEntry.NumberOfRequests++;
						statTrackEntry.LastModified = DateTime.Now;
						LeaderboardEntries[i] = statTrackEntry;
						return;
					}
				}

				LeaderboardEntries.Add(new StatTrackEntry(user, 1));
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