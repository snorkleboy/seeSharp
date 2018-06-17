using System;
using System.Threading;
namespace seeSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Threader threader = new Threader();
            ThreadStart job = new ThreadStart(threader.DoWork);
            Thread thread = new Thread(job);
            Thread.CurrentThread.Name = "MainThread";

            thread.Start();

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("{0}: {1},", Thread.CurrentThread.Name, i);
            }
        }

       
    }
    public class Threader
    {
        public void DoWork()
        {
            // Queue a task.  
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(SomeLongTask));
            // Queue another task.  
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(AnotherLongTask));
        }

        private void SomeLongTask(Object state)
        {
            Thread.CurrentThread.Name = "thread 2";
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("{0}: {1},", Thread.CurrentThread.Name, i);
            }
        }

        private void AnotherLongTask(Object state)
        {
            Thread.CurrentThread.Name = "thread 3";
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("{0}: {1},", Thread.CurrentThread.Name, i);
            }
        }
    }
    
}
