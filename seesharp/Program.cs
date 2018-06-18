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
        Tasker tasker = new Tasker();
        custom.DoWork();
        Console.WriteLine("custom threader finished");
        pooler.DoWork();
        Console.WriteLine("pooler finished");
        parrallel.DoWork();
        Console.WriteLine("parrallel finished");
        tasker.DoWork();
        Console.WriteLine("tasker finished");



    }


}

public class CustomThreads
{
    public int DoWork() 
    {
        List<Thread> threads = new List<Thread>();
        ThreadStart[] jobs = { new ThreadStart(Work.SomeLongTask), new ThreadStart(Work.AnotherLongTask) };
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
        Work.SomeLongTask();

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
            ManualResetEvent[] handles = new ManualResetEvent[2];
            for (int i = 0; i < 2; i++)
            {
                handles[i] = new ManualResetEvent(false);
            }

            ThreadPool.QueueUserWorkItem(
                new WaitCallback((x) => {
                    Thread.CurrentThread.Name = "2";
                    Work.SomeLongTask();
                    handles[(int) ThreadPooler.counter].Set();
                    Interlocked.Increment(ref ThreadPooler.counter);
                }) );
            ThreadPool.QueueUserWorkItem(
                new WaitCallback((x) => {
                    Thread.CurrentThread.Name = "3";
                    Work.AnotherLongTask();
                    handles[(int) ThreadPooler.counter].Set();
                    Interlocked.Increment(ref ThreadPooler.counter);

                }));
            WaitHandle.WaitAll(handles);
            Work.SomeLongTask();
            Console.WriteLine("counter = {0}", counter);


        }

        return 1;
    }
    
}


public class MultiTask
{

    public int DoWork()
    {
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
                    Work.SomeLongTask();
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
                    Work.AnotherLongTask();
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
                    Work.AnotherLongTask();
                }
            }
        });
        return 1;

    }
}
public class Tasker
{
    public int DoWork()
    {
        Task<String>[] taskArray =
        {
           Task<String>.Factory.StartNew(()=>{
               Thread.CurrentThread.Name = "7";
               String res = Work.SomeLongTask(new object());
               return res;
           }),
           Task<String>.Factory.StartNew(()=>{
               Thread.CurrentThread.Name = "8";
               String res = Work.AnotherLongTask(new object());
               return res;
           })
        };
        String results = "";
        for (int i = 0; i < taskArray.Length; i++)
        {
            results += taskArray[i].Result;
            Console.WriteLine("res {0}", taskArray[i].Result);
        }
        Console.WriteLine("results :{0}", results);
        Work.AnotherLongTask();
        return 1;
    }
}
static public class Work
{
    static public void SomeLongTask()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, shorttask", Thread.CurrentThread.Name, i);
            Thread.Sleep(1);

        }

    }

    static public void AnotherLongTask()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, longtask", Thread.CurrentThread.Name, i);
            Thread.Sleep(3);

        }

    }

    static public string SomeLongTask(Object state)
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, shorttask", Thread.CurrentThread.Name, i);
            Thread.Sleep(1);

        }
        return "task done";

    }

    static public string AnotherLongTask(Object state)
    {

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("thread name: {0}, i: {1}, longtask", Thread.CurrentThread.Name, i);
            Thread.Sleep(3);

        }
        return "task done";
    }
}

