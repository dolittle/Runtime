// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services;

namespace Timer
{
    class Program
    {
        static void Main(string[] args)
        {
            var timerAmount = 10;
            var timerInterval = 1500;
            var taskAmount = 4000;
            var taskLength = 2;
            Console.WriteLine($"Creating {timerAmount} timers with {timerInterval}ms interval, and spawning {taskAmount} tasks that take {taskLength}s to complete.");

            using var cts = new CancellationTokenSource();
            var scheduler = new CallbackScheduler(cts.Token);
            var timers = RegisterTimers(scheduler, timerAmount, timerInterval);
            timers.AddRange(RegisterTimers(scheduler, 10, 600));
            timers.AddRange(RegisterTimers(scheduler, 2, 100));

            var tasks = SpawnTasks(taskAmount, taskLength);
            Task.WhenAll(tasks).Wait();
            Console.WriteLine("Tasks done, disposing timers");
            cts.Cancel();
            foreach (var disposable in timers)
            {
                disposable.Dispose();
            }
        }

        static List<IDisposable> RegisterTimers(ICanScheduleCallbacks scheduler, int amount, int interval)
        {
            var timers = new List<IDisposable>();
            for (var i = 0; i < amount; i++)
            {
                var timerTest = new TimerTest($"Timer {i + 1}");
                timers.Add(scheduler.ScheduleCallback(timerTest.TimerCallback, TimeSpan.FromMilliseconds(interval)));
            }
            return timers;
        }

        static IEnumerable<Task> SpawnTasks(int amount, int taskLength)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < amount; i++)
            {
                var task = Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(taskLength)));
                // var task = Task.Run(() => Task.Delay(TimeSpan.FromSeconds(taskLength)).Wait());

                tasks.Add(task);
            }
            return tasks;
        }
    }


    public class TimerTest
    {
        DateTime _timeWas;
        readonly string _timerName;

        public TimerTest(string timerName)
        {
            _timerName = timerName;
            _timeWas = DateTime.UtcNow;
        }

        public void TimerCallback()
        {
            var now = DateTime.UtcNow;
            var difference = now - _timeWas;
            Console.WriteLine("{0} was last called {1:s\\.fff}s ago", _timerName, difference);
            _timeWas = now;
        }
    }
}
