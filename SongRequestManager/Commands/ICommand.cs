using System.Threading.Tasks;
using ChatCore.Interfaces;

namespace SongRequestManager.Commands
{
	public interface ICommand
	{
		public string Id { get; }
		public string[] Alias { get; set; }
		public string[] RequiredPermissions { get; set; }
		public Task HandleCommandAsync(IChatService chatService, IChatMessage chatMessage, string leftOverMessage);
	}
}