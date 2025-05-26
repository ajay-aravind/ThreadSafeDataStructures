using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreadSafeDS;

public class ConcurrentList{


    /// <summary>
    /// The default Concurrent queue is atleast twice as fast as my implementation. Here my implementation always use locks, where C# internal queue uses spinWait(){wait for cpu tick, instead of context swtiching) and volatile variables and atomic operations and segmented linked list to achieve good performance. Atomic operations and linkedlist would be a good addition
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args) {
        // TestThreadSafeList();

        //for (int index = 0; index < 20; index++)
        //{
        //    TestThreadSafeQueueTest();
        //}

        ConcurrentQueueRealWorldTest();
    }


    //Result:
    /*
     *  TimeTakenToComplete, custome queue:263
        TimeTakenToComplete:120
        TimeTakenToComplete, custome queue:98
        TimeTakenToComplete:119
        TimeTakenToComplete, custome queue:93
        TimeTakenToComplete:97
        TimeTakenToComplete, custome queue:57
        TimeTakenToComplete:90
        TimeTakenToComplete, custome queue:95
        TimeTakenToComplete:98
        TimeTakenToComplete, custome queue:85
        TimeTakenToComplete:78
        TimeTakenToComplete, custome queue:93
        TimeTakenToComplete:82
        TimeTakenToComplete, custome queue:69
        TimeTakenToComplete:80
        TimeTakenToComplete, custome queue:84
        TimeTakenToComplete:89
        TimeTakenToComplete, custome queue:87
        TimeTakenToComplete:88
        TimeTakenToComplete, custome queue:89
        TimeTakenToComplete:79
        TimeTakenToComplete, custome queue:90
        TimeTakenToComplete:89
        TimeTakenToComplete, custome queue:67
        TimeTakenToComplete:89
        TimeTakenToComplete, custome queue:98
        TimeTakenToComplete:59
        TimeTakenToComplete, custome queue:87
        TimeTakenToComplete:85
        TimeTakenToComplete, custome queue:82
        TimeTakenToComplete:85
        TimeTakenToComplete, custome queue:71
        TimeTakenToComplete:96
        TimeTakenToComplete, custome queue:92
        TimeTakenToComplete:89
        TimeTakenToComplete, custome queue:186
        TimeTakenToComplete:92
        TimeTakenToComplete, custome queue:67
        TimeTakenToComplete:84

        For some reason except for the first iteration, my naive implementation is as fast as ConcurrentQueue .net internal implementation which is lock free.
    May be i am not testing properly?
     */
    private static void TestThreadSafeQueueTest()
    {
        new ThreadSafeQueueTest().TestMyThreadSafeQueue();
    }

    private static void ConcurrentQueueRealWorldTest()
    {
        ConcurrentQueueRealWorldTestClass.Test();
    }

    private static void TestThreadSafeList()
    {
        Console.WriteLine("hello world");
        ThreadSafeList safeList = new ThreadSafeList();
        int i = 2000, j = 0;
        Task[] tasks = new Task[3000];

        Random rand = new Random();

        // 20 add tasks
        for (; j < i; j++) {
            int localJ = j;
            Task t = new Task(() => { safeList.Add(localJ); });
            tasks[j] = t;
        }

        //10 random delete tasks
        for(int k = 0; k < 1000; k++)
        {
            Task t = new Task(() =>
            {
                try
                {
                    int index = rand.Next(1, 1000);
                    safeList.Delete(index);
                }catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            
            });
            tasks[i+k] = t;
        }

        var watch = Stopwatch.StartNew();
        for (int k = 0; k < 3000; k++)
        {
            tasks[k].Start();
        }

        Task.WaitAll(tasks);
        watch.Stop();
        Console.WriteLine("TimeTakenToComplete:" + watch.ElapsedMilliseconds.ToString());

        foreach(int element in safeList)
        {
            // Console.WriteLine("int:" + element);
        }

        List<Task> defaultImplTasks = new List<Task>();

        ConcurrentQueue<int> queue = new ConcurrentQueue<int>();

        // 2000 adds
        for(int index = 0; index < 2000; index++)
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