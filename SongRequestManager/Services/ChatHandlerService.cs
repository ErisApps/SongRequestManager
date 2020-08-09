using System;
using System.Linq;
using ChatCore;
using ChatCore.Interfaces;
using ChatCore.Services.Twitch;
using SongRequestManager.Settings;
using Zenject;
using Logger = SongRequestManager.Utilities.Logger;

namespace SongRequestManager.Services
{
	public class ChatHandlerService : IInitializable, IDisposable
	{
		private readonly CommandManager _commandManager;

		private ChatCoreInstance? _chatCore;
		private TwitchService? _twitchService;

		[Inject]
		public ChatHandlerService(CommandManager commandManager)
		{
			_commandManager = commandManager;
		}

		public void Initialize()
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