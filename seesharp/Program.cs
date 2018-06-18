using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


class Program
{

    static void Main(string[] args)
    {
        Thread.CurrentThread.Name = "main";

        CustomThreads custom = new CustomThreads();
        ThreadPooler pooler = new ThreadPooler();
        MultiTask parrallel = new MultiTask();
        custom.DoWork();
        Console.WriteLine("custom threadr finished");
        pooler.DoWork();
        Console.WriteLine("pooler finished");
        parrallel.DoWork();
        Console.WriteLine("paralell finished");



    }


}

public class CustomThreads
{
    public int DoWork() 
    {
        Work work = new Work();
        List<Thread> threads = new List<Thread>();
        ThreadStart[] jobs = { new ThreadStart(work.SomeLongTask), new ThreadStart(work.AnotherLongTask) };
        int num = 0;
        foreach (ThreadStart job in jobs)
        {
            Thread thread = new Thread(job);
            thread.Name = $"{num++}";
            threads.Add(thread);
        }
        foreach (Thread thread in threads)
        {
            thread.Start();
        }
        foreach (Thread thread in threads)
        {
            thread.Join();
        }
        work.SomeLongTask();

        return 1;
    }
}
    
public class ThreadPooler 
{
    public static int counter = 0;
    public int DoWork()
    {
        using (ManualResetEvent resetEvent = new ManualResetEvent(false))
        {
            Work work = new Work();
            ManualResetEvent[] handles = new ManualResetEvent[2];
            for (int i = 0; i < 2; i++)
            {
                handles[i] = new ManualResetEvent(false);
            }

            ThreadPool.QueueUserWorkItem(
                new WaitCallback((x) => {
                    Thread.CurrentThread.Name = "2";
                    work.SomeLongTask();
                    handles[(int) ThreadPooler.counter].Set();
                    Interlocked.Increment(ref ThreadPooler.counter);
                }) );
            ThreadPool.QueueUserWorkItem(
                new WaitCallback((x) => {
                    Thread.CurrentThread.Name = "3";
                    work.AnotherLongTask();
                    handles[(int) ThreadPooler.counter].Set();
                    Interlocked.Increment(ref ThreadPooler.counter);

                }));
            WaitHandle.WaitAll(handles);
            work.SomeLongTask();
            Console.WriteLine("counter = {0}", counter);


        }

        return 1;
    }
    
}


public class MultiTask
{

    public int DoWork()
    {
        Work work = new Work();
        Parallel.For(0, 3, (index) =>
        {
            if (index == 0)
            {
                try
                {
                    Thread.CurrentThread.Name = "4";

                }
                catch
                {
                    Console.WriteLine("thread cant be renamed probably main thread, {0}", Thread.CurrentThread.Name);
                }
                finally
                {
                    work.SomeLongTask();
                }
            }
            else if (index == 1)
            {
                try
                {
                    Thread.CurrentThread.Name = "5";

                }
                catch
                {
                    Console.WriteLine("thread cant be renamed, {0}", Thread.CurrentThread.Name);
                }
                finally
                {
                    work.AnotherLongTask();
                }
            }
            else
            {
                try
                {
                    Thread.CurrentThread.Name = "6";

                }
                catch
                {
                    Console.WriteLine("thread cant be renamed, {0}", Thread.CurrentThread.Name);
                }
                finally
                {
                    work.AnotherLongTask();
                }
            }
        });
        return 1;

    }
}

public class Work
{
    public void SomeLongTask()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, shorttask", Thread.CurrentThread.Name, i);
            Thread.Sleep(1);

        }
    }

    public void AnotherLongTask()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, longtask", Thread.CurrentThread.Name, i);
            Thread.Sleep(3);

        }
    }

    public void SomeLongTask(Object state)
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, shorttask", Thread.CurrentThread.Name, i);
            Thread.Sleep(1);

        }
    }

    public void AnotherLongTask(Object state)
    {

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, longtask", Thread.CurrentThread.Name, i);
            Thread.Sleep(3);

        }
    }
}

