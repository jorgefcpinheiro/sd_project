using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //ip and port information
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            try
            {
                //creates a new client and connects to the server
                using TcpClient client = new TcpClient();

                await client.ConnectAsync(ipAddress, port);
                Console.WriteLine("Connected to server");

                //get the stream
                NetworkStream stream = client.GetStream();

                //receive the "100 OK" message from the server
                Console.WriteLine(await ReceiveMessageAsync(client));

                //handles user input
                while (true)
                {
                    Console.WriteLine("Options:\n1- Send the file\n2- Show the coverages\n3- Exit");
                    Console.WriteLine("Option: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            await SendMessageAsync(client, "1");
                            await SendFileAsync(client, "./files/example.txt");
                            break;
                        case "2":
                            Console.Clear();
                            await SendMessageAsync(client, "2");
                            break;
                        case "3":
                            Console.Clear();
                            await SendMessageAsync(client, "3");
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid option");
                            break;
                    }
                    if (choice == "3")
                    {
                        break;
                    }
                }
                //closes the connection
                client.Close();
                Console.WriteLine("Connection closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
            }
        }

        // Receive a message from the server asynchronously
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

        // Send a message to the server asynchronously
        private static async Task SendMessageAsync(TcpClient client, string message)
        {
            // Get the network stream for the client
            NetworkStream stream = client.GetStream();

            // Convert the message to bytes and send it to the server asynchronously
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task SendFileAsync(TcpClient client, string filePath)
        {
            //get the network stream for the client
            NetworkStream stream = client.GetStream();
            //read the file
            byte[] fileData = File.ReadAllBytes(filePath);
            //send the file name
            string fileName = Path.GetFileName(filePath);
            await SendMessageAsync(client, fileName);
            Console.WriteLine(await ReceiveMessageAsync(client));
            //send the file size
            string fileSize = fileData.Length.ToString();
            //send the file
            await stream.WriteAsync(fileData, 0, fileData.Length);

        }
    }
}