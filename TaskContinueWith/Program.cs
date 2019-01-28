using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TaskContinueWith
{
    class Program
    {
        static Stopwatch watch = new Stopwatch();
        static int pendingTasks;
        static void Main(string[] args)
        {
            const int MaxValue = 1000000000;
            watch.Restart();
            int numTasks = Environment.ProcessorCount;
            pendingTasks = numTasks;
            int perThreadCount = MaxValue / numTasks;
            int perThreadLeftover = MaxValue % numTasks;
            var tasks = new Task<long>[numTasks];
            for (int i = 0; i < numTasks; i++)
            {
                int start = i * perThreadCount;
                int end = (i + 1) * perThreadCount;
                if (i == numTasks - 1)
                {
                    end += perThreadLeftover;
                }
                tasks[i] = Task<long>.Run(() =>
                {
                    long threadSum = 0;
                    for (int j = start; j <= end; j++)
                    {
                        threadSum += (long)Math.Sqrt(j);
                    }
                    return threadSum;
                });
                tasks[i].ContinueWith(OnTaskEnd, TaskContinuationOptions.ExecuteSynchronously);
            }
            Console.WriteLine("Main Completed!");
            Console.ReadKey();
        }
        private static async void OnTaskEnd(Task<long> task)
        {
            Console.WriteLine("Thread sum: {0}", await task);
            if (Interlocked.Decrement(ref pendingTasks) == 0)
            {
                watch.Stop();
                Console.WriteLine("Tasks: {0}", watch.Elapsed);
            }
        }
    }
}
