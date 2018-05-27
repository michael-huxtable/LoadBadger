using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public interface IExecutor
    {
        Task ExecuteAsync(CancellationTokenSource cancellationToken);
    }
}