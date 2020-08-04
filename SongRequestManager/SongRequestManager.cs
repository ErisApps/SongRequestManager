using System.Linq;
using SongRequestManager.Services;
using SongRequestManager.Services.Interfaces;
using SongRequestManager.UI;
using UnityEngine;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager
{
	public class SongRequestManager : MonoBehaviour
	{
		internal IBeatSaverService _beatSaverService;
		internal IUserRequestTrackerManager _userRequestTrackerManager;
		internal IChatHandlerService _chatHandlerService;
		internal ICommandManager _commandManager;

		public ISongQueueService SongQueueService;

		public static SongRequestManager Instance { get; set; }

		protected virtual void Awake()
		{
			Init();
		}
		
		
		public void Init()
		{
			Instance = this;
		}

		// Setup
		public void TheShowMustGoOn()
		{
			_beatSaverService ??= new BeatSaverService();
			_userRequestTrackerManager ??= new UserRequestTrackerManager();
			SongQueueService ??= new SongQueueService(_beatSaverService);

			if (_commandManager == null)
			{
				_commandManager = new CommandManager(SongQueueService, _beatSaverService, _userRequestTrackerManager);
				_commandManager.Setup();
			}

			if (_chatHandlerService == null)
			{
				_chatHandlerService = new ChatHandlerService(_commandManager, _beatSaverService);
				_chatHandlerService.Setup();
			}

			var srmButtonGo = Resources.FindObjectsOfTypeAll<SongRequestsButtonViewController>().FirstOrDefault();
			if (!srmButtonGo)
			{
				srmButtonGo = gameObject.AddComponent<SongRequestsButtonViewController>();
				DontDestroyOnLoad(srmButtonGo);
			}
			else
			{
				srmButtonGo.Init();
			}
		}

		// Cleanup
		internal void ShowEndedSendCleaningTeam()
		{
			if (Instance != null)
			{
				Logger.Log($"Attempt destroying of {nameof(SongRequestManager)}");

				_chatHandlerService.Dispose();

				Destroy(Instance);

				Instance = null!;
			}
		}
	}
}