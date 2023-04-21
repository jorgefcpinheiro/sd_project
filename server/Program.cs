using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            //ip and port information
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            //create a new TcpListener 
            TcpListener server = new TcpListener(ip, 5000);

            //start listening
            server.Start();
            Console.WriteLine("Server started at port " + port.ToString() + "...");
            //handles all the clients that connect to the server
            Console.WriteLine("Waiting for clients to connect...");
            while (true)
            {
                //accepts the client
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected at " + client.Client.RemoteEndPoint);

                //get the stream
                NetworkStream stream = client.GetStream();

                //send a message to the client
                sendMessage(stream, "Hello " + client.Client.RemoteEndPoint);
    
                //receive a message from the client
                Console.WriteLine(receiveMessage(stream));

            }
        }

        //function to send a message to the client
        static void sendMessage(NetworkStream stream, string message)
        {
            //convert the message to bytes
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            //send the message to the client
            stream.Write(buffer, 0, buffer.Length);
        }

        //function to receive a message from the client
        static string receiveMessage(NetworkStream stream)
        {
            //create a buffer to store the message
            byte[] buffer = new byte[1024];

            //read the message from the client
            stream.Read(buffer, 0, buffer.Length);

            //convert the message to string
            string message = Encoding.ASCII.GetString(buffer);

            //return the message
            return message;
        }

        //function to send a message to the server
        static void sendMessageToServer(NetworkStream stream, string message)
        {
            //convert the message to bytes
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            //send the message to the server
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}