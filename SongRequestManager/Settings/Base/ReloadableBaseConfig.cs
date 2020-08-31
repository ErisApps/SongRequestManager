using System;
using SongRequestManager.Utilities;

namespace SongRequestManager.Settings.Base
{
	internal abstract class ReloadableBaseConfig<T> : BaseConfig<T>
	{
		public event EventHandler ConfigChanged;

		public virtual void OnReload()
		{
			// This is called whenever the config file is reloaded from disk.
			// Use it to tell all of your systems that something has changed.

			// This is called off of the main thread, and is not safe to interact with Unity in
			Logger.Log($"{GetType().Name} got changed externally, reloading.");
			ConfigChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}