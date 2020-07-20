using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SongRequestManager.Models;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SongRequestManager.Settings
{
	// ReSharper disable once InconsistentNaming
	[NotifyPropertyChanges]
	internal class SRMConfig
	{
		public static SRMConfig Instance { get; set; }
		
		public virtual GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();
		public virtual TwitchSettings TwitchSettings { get; set; } = new TwitchSettings();
		
		[UseConverter(typeof(ListConverter<Request>))]
		public virtual List<Request> QueueData { get; set; } = new List<Request>();

		[UseConverter(typeof(ListConverter<Request>))]
		public virtual List<Request> HistoryData { get; set; } = new List<Request>();
		
		public virtual void Changed()
		{
			// this is called whenever one of the virtual properties is changed
			// can be called to signal that the content has been changed
		}

		public virtual void OnReload()
		{
			// this is called whenever the config file is reloaded from disk
			// use it to tell all of your systems that something has changed
            
			// this is called off of the main thread, and is not safe to interact
			//   with Unity in
		}
	}
}