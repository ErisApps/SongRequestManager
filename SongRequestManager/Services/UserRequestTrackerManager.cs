using System.Collections.Concurrent;
using SongRequestManager.Models;

namespace SongRequestManager.Services
{
	// TODO: Unused for now
	public class UserRequestTrackerManager
	{
		private ConcurrentDictionary<string, UserRequestsTracker> _requestsTrackers;

		public UserRequestTrackerManager()
		{
			_requestsTrackers = new ConcurrentDictionary<string, UserRequestsTracker>();
		}

		public void IncrementRequest(string userId)
		{
		}
	}
}