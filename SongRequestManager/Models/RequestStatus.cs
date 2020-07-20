using System;

namespace SongRequestManager.Models
{
	[Flags]
	public enum RequestStatus
	{
		Queued,
		Forbidden,
		Skipped,
		Played
	}
}