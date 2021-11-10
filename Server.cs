using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThreadUtils;
  
namespace BBB
{
public class SynchronousSocketListener: ActiveObject 
{  
        // Incoming data from the client.  
        private string data = null;  
        private int port_;
        private Dictionary<string, BlockingCollection<string>> msgQueues = new Dictionary<string, BlockingCollection<string>>();

        public SynchronousSocketListener(int port)
        {
            port_ = port;
        }
        public void StartListening() 
        {  
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];  

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port_);   
    
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp );  
            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            try {  
                listener.Bind(localEndPoint);  
                listener.Listen(10);  
    
                // Start listening for connections.  
                while (true) 
                {  
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();  
                    data = "";  
    
                    // An incoming connection needs to be processed.  
                    while (true) 
                    {  
                        int bytesRec = handler.Receive(bytes);  
                        data += Encoding.ASCII.GetString(bytes,0,bytesRec);  
                        if (data.IndexOf("<EOF>") > -1) 
                        {  
                            break;  
                        }  
                    }  
                    data = data.Substring(0, data.Length -5);
                    var tokens = data.Split("|") ; 
                    if (!msgQueues.ContainsKey(tokens[0]))
                        msgQueues.Add(tokens[0], new BlockingCollection<string>());
                    msgQueues[tokens[0]].Add(tokens[1]);
    
                    // Echo the data back to the client.    
                    handler.Send(Encoding.ASCII.GetBytes("OK"));  
                    handler.Shutdown(SocketShutdown.Both);  
                    handler.Close();  
                }  
    
            } 
            catch (Exception e) 
            {  
                Console.WriteLine(e.ToString());  
            }  
    
            Console.WriteLine("\nPress ENTER to continue...");  
            Console.Read();  
    
        }  
        override public void DoWork()
        {
            StartListening();
        }

        public string ReadMsg(string queueId)
        {
                if (!msgQueues.ContainsKey(queueId))
                    msgQueues.Add(queueId, new BlockingCollection<string>());
                return msgQueues[queueId].Take();
         }
    }  
}