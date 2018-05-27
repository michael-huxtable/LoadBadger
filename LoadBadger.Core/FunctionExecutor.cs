using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.Core
{
    public class FunctionExecutor : IExecutor
    {
        private readonly Func<Task> _task;

        public FunctionExecutor(Func<Task> task)
        {
            _task = task;
        }

        public Task ExecuteAsync(CancellationTokenSource cancellationToken)
        {
            return _task();
        }
    }
}