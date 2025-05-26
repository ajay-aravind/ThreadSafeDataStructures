using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafeDS
{
    public class ThreadSafeQueueTest
    {

        public void TestMyThreadSafeQueue() 
        { 
            var watch = Stopwatch.StartNew();
            List<Task> tasks = new List<Task>();

            ThreadSafeQueue<int> customQueue = new ThreadSafeQueue<int>();

            for(int index = 0; index< 20000; index++)
            {
                int localIndex = index;
                Task t = new Task(() => { customQueue.Enqueue(localIndex); });
                tasks.Add(t);
            }

            for(int index =0; index < 1000; index++)
            {
                Task t = new Task(() =>
                {
                    try
                    {
                        customQueue.Dequeue();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                tasks.Add(t);
            }

            watch.Reset();
            watch.Start();
            foreach(Task t in tasks)
            {
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
            watch.Stop();
            Console.WriteLine("TimeTakenToComplete, custome queue:"+ watch.ElapsedMilliseconds.ToString());

            // customQueue.PrintList();

            List<Task> defaultImplTasks = new List<Task>();

        ConcurrentQueue<int> queue = new ConcurrentQueue<int>();

        // 2000 adds
        for(int index = 0; index < 20000; index++)
        {
            int localIndex = index;
            Task t = new Task(() => { queue.Enqueue(localIndex); });
            defaultImplTasks.Add(t);
        }

        for(int index =0; index < 1000; index++)
        {
            Task t = new Task(() => { queue.TryDequeue(out int result); });
            defaultImplTasks.Add(t);
        }

        watch.Reset();
        watch.Start();
        foreach(Task t in defaultImplTasks)
        {
            t.Start();
        }
        Task.WaitAll(tasks.ToArray());
        watch.Stop();
        Console.WriteLine("TimeTakenToComplete:"+ watch.ElapsedMilliseconds.ToString());

        foreach(int element in queue)
        {
            // Console.WriteLine("int:" + element);
        }
        }
    }
}
