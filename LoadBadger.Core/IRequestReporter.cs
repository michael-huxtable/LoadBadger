using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public interface IRequestReporter
    {
        ConcurrentBag<RequestTime> CompletedRequests { get; }
        ConcurrentBag<Task> InProgressRequests { get; }
        void GetStatistics();
    }
}