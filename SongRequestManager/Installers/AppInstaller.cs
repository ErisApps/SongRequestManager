using System;
using SongRequestManager.Commands;
using SongRequestManager.Services;
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

				Logger.Log($"Binding {nameof(PluginUtils)}");
				Container.BindInterfacesAndSelfTo<PluginUtils>().AsSingle().NonLazy();
				Logger.Log($"Binding {nameof(BeatSaverService)}");
				Container.BindInterfacesAndSelfTo<BeatSaverService>().AsSingle().NonLazy();
				Logger.Log($"Binding {nameof(ChatHandlerService)}");
				Container.BindInterfacesAndSelfTo<ChatHandlerService>().AsSingle().NonLazy();
				Logger.Log($"Binding {nameof(CommandManager)}");
				Container.BindInterfacesAndSelfTo<CommandManager>().AsSingle().NonLazy();
				Logger.Log($"Binding {nameof(SongQueueService)}");
				Container.BindInterfacesAndSelfTo<SongQueueService>().AsSingle().NonLazy();
				Logger.Log($"Binding {nameof(UserRequestTrackerManager)}");
				Container.BindInterfacesAndSelfTo<UserRequestTrackerManager>().AsSingle().NonLazy();
				Logger.Log($"Binding commands of type {nameof(ICommand)}");
				Container.Bind<ICommand>().To(binder => binder.AllNonAbstractClasses().DerivingFrom<ICommand>().FromThisAssembly()).AsSingle().NonLazy();
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