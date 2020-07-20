using System.Linq;
using System.Reflection;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SemVer;
using SongRequestManager.Extensions;
using SongRequestManager.Settings;
using SongRequestManager.Utilities;
using UnityEngine;
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
			BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += OnLateMenuSceneLoadedFresh;
			
			PluginUtils.Setup();
		}

		[OnDisable]
		public void OnDisable()
		{
			PluginUtils.Cleanup();

			BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh -= OnLateMenuSceneLoadedFresh;

			SongRequestManager.Instance.ShowEndedSendCleaningTeam();
		}

		private void OnLateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
		{
			Logger.Log("BS_utils lateMenuSceneLoadedFresh invoked", IPA.Logging.Logger.Level.Trace);

			SongRequestManager instance = Resources.FindObjectsOfTypeAll<SongRequestManager>().FirstOrDefault();
			if (!instance)
			{
				instance = new GameObject($"[{nameof(SongRequestManager)}] - Instance").AddComponent<SongRequestManager>();
				Object.DontDestroyOnLoad(instance.gameObject);
			}
			else
			{
				instance.Init();
			}

			instance.TheShowMustGoOn();
		}
	}
}