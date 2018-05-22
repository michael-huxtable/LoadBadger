using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public interface IExecutor
    {
        Task ExecuteAsync();
    }
}