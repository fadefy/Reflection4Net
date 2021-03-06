﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Reflection4Net.Test.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class TestTimer
    {
        private readonly Action<long> action;
        private readonly Stopwatch timer = new Stopwatch();
        private long times = 0;

        public TestTimer(Action<long> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.action = action;
        }

        /// <summary>
        /// Test how many times an operation could perform within a given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long TimesInTime(TimeSpan time)
        {
            action(0);

            times = 1;
            timer.Restart();
            while (timer.Elapsed < time)
            {
                action(times);
                times++;
            }
            timer.Stop();

            return times;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="workerCount"></param>
        /// <returns></returns>
        public long TimesInTimeParallel(TimeSpan time, int workerCount)
        {
            if (workerCount <= 0 || workerCount > 64)
                throw new ArgumentOutOfRangeException("workerCount", "wokerCount must between 1 and 64");

            action(0);
            times = 0;
            var cts = new CancellationTokenSource();
            var factory = new TaskFactory();
            var tasks = new List<Task>(workerCount);
            for (int i = 0; i < workerCount; i++)
            {
                tasks.Add(factory.StartNew(TimesParallelWorkerAction, cts));
            }

            Thread.Sleep(time);
            cts.Cancel();

            return times;
        }

        /// <summary>
        /// Test the time an operation could take to run specified times.
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public TimeSpan TimeForTimes(int times)
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException("times");

            // Run a time to worm up.
            action(0);

            timer.Restart();
            var counter = 0;
            while (counter++ < times)
            {
                action(counter);
            }
            timer.Stop();

            return timer.Elapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="times"></param>
        /// <param name="workerCount"></param>
        /// <returns></returns>
        public TimeSpan TimeForTimesParallel(int times, int workerCount)
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException("times");
            if (workerCount <= 0 || workerCount > 64)
                throw new ArgumentOutOfRangeException("workerCount", "wokerCount must between 1 and 64");

            action(0);
            this.times = times;
            var tasks = new List<Task>(workerCount);
            for (int i = 0; i < workerCount; i++)
            {
                tasks.Add(new Task(TimeParallelWorkerAction));
            }

            tasks.ForEach(t => t.Start());
            timer.Restart();
            Task.WaitAll(tasks.ToArray());
            timer.Stop();

            return timer.Elapsed;
        }

        private void TimeParallelWorkerAction()
        {
            while (times > 0)
            {
                action(times);
                Interlocked.Decrement(ref times);
            }
        }

        private void TimesParallelWorkerAction(object state)
        {
            var cts = state as CancellationTokenSource;
            while (!cts.IsCancellationRequested)
            {
                action(times);
                Interlocked.Increment(ref times);
            }
        }
    }
}
