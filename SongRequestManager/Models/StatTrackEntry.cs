using System;
using IPA.Config.Stores.Attributes;

namespace SongRequestManager.Models
{
	internal class StatTrackEntry : User
	{
		public StatTrackEntry()
		{
		}

		public StatTrackEntry(User user, uint numberOfRequests = 0, DateTime? lastModified = null)
			: base(user.Id, user.Platform, user.Username)
		{
			NumberOfRequests = numberOfRequests;
			LastModified = lastModified ?? DateTime.Now;
		}

		[NonNullable]
		public uint NumberOfRequests { get; set; }

		[NonNullable]
		public DateTime LastModified { get; set; }
	}
}