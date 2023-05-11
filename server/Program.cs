using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Connect to the database
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            //creates a table called "coverages" with id,name and description columns
            string sql = "CREATE IF NOT EXIST TABLE coverages (id INT IDENTITY(1,1) PRIMARY KEY, name VARCHAR(255), description VARCHAR(255))";
            using SqlCommand command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
                        
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
                _ = Task.Run(() => HandleClientAsync(client));

                //closes connection to the database
                connection.Close();
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
                    if (choice == 1)
                    {
                        await ReceiveFileAsync(client);
                    }
                    if (choice == 2)
                    {
                        //Get the data from the table "coverage" in the database
                        

                    }
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

        private static async Task ReceiveFileAsync(TcpClient client)
        {
            Console.WriteLine("Receiving file...");
            string fileName = await ReceiveMessageAsync(client);
            await SendMessageAsync(client, "File name received");
            Console.WriteLine("File name: " + fileName);
            string file = await ReceiveMessageAsync(client);

            //Create a new directory to store the files if it doesn't exist
            if (!Directory.Exists("./files"))
            {
                Directory.CreateDirectory("./files");
            }


            //save the data to a file in the current directory
            File.WriteAllLines("files/" + fileName, new string[] { file });
            await SendMessageAsync(client, "File received");


            Console.WriteLine($"File received and saved to files/{fileName}");
        }
    }
}