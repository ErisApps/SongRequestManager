using System;
using System.Linq;
using ChatCore;
using ChatCore.Interfaces;
using ChatCore.Services.Twitch;
using SongRequestManager.Settings;
using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.Services
{
	internal class ChatHandlerService : IInitializable, IDisposable
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
			Logger.Log($"{nameof(ChatHandlerService)} hash: {GetHashCode()}", IPA.Logging.Logger.Level.Warning);
			Logger.Log($"{nameof(CommandManager)} hash: {_commandManager.GetHashCode()}", IPA.Logging.Logger.Level.Warning);

			// ChatCore setup
			_chatCore = ChatCoreInstance.Create();

			if (_chatCore != null)
			{
				if (SRMConfig.Instance.TwitchSettings.Enabled)
				{
					Logger.Log("Creating ChatHandlerService");

					_twitchService = _chatCore.RunTwitchServices();
					_twitchService.OnTextMessageReceived -= OnTextMessageReceived;
					_twitchService.OnTextMessageReceived += OnTextMessageReceived;

					Logger.Log("Created ChatHandlerService");
				}
			}
		}

		private async void OnTextMessageReceived(IChatService chatService, IChatMessage message)
		{
			Logger.Log($"{nameof(ChatHandlerService)} hash: {GetHashCode()}", IPA.Logging.Logger.Level.Warning);
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
			if (_chatCore != null)
			{
				if (_twitchService != null)
				{
					Logger.Log("Disposing ChatHandlerService");

					_twitchService.OnTextMessageReceived -= OnTextMessageReceived;
					_twitchService = null;

					_chatCore.StopTwitchServices();

					Logger.Log("Disposed ChatHandlerService");
				}
			}
		}
	}
}