using System;
using BeatSaverSharp;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SongRequestManager.Settings.Converters;

namespace SongRequestManager.Models
{
	public class Request
	{
		public virtual string BeatSaverKey { get; set; }
		[UseConverter(typeof(EnumConverter<RequestStatus>))]
		public virtual RequestStatus Status { get; set; }
		[UseConverter(typeof(DateTimeConfigConverter))]
		public virtual DateTime RequestDateTime { get; set; }
		[UseConverter(typeof(BeatMapConfigConverter))]
		public virtual Beatmap BeatMap { get; set; }
	}
}