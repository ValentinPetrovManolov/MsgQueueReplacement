using System.Threading;
using System;

namespace BBB
{
    public class Program
    {
        private static SynchronousSocketListener listener;
        private static SocketWriter sender;
        public static int Main(String[] args) 
        {  
            bool IsA = args.Length >= 1;

             var exitEvent = new ManualResetEvent(false);

            // This is used to handle Ctrl+C
            Console.CancelKeyPress += (sender, eventArgs) => {
                                        eventArgs.Cancel = true;
                                        exitEvent.Set();
                                    };

            listener = new SynchronousSocketListener(IsA ? 11001 : 11000);
            var listThread = new Thread(ReadMethod);
            listThread.Start();

            sender = new SocketWriter(IsA ? 11000 : 11001);
            var dispThread = new Thread(Display);
            dispThread.Start();
   

            // Wait for Ctrl+C
            exitEvent.WaitOne();

            listener.RequestStop();
            listener.Dispose();
            listThread.Join();
            dispThread.Join();

            return 0;  
        }  

        public static void ReadMethod()
        {
            while(true)
            {
                // Console.WriteLine("\nPlease Enter your next message:");
                sender.Send("//mq01", Console.ReadLine());
            }
        }

        public static void Display()
        {
            while(true)
            {
                Console.WriteLine("\n" + listener.ReadMsg("//mq01"));
            }            
        }
    }
}