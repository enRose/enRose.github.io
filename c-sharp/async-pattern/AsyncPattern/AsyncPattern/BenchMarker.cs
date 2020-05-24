using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace AsyncPattern
{
    public struct Measurement
    {
        public string TaskName { get; set; }
        public long ElapsedMs { get; set; }
        public bool IsBackground { get; set; }
        public bool IsThreadPoolThread { get; set; }
        public int ManagedThreadId { get; set; }
    }

    public class BenchMarker
    {
        public static Stopwatch stopwatchLite;

        private readonly string appName;
        private Stopwatch stopwatch;
        private static readonly object lockObj = new object();       
        private readonly Graph graph = new Graph();

        private HashSet<int> ThreadUsedLog { get; set; } = new HashSet<int>();
        private HashSet<int> ThreadCountLog { get; set; } = new HashSet<int>();
        private HashSet<Measurement> Log { get; set; } = new HashSet<Measurement>();

        public BenchMarker(string name)
        {
            appName = name;
        }

        public void StartWatch()
        {
            stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"{appName} start at " +
                $"{stopwatch.ElapsedMilliseconds} ms *************\n");
        }

        public void StopWatch()
        {
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            Log.OrderBy(x => x.ElapsedMs).ToList().ForEach(x => {
                var threadInfo = $"{x.TaskName} at {x.ElapsedMs} ms\n" +
                      $"Background: {x.IsBackground}\n" +
                      $"Thread Pool: {x.IsThreadPoolThread}\n" +
                      $"Thread ID: {x.ManagedThreadId}\n";

                Console.WriteLine(threadInfo);
            });

            var i = ThreadCountLog.OrderBy(c => c).ToList();

            var c = i[^1] - i[0] + 1;

            string msg = $"{appName}\n" +
                $"Elapsed: {elapsedMs} ms\n" +
                $"Thread used: {ThreadUsedLog.Count}\n" +
                $"Concurrent thread used: {c}\n";

            Console.WriteLine(msg);

            graph.Draw(Log);
        }

        public void Measure(string taskName)
        {
            var thread = Thread.CurrentThread;

            lock (lockObj)
            {
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                ThreadUsedLog.Add(thread.ManagedThreadId);
             
                ThreadCountLog.Add(Process.GetCurrentProcess().Threads.Count);
                
                Log.Add(new Measurement {
                    TaskName = taskName,
                    ElapsedMs = elapsedMs,
                    IsBackground = thread.IsBackground,
                    ManagedThreadId = thread.ManagedThreadId,
                    IsThreadPoolThread = thread.IsThreadPoolThread  
                });
            }
        }

        public void ShowAvailableThreadInfo()
        {
            ThreadPool.GetAvailableThreads(out int worker, out int io);

            Console.WriteLine("Thread pool threads available at startup: ");
            Console.WriteLine("Worker threads: {0:N0}", worker);
            Console.WriteLine("Asynchronous I/O threads: {0:N0}", io);
        }

        public static void StopwatchLiteDip(string taskName)
        {
            var elapsedMs = stopwatchLite.ElapsedMilliseconds;
            Console.WriteLine($"{taskName} at {elapsedMs}\n");
        }
    }
}