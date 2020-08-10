using System.Reflection;
using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SemVer;
using SongRequestManager.Extensions;
using SongRequestManager.Settings;
using Config = IPA.Config.Config;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		private static PluginMetadata? _metadata;
		private static string? _name;
		private static Version? _version;

		private SettingsController? _settingsController;

		public static string Name => _name ??= _metadata?.Name ?? Assembly.GetExecutingAssembly().GetName().Name;
		public static Version Version => _version ??= _metadata?.Version ?? Assembly.GetExecutingAssembly().GetName().Version.ToSemVerVersion();

		[Init]
		public void Init([Config.Name("SongRequestManager")] Config config, IPA.Logging.Logger logger, PluginMetadata metaData)
		{
			SRMConfig.Instance = config.Generated<SRMConfig>();

			Logger.LogInstance = logger;

			_metadata = metaData;
		}

		[OnEnable]
		public void OnEnable()
		{
			SiraUtil.Zenject.Installer.RegisterAppInstaller<Installers.AppInstaller>();
			SiraUtil.Zenject.Installer.RegisterMenuInstaller<Installers.MenuInstaller>();
			
			BSMLSettings.instance.AddSettingsMenu("SRM (Alpha)", "SongRequestManager.Settings.Settings.bsml", _settingsController ??= new SettingsController());
		}

		[OnDisable]
		public void OnDisable()
		{
			BSMLSettings.instance.RemoveSettingsMenu(_settingsController);
			_settingsController = null;

			SiraUtil.Zenject.Installer.UnregisterMenuInstaller<Installers.MenuInstaller>();
			SiraUtil.Zenject.Installer.UnregisterAppInstaller<Installers.AppInstaller>();
		}
	}
}