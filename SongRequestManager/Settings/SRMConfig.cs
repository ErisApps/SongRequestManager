using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using SongRequestManager.Settings.Base;
using SongRequestManager.Settings.Partial;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SongRequestManager.Settings
{
	// ReSharper disable once InconsistentNaming
	[NotifyPropertyChanges]
	internal class SRMConfig : ReloadableBaseConfig<SRMConfig>
	{
		public virtual GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();
		public virtual TwitchSettings TwitchSettings { get; set; } = new TwitchSettings();
	}
}