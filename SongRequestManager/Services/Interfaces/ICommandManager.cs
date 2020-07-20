using System;
using SongRequestManager.Commands;

namespace SongRequestManager.Services.Interfaces
{
	public interface ICommandManager : IDisposable
	{
		void Setup();
		ICommand FindCommand(string commandName);
	}
}