using System;

namespace SongRequestManager.Settings.Base
{
	internal abstract class BaseConfig<T>
	{
		internal static T Instance { get; set; }

		public virtual void Changed()
		{
			// this is called whenever one of the virtual properties is changed
			// can be called to signal that the content has been changed
		}

		public virtual IDisposable ChangeTransaction => null!;
	}
}