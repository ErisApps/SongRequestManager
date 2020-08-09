using System;
using System.Threading.Tasks;
using ChatCore.Interfaces;
using SongRequestManager.Models;
using SongRequestManager.Services;
using SongRequestManager.Utilities;
using Zenject;

namespace SongRequestManager.Commands
{
	public class RequestCommand : ICommand
	{
		private readonly SongQueueService _songQueueService;

		[Inject]
		public RequestCommand(SongQueueService songQueueService)
		{
			_songQueueService = songQueueService;

			Alias = new[] {"bsr", "sr", "request"};
			RequiredPermissions = new string[0];
		}

		public string Id => nameof(RequestCommand);
		public string[] Alias { get; set; }
		public string[] RequiredPermissions { get; set; }

		public async Task HandleCommandAsync(IChatService chatService, IChatMessage chatMessage, string leftOverMessage)
		{
			Logger.Log("Recognized bsr command");
			// TODO: Do validation and possibly support more
			try
			{
				var added = await _songQueueService.AddRequest(new Request
				{
					BeatSaverKey = leftOverMessage,
					Status = RequestStatus.Queued,
					RequestDateTime = DateTime.Now
				});
				Logger.Log($"Song added: {added.Item1}");
				chatService.SendTextMessage(added.Item2, chatMessage.Channel);
			}
			catch (Exception e)
			{
				Logger.Log(e.ToString(), IPA.Logging.Logger.Level.Error);
				throw;
			}
		}
	}
}