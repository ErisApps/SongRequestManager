using BeatSaberMarkupLanguage;
using SiraUtil.Zenject;
using SongRequestManager.UI;
using SongRequestManager.UI.LoadingProgressModal;
using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.Installers
{
	[RequiresInstaller(typeof(AppInstaller))]
	public class MenuInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Logger.Log($"Running {nameof(InstallBindings)} of {nameof(MenuInstaller)}");

			var songRequestsListViewController = BeatSaberUI.CreateViewController<SongRequestsListViewController>();
			var songRequestsSettingsViewController = BeatSaberUI.CreateViewController<SongRequestsSettingsViewController>();
			var songRequestsFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<SongRequestsFlowCoordinator>();
			var songRequestsButtonViewController = BeatSaberUI.CreateViewController<SongRequestsButtonViewController>();

			Container.Bind<SongListUtils>().ToSelf().AsSingle().NonLazy();
			Container.Bind<LoadingProgressModal>().ToSelf().AsTransient().Lazy();
			Container.InjectSpecialInstance<SongRequestsListViewController>(songRequestsListViewController);
			Container.InjectSpecialInstance<SongRequestsSettingsViewController>(songRequestsSettingsViewController);
			Container.InjectSpecialInstance<SongRequestsFlowCoordinator>(songRequestsFlowCoordinator);
			Container.InjectSpecialInstance<SongRequestsButtonViewController>(songRequestsButtonViewController);

			Logger.Log($"All bindings installed in {nameof(MenuInstaller)}");
		}
	}
}