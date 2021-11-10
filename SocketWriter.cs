using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  

namespace BBB
{
    public class SocketWriter
    {
        public SocketWriter(int port)
        {
            port_ = port;
        }
        private int port_;
        public void Send(string queueId, string message)
        {
            try 
            {  
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");  
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port_);  
                message = queueId + "|" + message ;
                message += "<EOF>";
                 byte[] msg = Encoding.ASCII.GetBytes(message) ;  
                 try 
                 {  
                    // Create a TCP/IP  socket.  
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp ); 
                    sender.Connect(remoteEP);  
                    sender.Send(msg);

                    // Receive the response from the remote device. 
                    byte[] bytes = new Byte[50];; 
                    sender.ReceiveTimeout = 10000;
                    int bytesRec = sender.Receive(bytes);
                    // Console.WriteLine("Response = {0}",
                    //    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                        sender.Shutdown(SocketShutdown.Both);  
                    sender.Close();
                    // Release the socket.  
      
                } 
                catch (SocketException se) 
                {  
                    Console.WriteLine("SocketException : {0}",se.ToString());  
                } catch (Exception e) {  
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());  
                }  
      
            } catch (Exception e) {  
                Console.WriteLine( e.ToString());  
        }  
    }
    }
}