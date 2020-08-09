using System;

namespace SongRequestManager.Models
{
	public class UserRequestsTracker
	{
		public UserRequestsTracker(uint numberOfRequests = 0, DateTime? lastReset = null)
		{
			NumberOfRequests = numberOfRequests;
			LastReset = lastReset ?? DateTime.Now;
		}
		
		public uint NumberOfRequests { get; set; }
		public DateTime LastReset { get; set; }
	}
}