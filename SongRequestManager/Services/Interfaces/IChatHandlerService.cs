using System;

namespace SongRequestManager.Services.Interfaces
{
	public interface IChatHandlerService : IDisposable
	{
		internal void Setup();
	}
}