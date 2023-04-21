using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            //create a new TcpClient
            TcpClient client = new TcpClient();

            //connect to the server
            client.Connect(ip, port);
            Console.WriteLine("Connected to server");

            //get the stream
            NetworkStream stream = client.GetStream();

            //receive a message from the server
            Console.WriteLine(receiveMessage(stream));

            //send a message to the server
            sendMessage(stream, "100 OK");

            Console.ReadKey();

        }

        //function to send a message to the server
        static void sendMessage(NetworkStream stream, string message)
        {
            //convert the message to bytes
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            //send the message to the server
            stream.Write(buffer, 0, buffer.Length);
        }

        //funtion to receive a message from the server
        static string receiveMessage(NetworkStream stream)
        {
            //create a buffer to store the message
            byte[] buffer = new byte[1024];

            //read the message from the server
            stream.Read(buffer, 0, buffer.Length);

            //convert the message to string
            string message = Encoding.ASCII.GetString(buffer);

            //return the message
            return message;
        }
    }

}
