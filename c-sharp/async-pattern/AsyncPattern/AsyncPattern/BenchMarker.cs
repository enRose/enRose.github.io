using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AsyncPattern
{
    public class BenchMarker
    {
        private Stopwatch stopwatch;
        private static readonly object lockObj = new object();

        public List<int> ThreadUsedLog { get; set; } = new List<int>();
        public List<int> ThreadCountLog { get; set; } = new List<int>();

        private SeeIt seeIt = new SeeIt();

        public void Start()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void Stop(string taskName)
        {
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            var i = ThreadCountLog[^1] -
                ThreadCountLog[0] + 1;

            string msg = $"{taskName}\n" +
                $"Elapsed: {elapsedMs} ms\n" +
                $"Thread used: {ThreadUsedLog.Count}\n" +
                $"Concurrent thread used: {i}\n";

            Console.WriteLine(msg);

            seeIt.Save();
        }

        public void Log(string taskName)
        {
            string msg = null;

            var thread = Thread.CurrentThread;

            lock (lockObj)
            {
                if (!ThreadUsedLog.Contains(thread.ManagedThreadId))
                {
                    ThreadUsedLog.Add(thread.ManagedThreadId);
                }

                ThreadCountLog.Add(Process.GetCurrentProcess().Threads.Count);

                var elapsedMs = stopwatch.ElapsedMilliseconds;

                seeIt.Draw(taskName, elapsedMs);

                msg = $"{taskName} at {elapsedMs} ms\n" +
                      $"Background: {thread.IsBackground}\n" +
                      $"Thread Pool: {thread.IsThreadPoolThread}\n"+
                      $"Thread ID: {thread.ManagedThreadId}\n";
            }

            Console.WriteLine(msg);
        }

        public void ShowAvailableThreadInfo()
        {
            ThreadPool.GetAvailableThreads(out int worker, out int io);

            Console.WriteLine("Thread pool threads available at startup: ");
            Console.WriteLine("Worker threads: {0:N0}", worker);
            Console.WriteLine("Asynchronous I/O threads: {0:N0}", io);
        }
    }
}