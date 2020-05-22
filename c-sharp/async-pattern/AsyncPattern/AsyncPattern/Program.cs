using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncPattern
{
    public class Program
    {
        static BenchMarker marker;

        static async Task Main(string[] args)
        {
            marker = new BenchMarker();

            marker.Start();

            marker.Log("Main() start");

            //await Run(ProcessResultOneAtTheTime);

            //await Run(WhenAllFinishesThenProcess);

            await Run(WhenAllWithAsyncAndResultProcessInOneUnit);

            //await Run(WhenAllWithAsyncAndResultProcessInOneUnitStress);

            //await Run(EagerProcessingV1);

            //await Run(EagerProcessingV2);

            marker.Stop("Main() end");
        }

        static async Task Run(Func<Task> f)
        {
            marker.Log($"{f.Method.Name} start");

            await f();

            marker.Log($"{f.Method.Name} end");
        }

        static async Task ProcessResultOneAtTheTime()
        {
            var r = new Result();

            var t1 = APICall1();
            var t2 = APICall2();
            var t3 = APICall3();

            r.Result3 = ProcessResult3(await t3);
            r.Result2 = ProcessResult2(await t2);
            r.Result1 = ProcessResult1(await t1);
        }

        static async Task WhenAllFinishesThenProcess()
        {
            var response = new Result();

            var t1 = APICall1();
            var t2 = APICall2();
            var t3 = APICall3();

            await Task.WhenAll(t1, t2, t3);

            response.Result1 = ProcessResult1(await t1);
            response.Result2 = ProcessResult2(await t2);
            response.Result3 = ProcessResult3(await t3);
        }

        static async Task WhenAllWithAsyncAndResultProcessInOneUnit()
        {
            var result = new Result();

            async Task t1()
            {
                var r = await APICall1();

                result.Result1 = ProcessResult1(r);
            }

            async Task t2()
            {
                var r = await APICall2();

                result.Result2 = ProcessResult2(r);
            }

            async Task t3()
            {
                var r = await APICall3();

                result.Result3 = ProcessResult3(r);
            }

            await Task.WhenAll(t1(), t2(), t3());
        }

        // //////////////////////////
        // Stress test Task.WhenAll()
        // //////////////////////////
        static async Task WhenAllWithAsyncAndResultProcessInOneUnitStress()
        {
            var result = new Result();

            async Task t1()
            {
                var r = await APICall1();

                result.Result1 = ProcessResult1(r);
            }

            async Task t2()
            {
                var r = await APICall2();

                result.Result2 = ProcessResult2(r);
            }

            async Task t3()
            {
                var r = await APICall3();

                result.Result3 = ProcessResult3(r);
            }

            var stress = new List<Task>();
            var count = 0;

            while (count < 1) {
                stress.Add(t1());
                //stress.Add(t2());
                //stress.Add(t3());
                count++;
            }

            await Task.WhenAll(stress);
        }

        static async Task EagerProcessingV1()
        {
            var result = new Result();

            async Task<Action> t1()
            {
                var r = await APICall1();

                return () => result.Result1 = ProcessResult1(r);
            }

            async Task<Action> t2()
            {
                var r = await APICall2();

                return () => result.Result2 = ProcessResult2(r);
            }

            async Task<Action> t3()
            {
                var r = await APICall3();

                return () => result.Result3 = ProcessResult3(r);
            }

            var tasks = new List<Task<Action>> {
                t1(), t2(), t3()
            };

            while (tasks.Any()) {
                var completedTask = await Task.WhenAny(tasks);

                tasks.Remove(completedTask);

                var processor = await completedTask;

                processor();
            }
        }

        static async Task EagerProcessingV2()
        {
            var resultContext = new Result();

            Func<Task> t1 = async () =>
            {
                var r = await APICall1();

                resultContext.Result1 = ProcessResult1(r);
            };

            Func<Task> t2 = async () =>
            {
                var r = await APICall2();

                resultContext.Result2 = ProcessResult2(r);
            };

            Func<Task> t3 = async () =>
            {
                var r = await APICall3();

                resultContext.Result3 = ProcessResult3(r);
            };


            var tasks = new List<Task>() { t1(), t2(), t3() };

            while (tasks.Any())
            {
                var completedTask = await Task.WhenAny(
                    tasks
                    );

                tasks.Remove(completedTask);
            }
        }

        static async Task<TaskResult1> APICall1()
        {
            marker.Log("API call 1 start");

            await Task.Delay(250);

            marker.Log("API call 1 end");

            return new TaskResult1
            {
                APIResult1 = "Task 1 Result"
            };
        }

        static async Task<TaskResult2> APICall2()
        {
            marker.Log("API call 2 start");

            await Task.Delay(100);

            marker.Log("API call 2 end");

            return new TaskResult2
            {
                APIResult2 = "Task 2 Result"
            };
        }

        static async Task<TaskResult3> APICall3()
        {
            marker.Log("API call 3 start");

            await Task.Delay(60);

            marker.Log("API call 3 end");

            return new TaskResult3
            {
                APIResult3 = "Task 3 Result"
            };
        }

        static Result1Processed ProcessResult1(TaskResult1 r1)
        {
            marker.Log("Process result 1 start");

            Thread.Sleep(50);

            marker.Log("Process result 1 end");

            return new Result1Processed() {
                FinalResult = r1.APIResult1 + " processed"
            };
        }

        static Result2Processed ProcessResult2(TaskResult2 r1)
        {
            marker.Log("Process result 2 start");

            Thread.Sleep(60);
            
            marker.Log("Process result 2 end");

            return new Result2Processed()
            {
                FinalResult = r1.APIResult2 + " processed"
            };
        }

        static Result3Processed ProcessResult3(TaskResult3 r1)
        {
            marker.Log("Process result 3 start");

            Thread.Sleep(30);

            marker.Log("Process result 3 end");

            return new Result3Processed()
            {
                FinalResult = r1.APIResult3 + " processed"
            };
        }    
    }

    class Result
    {
        public Result1Processed Result1 { get; set; }
        public Result2Processed Result2 { get; set; }
        public Result3Processed Result3 { get; set; }
    }

    class Result1Processed
    {
        public string FinalResult { get; set; } 
    }

    class Result2Processed
    {
        public string FinalResult { get; set; }
    }

    class Result3Processed
    {
        public string FinalResult { get; set; }
    }

    class TaskResult1
    {
        public string APIResult1 { get; set; }
    }

    class TaskResult2
    {
        public string APIResult2 { get; set; }
    }

    class TaskResult3
    {
        public string APIResult3 { get; set; }
    }
}
