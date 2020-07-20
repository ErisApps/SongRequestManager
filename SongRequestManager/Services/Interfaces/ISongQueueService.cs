using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SongRequestManager.Models;

namespace SongRequestManager.Services.Interfaces
{
	public interface ISongQueueService
	{
		ObservableCollection<Request> RequestQueue { get; }
		bool QueueOpen { get; }
		int QueuedRequestCount { get; }
		bool ToggleQueue();
		Task<(bool, string)> AddRequest(Request request);
		Task Play(Request request, CancellationToken cancellationToken, IProgress<double> downloadProgress = null);
	}
}