using System;
using System.Linq;
using ChatCore;
using ChatCore.Interfaces;
using ChatCore.Services;
using SongRequestManager.Services.Interfaces;
using SongRequestManager.Settings;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.Services
{
	public class ChatHandlerService : IChatHandlerService
	{
		private readonly ICommandManager _commandManager;
		private readonly IBeatSaverService _beatSaverService;

		private ChatCoreInstance _chatCore;
		private ChatServiceMultiplexer _chatServiceMultiplexer;

		public ChatHandlerService(ICommandManager commandManager, IBeatSaverService beatSaverService)
		{
			_commandManager = commandManager;
			_beatSaverService = beatSaverService;

			// ChatCore setup
			_chatCore = ChatCoreInstance.Create();
		}

		void IChatHandlerService.Setup()
		{
			// ChatCore setup
			_chatCore = ChatCoreInstance.Create();
			
			if (_chatCore != null)
			{
				if (SRMConfig.Instance.TwitchSettings.Enabled)
				{
					_twitchService = _chatCore.RunTwitchServices();
					_twitchService.OnTextMessageReceived -= OnTextMessageReceived;
					_twitchService.OnTextMessageReceived += OnTextMessageReceived;
				}
			}
		}

		private async void OnTextMessageReceived(IChatService chatService, IChatMessage message)
		{
			Logger.Log($"ChatService: {chatService.DisplayName} - Message: {message.ToJson().ToString(4)}", IPA.Logging.Logger.Level.Trace);

			var prefix = SRMConfig.Instance.GeneralSettings.Prefix;
			if (!message.Message.StartsWith(prefix) || message.Message.TrimEnd().Length <= prefix.Length)
			{
				return;
			}

			var content = message.Message.Substring(prefix.Length);
			var commandSections = content.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			var commandName = commandSections.FirstOrDefault();
			if (commandName == null)
			{
				return;
			}

			var command = _commandManager.FindCommand(commandName);
			await command?.HandleCommandAsync(chatService, message, content.Substring(commandName.Length + 1))!;
		}

		public void Dispose()
		{
			Logger.Log("Disposing ChatHandlerService");
			if (_chatCore != null)
			{
				_chatCore.StopAllServices();

				if (_twitchService != null)
				{
					_twitchService.OnTextMessageReceived -= OnTextMessageReceived;
					_twitchService = null;
				}
			}
		}
	}
}