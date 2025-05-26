using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafeDS
{
    using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;


    /// <summary>
    /// This is not working properly
    /// </summary>
class ConcurrentQueueRealWorldTestClass
{
    static ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
    static volatile bool running = true;
    static int enqueueCount = 0;
    static int dequeueCount = 0;

    static void Producer(int id, int targetOpsPerSecond)
    {
        Random rand = new Random(id);
        Stopwatch sw = Stopwatch.StartNew();

        double interval = 1000.0 / targetOpsPerSecond; // ms per op
        long lastTick = sw.ElapsedMilliseconds;

        while (running)
        {
            // Enqueue an integer (just id here)
            queue.Enqueue(id);
            Interlocked.Increment(ref enqueueCount);

            // Add jitter: sleep a small random time (0-200 microseconds)
            int jitterUs = rand.Next(0, 200);
            Thread.Sleep(TimeSpan.FromMilliseconds(interval / 2) + TimeSpan.FromTicks(jitterUs * 10)); // 1 tick = 100ns
            
            // Optionally throttle to maintain approx rate
            long now = sw.ElapsedMilliseconds;
            double elapsed = now - lastTick;
            if (elapsed < interval)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(interval - elapsed));
            }
            lastTick = now;
        }
    }

    static void Consumer(int targetOpsPerSecond)
    {
        Random rand = new Random();
        Stopwatch sw = Stopwatch.StartNew();

        double interval = 1000.0 / targetOpsPerSecond; // ms per op
        long lastTick = sw.ElapsedMilliseconds;

        while (running)
        {
            if (queue.TryDequeue(out int item))
            {
                Interlocked.Increment(ref dequeueCount);
            }
            else
            {
                // If queue empty, small wait to avoid busy spinning
                Thread.Sleep(0);
            }

            // Add jitter: sleep a small random time (0-200 microseconds)
            int jitterUs = rand.Next(0, 200);
            Thread.Sleep(TimeSpan.FromTicks(jitterUs * 10));

            // Throttle to maintain approx rate
            long now = sw.ElapsedMilliseconds;
            double elapsed = now - lastTick;
            if (elapsed < interval)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(interval - elapsed));
            }
            lastTick = now;
        }
    }

    public static void Test()
    {
        int totalOpsPerSecond = 400*1000;
        int producersCount = 10;
        int consumerCount = 1;

        int producerOpsPerSecond = (totalOpsPerSecond / 2 ) / producersCount; // Half ops to producers
        int consumerOpsPerSecond = totalOpsPerSecond / 2; // Half ops to consumer


        Console.WriteLine(producerOpsPerSecond + ":" + consumerOpsPerSecond);
        Thread[] producers = new Thread[producersCount];
        for (int i = 0; i < producersCount; i++)
        {
            int id = i;
            producers[i] = new Thread(() => Producer(id, producerOpsPerSecond));
            producers[i].IsBackground = true;
            producers[i].Start();
        }

        Thread consumer = new Thread(() => Consumer(consumerOpsPerSecond));
        consumer.IsBackground = true;
        consumer.Start();

        Stopwatch sw = Stopwatch.StartNew();

        Console.WriteLine("Running for 60 seconds...");
        while (sw.Elapsed.TotalSeconds < 60)
        {
            Thread.Sleep(1000);
            int enq = Interlocked.Exchange(ref enqueueCount, 0);
            int deq = Interlocked.Exchange(ref dequeueCount, 0);
            Console.WriteLine($"Enqueued: {enq} items/sec, Dequeued: {deq} items/sec, Queue size: {queue.Count}");
        }

        running = false;

        // Give threads a moment to finish
        Thread.Sleep(1000);

        Console.WriteLine("Test completed.");
    }
}

}
