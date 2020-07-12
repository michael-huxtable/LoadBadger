using System;
using System.Diagnostics;
using System.Threading;

namespace LoadBadger.Core
{
    public abstract class FrequencyLoop
    {
        private readonly TimeSpan _frameTime;

        public Stopwatch Stopwatch { get; } = new Stopwatch();

        protected FrequencyLoop(double fps)
        {
            _frameTime = TimeSpan.FromSeconds(1.0 / fps);
        }
        
        public void Execute(CancellationTokenSource cancellationToken)
        {
            Stopwatch.Start();
            var last = Stopwatch.Elapsed;
            var updateTime = new TimeSpan(0);

            while (!cancellationToken.IsCancellationRequested)
            {
                var delta = Stopwatch.Elapsed - last;
                last += delta;
                updateTime += delta;
                
                while (updateTime > _frameTime)
                {
                    updateTime -= _frameTime;
                    OnUpdate(cancellationToken);
                }
                
                // On some systems, this returns in 1 millisecond
                // on others, it may return in much higher.
                // If so, you should just comment this out, and let the main loop
                // spin to wait out the timer.
                Thread.Sleep(1);
            }
        }

        protected abstract void OnUpdate(CancellationTokenSource cancellationTokenSource);
    }
}