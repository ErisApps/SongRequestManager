using System;
using BeatSaverSharp;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Settings.Converters;

namespace SongRequestManager.Models
{
	public class Request
	{
		public virtual string BeatSaverKey { get; set; }
		[UseConverter(typeof(EnumStringConfigConverter<RequestStatus>))]
		public virtual RequestStatus Status { get; set; }
		[UseConverter(typeof(DateTimeConfigConverter))]
		public virtual DateTime RequestDateTime { get; set; }
		[UseConverter(typeof(BeatMapConfigConverter))]
		public virtual Beatmap BeatMap { get; set; }
	}
}