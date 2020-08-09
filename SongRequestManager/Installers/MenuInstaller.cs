using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.Installers
{
	public class MenuInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			if (!AppInstaller.FirstBindingInstalled)
			{
				return;
			}

			Logger.Log($"Running {nameof(InstallBindings)} of {nameof(MenuInstaller)}");

			Logger.Log($"All bindings installed in {nameof(MenuInstaller)}");
		}
	}
}