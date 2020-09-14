using System;

namespace SongRequestManager.Helpers
{
	public interface IHasWeakReference
	{
		WeakReference WeakReference { get; }
	}
}