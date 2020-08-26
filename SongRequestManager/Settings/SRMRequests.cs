using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SongRequestManager.Models;
using SongRequestManager.Settings.Base;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SongRequestManager.Settings
{
	// ReSharper disable once InconsistentNaming
	[NotifyPropertyChanges]
	internal class SRMRequests : BaseConfig<SRMRequests>
	{
		[UseConverter(typeof(ListConverter<Request>))]
		public virtual List<Request> QueueData { get; set; } = new List<Request>();

		[UseConverter(typeof(ListConverter<Request>))]
		public virtual List<Request> HistoryData { get; set; } = new List<Request>();
	}
}