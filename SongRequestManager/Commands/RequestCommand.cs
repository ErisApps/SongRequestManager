using System;
using System.Threading.Tasks;
using ChatCore.Interfaces;
using SongRequestManager.Models;
using SongRequestManager.Services.Interfaces;
using SongRequestManager.Utilities;

namespace SongRequestManager.Commands
{
	public class RequestCommand : ICommand
	{
		private readonly ISongQueueService _songQueueService;
		private readonly IBeatSaverService _beatSaverService;

		public RequestCommand(ISongQueueService songQueueService, IBeatSaverService beatSaverService)
		{
			_songQueueService = songQueueService;
			_beatSaverService = beatSaverService;

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