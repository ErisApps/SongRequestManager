using System;
using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.Installers
{
	public class AppInstaller : MonoInstaller
	{
		public static bool FirstBindingInstalled { get; private set; } = false;

		public override void InstallBindings()
		{
			try
			{
				Logger.Log($"Running {nameof(InstallBindings)} of {nameof(AppInstaller)}");

				Logger.Log($"All bindings installed in {nameof(AppInstaller)}");

				FirstBindingInstalled = true;
			}
			catch (Exception e)
			{
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Info);
				if (e.InnerException != null)
				{
					Logger.Log(e.InnerException.ToString(), IPA.Logging.Logger.Level.Info);
				}
			}
		}
	}
}