﻿using System;
using System.Diagnostics;
using System.Threading;

namespace LoadBadger.ConsolerRunner
{
    public class FrequencyLoop
    {
        private int _spinCounter;
        private readonly double _desiredFps;
        private readonly TimeSpan _frameTime;

        public Stopwatch Stopwatch { get; } = new Stopwatch();

        public FrequencyLoop(double fps)
        {
            _desiredFps = fps;
            _frameTime = TimeSpan.FromSeconds(1.0 / fps);
        }
        
        public void Exeucte()
        {
            Stopwatch.Start();
            var last = Stopwatch.Elapsed;
            var updateTime = new TimeSpan(0);

            //TODO: Make Cancellable

            while (true)
            {
                var delta = Stopwatch.Elapsed - last;
                last += delta;
                updateTime += delta;
                
                while (updateTime > _frameTime)
                {
                    updateTime -= _frameTime;
                    OnUpdate();
                    _spinCounter = 0;
                }
                
                _spinCounter++;
                
                // On some systems, this returns in 1 millisecond
                // on others, it may return in much higher.
                // If so, you should just comment this out, and let the main loop
                // spin to wait out the timer.
                Thread.Sleep(1);
            }
        }

        public event EventHandler Update;

        protected virtual void OnUpdate()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }
    }
}