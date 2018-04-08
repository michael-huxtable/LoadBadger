using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBadger.ConsolerRunner
{
    public abstract class FrequencyLoop
    {
        private int _spinCounter;
        private readonly double _desiredFps;
        private readonly TimeSpan _frameTime;

        public Stopwatch Stopwatch { get; } = new Stopwatch();

        protected FrequencyLoop(double fps)
        {
            _desiredFps = fps;
            _frameTime = TimeSpan.FromSeconds(1.0 / fps);
        }
        
        public List<Task> Exeucte(CancellationToken cancellationToken)
        {
            Stopwatch.Start();
            var last = Stopwatch.Elapsed;
            var updateTime = new TimeSpan(0);
            var tasks = new List<Task>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var delta = Stopwatch.Elapsed - last;
                last += delta;
                updateTime += delta;
                
                while (updateTime > _frameTime)
                {
                    updateTime -= _frameTime;
                    tasks.AddRange(OnUpdate());
                    _spinCounter = 0;
                }
                
                _spinCounter++;
                
                // On some systems, this returns in 1 millisecond
                // on others, it may return in much higher.
                // If so, you should just comment this out, and let the main loop
                // spin to wait out the timer.
                Thread.Sleep(1);
            }

            return tasks;
        }

        protected abstract IEnumerable<Task> OnUpdate();
    }
}