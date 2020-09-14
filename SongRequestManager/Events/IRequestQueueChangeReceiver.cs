using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SongRequestManager.Events
{
	public interface IRequestQueueChangeReceiver
	{
		Task Handle(object sender, NotifyCollectionChangedEventArgs e);
	}
}