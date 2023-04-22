using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //ip and port information
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            //create a new TcpListener and start listening for client requests
            TcpListener server = new TcpListener(ipAddress, port);

            //start listening for client requests
            server.Start();
            Console.WriteLine($"Server started at port " + port + "...");

            //handles client requests asynchronously
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected.");
                await SendMessageAsync(client, "100 OK");

                //handle client in separate task
                _ = HandleClientAsync(client);
            }
        }

        static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                //gets the reference to the network stream
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    int choice = int.Parse(await ReceiveMessageAsync(client));
                    Console.WriteLine("Client(" + client.Client.RemoteEndPoint + ") sent: " + choice);
                    if (choice == 3)
                    {
                        Console.WriteLine("Client(" + client.Client.RemoteEndPoint + ") disconnected");
                        client.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                //closes the socket connection
                client.Close();
            }
        }

        // Receive a message from the client asynchronously
        private static async Task<string> ReceiveMessageAsync(TcpClient client)
        {
            // Get the network stream for the client
            NetworkStream stream = client.GetStream();

            // Read the message from the stream asynchronously
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // Return the message as a string
            return message;
        }

        // Send a message to the client asynchronously
        private static async Task SendMessageAsync(TcpClient client, string message)
        {
            // Get the network stream for the client
            NetworkStream stream = client.GetStream();

            // Convert the message to bytes and send it to the client asynchronously
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}