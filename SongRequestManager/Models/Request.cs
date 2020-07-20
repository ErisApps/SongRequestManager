using System;
using BeatSaverSharp;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Settings;

namespace SongRequestManager.Models
{
	public class Request
	{
		public virtual string BeatSaverKey { get; set; }
		public virtual RequestStatus Status { get; set; }
		public virtual DateTime RequestDateTime { get; set; }
		[UseConverter(typeof(BeatMapConfigConverter))]
		public virtual Beatmap BeatMap { get; set; }
	}
}