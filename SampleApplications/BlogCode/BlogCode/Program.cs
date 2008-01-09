namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Pipelines;
    using pipelines.ErlangTasks.Demos;

    internal class Program
    {
        private static void Main(string[] args)
        {
            //ProcessesPipeline();

            //MultiThreadedMultiplication();

            ParallelTasks();

            //MillionsOfTasks();

            //GatherAllFilesTasks();

            //HandlingErrors();

            //ScheduledTasks();

        }

        private static void ScheduledTasks()
        {
            Scheduler scheduler = new Scheduler();
            Future result = scheduler.Schedule(new WaitFiveSeconds());
            scheduler.Execute();
        }

        private static void HandlingErrors()
        {
            Scheduler scheduler = new Scheduler();
            Future result = scheduler.Schedule(new ThrowExceptionWhenZero(5));
            scheduler.Execute();
            try
            {
                result.GetValue<int>();
            }
            catch (TaskFailedException e)
            {
                Console.WriteLine(e);
            }

        }

        private static void GatherAllFilesTasks()
        {
            Scheduler scheduler = new Scheduler();
            Future files = scheduler.Schedule(new GatherAllFiles(@"D:\OSS"));
            scheduler.Execute();
            Console.WriteLine("Files: {0}, Tasks: {1}", files.GetValue<ICollection<string>>().Count,
                scheduler.TotalTasks);
        }

        private static void MillionsOfTasks()
        {
            Scheduler scheduler = new Scheduler();
            scheduler.Schedule(new GeneratorTask(10));
            scheduler.Execute();
        }

        private static void ProcessesPipeline()
        {
            TrivialProcessesPipeline process = new TrivialProcessesPipeline();
            process.Execute();
        }

        private static void MultiThreadedMultiplication()
        {
            Scheduler scheduler = new Scheduler();
            scheduler.Schedule(new UseMultiplicationTask());
            scheduler.Execute();
        }

        private static void ParallelTasks()
        {
            List<int> items = new List<int>();
            Random r = new Random();
            for (int i = 0; i < 500000; i++)
            {
                items.Add(r.Next());
            }
            Console.WriteLine("-----------");
            Scheduler scheduler = new Scheduler();
            ParallelQSort task = new ParallelQSort(items.ToArray());
            scheduler.Schedule(task);
            scheduler.Execute();

            int[] value = task.Completed.GetValue<int[]>();
        }
    }
}