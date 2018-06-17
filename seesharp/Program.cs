using System;
using System.Threading;
using System.Collections.Generic;

class Program
{

    static void Main(string[] args)
    {
        Thread.CurrentThread.Name = "main";

        CustomThreads custom = new CustomThreads();
        ThreadPooler pooler = new ThreadPooler();

        custom.DoWork();
        Console.WriteLine("custom threadr finished");
        pooler.DoWork();

    }

       
}
public class Threader
{
    public List<Thread> threads { get; set; }
    public Threader()
    {
        threads = new List<Thread>();
    }
    public virtual int DoWork()
    {
        return 1;
    }

}
public class CustomThreads : Threader
{
    public CustomThreads() : base()
    {

    }
    public override int DoWork() 
    {
        Work work = new Work();
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
        work.SomeLongTask();
        foreach (Thread thread in threads)
        {
            thread.Join();
        }
        return 1;
    }
}

public class ThreadPooler : Threader
{
    public ThreadPooler() : base()
    {

    }
    public override int DoWork()
    {
        Work work = new Work();
        System.Threading.ThreadPool.QueueUserWorkItem(
            new System.Threading.WaitCallback(work.SomeLongTask)
        );
        System.Threading.ThreadPool.QueueUserWorkItem(
            new System.Threading.WaitCallback(work.AnotherLongTask)
        );
        work.SomeLongTask();

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
        Thread.CurrentThread.Name = "0";
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, shorttask", Thread.CurrentThread.Name, i);
            Thread.Sleep(1);

        }
    }

    public void AnotherLongTask(Object state)
    {
        Thread.CurrentThread.Name = "1";

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, longtask", Thread.CurrentThread.Name, i);
            Thread.Sleep(3);

        }
    }
}
