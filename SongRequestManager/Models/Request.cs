using System;
using BeatSaverSharp;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SongRequestManager.Settings.Converters;

namespace SongRequestManager.Models
{
	public class Request
	{
		[NonNullable]
		public virtual string BeatSaverKey { get; set; }  = null!;

		[NonNullable]
		[UseConverter(typeof(EnumConverter<RequestStatus>))]
		public virtual RequestStatus Status { get; set; }

		[NonNullable]
		[UseConverter(typeof(DateTimeConfigConverter))]
		public virtual DateTime RequestDateTime { get; set; }

		[NonNullable]
		[UseConverter]
		public virtual User Requestor { get; set; } = null!;

		[NonNullable]
		[UseConverter(typeof(BeatMapConfigConverter))]
		public virtual Beatmap BeatMap { get; set; } = null!;
	}
}