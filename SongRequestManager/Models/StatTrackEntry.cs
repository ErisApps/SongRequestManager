using System;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Settings.Converters;

namespace SongRequestManager.Models
{
	internal class StatTrackEntry : User
	{
		/// <remarks>
		///	Please use this static Create method instead, the public ctor only exists for the BSIPA config
		/// </remarks>
		public static StatTrackEntry Create(User user, uint numberOfRequests = 0, DateTime? lastModified = null)
		{
			return new StatTrackEntry
			{
				Id = user.Id,
				Platform = user.Platform,
				DisplayName = user.DisplayName,
				NumberOfRequests = numberOfRequests,
				LastModified = lastModified ?? DateTime.Now
			};
		}

		[NonNullable]
		public virtual uint NumberOfRequests { get; set; }

		[NonNullable]
		[UseConverter(typeof(DateTimeConfigConverter))]
		public virtual DateTime LastModified { get; set; }
	}
}