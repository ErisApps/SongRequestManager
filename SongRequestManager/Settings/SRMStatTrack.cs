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
	internal class SRMStatTrack : BaseConfig<SRMStatTrack>
	{
		[NonNullable]
		[UseConverter(typeof(ListConverter<StatTrackEntry>))]
		public virtual List<StatTrackEntry> LeaderboardEntries { get; set; } = new List<StatTrackEntry>();
	}
}