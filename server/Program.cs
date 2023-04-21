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
                sendMessage(client.GetStream(), "100 OK");
                //threads the client
                Thread clientHandler = new Thread(new ParameterizedThreadStart(handleClient!));

                //starts the thread
                clientHandler.Start(client);
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

        static void handleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            // Handle the client connection here
            NetworkStream stream = client.GetStream();
            while (true){
                int choice = int.Parse(receiveMessage(stream));
                Console.WriteLine("Client(" + client.Client.RemoteEndPoint + ") sent: " + choice);
                if (choice == 3){
                    Console.WriteLine("Client(" + client.Client.RemoteEndPoint + ") disconnected");
                    client.Close();
                    break;
                }
            }
        }
    }
}