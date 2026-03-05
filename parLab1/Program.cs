using System;
using System.Threading;

class Program
{
    static int numberThreads = 4;
    static int[] steps = new int[numberThreads];
    static int[] duration = new int[numberThreads];
    static bool[] stopAllowed;
    static void Main()
    {
        GenerateStepsDurations();
        stopAllowed = new bool[numberThreads];
        Thread[] workerThreads = CreateWorkers();
        Thread controller = CreateController();
        controller.Start();
        foreach (Thread t in workerThreads)
        {
            t.Start();
        }
    }
    static void GenerateStepsDurations()
    {
        Random random = new Random();
        for (int i = 0; i < numberThreads; i++)
        {
            duration[i] = random.Next(1000, 5000);
            steps[i] = random.Next(1, 100);
        }
    }

    static Thread[] CreateWorkers()
    {
        Thread[] threads = new Thread[numberThreads];
        for (int i = 0; i < numberThreads; i++)
        {
            int index = i;
            threads[i] = new Thread(() => SumElements(index));
        }
        return threads;
    }
    static void SumElements(int index)
    {
        int step = steps[index];
        long sum = 0;
        long count = 0;
        long currentElement = 0;
        while (true)
        {
            bool shouldStop;
            shouldStop = stopAllowed[index];
            if (shouldStop)
            {
                break;
            }
            sum += currentElement;
            currentElement += step;
            count++;
        }

        Console.WriteLine($"Thread {index + 1}: sum={sum}, count={count}");
    }
    static Thread CreateController()
    {
        return new Thread(() =>
        {
            Thread[] timers = CreateTimers();
            foreach (Thread t in timers)
                t.Start();
        });
    }
    static Thread[] CreateTimers()
    {
        Thread[] timers = new Thread[numberThreads];
        for (int i = 0; i < numberThreads; i++)
        {
            int index = i;
            int waitTime = duration[index];
            timers[i] = new Thread(() =>
            {
                Thread.Sleep(waitTime);
                stopAllowed[index] = true;
            });
        }
        return timers;
    }
}