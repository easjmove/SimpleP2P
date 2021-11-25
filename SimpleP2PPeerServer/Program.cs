using SimpleP2PLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleP2PPeerServer
{
    public class Program
    {
        //we use this to tell the server if it should stop running
        private static bool ShouldRun;
        //a local list of the files the server shares
        private static List<string> myfiles;
        //used to listen for clients
        private static TcpListener listener;
        public static void Main(string[] args)
        {
            myfiles = new List<string>();
            ShouldRun = true;

            //Hardcoded path on the local machine, should of course in a real project be configurable
            string path = "c:\\temp";
            //The port to listen for client requests, should also be configurable
            int port = 30304;
            //the address of the Rest Service should also be configurable
            RestWorker myWorker = new RestWorker("http://localhost:58697/Files");
            //Define a shared endpoint to use for all files, as this won't change
            FileEndpoint myEndpoint = new FileEndpoint { IPAddress = "localhost", Port = port };
            //Registers all the files to the Rest Service
            RegisterFiles(myWorker, myEndpoint, path);

            //Starts listening on all network adapters and on the previous specified port (the port of the Endpoint we registered on the Rest Service)
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server listening");
            //a client can send a command to the server telling it to stop listening
            while (ShouldRun)
            {
                //The try is implemented, because the line
                //listener.stop
                //makes the listener.AcceptTcpClient throw an exception
                try
                {
                    TcpClient socket = listener.AcceptTcpClient();
                    //Makes the Server concurrent, being able to handle clients simultaneously
                    Task.Run(() => { HandleClient(socket, path); });
                }
                catch (SocketException e)
                {
                    if ((e.SocketErrorCode == SocketError.Interrupted))
                    {
                        // a blocking listen has been cancelled
                        //Here we just ignore the exception
                    } else
                    {
                        //if the exception didn't come from the listener.stop, we throw it again
                        throw e;
                    }
                }
            }
            Console.WriteLine("Out of loop");
            //When stopping we tell the Rest Server not to store information about this server anymore
            //.Wait() makes it wait until everything is removed before closing the console 
            DeregisterFiles(myWorker, myEndpoint).Wait();
        }

        private async static void RegisterFiles(RestWorker myWorker, FileEndpoint endpoint, string path)
        {

            if (Directory.Exists(path))
            {
                foreach (string filename in Directory.GetFiles(path))
                {
                    myfiles.Add(filename);
                    await myWorker.AddFile(Path.GetFileName(filename), endpoint);
                    Console.WriteLine("added file: " + filename);
                }
            }
        }

        private async static Task DeregisterFiles(RestWorker myWorker, FileEndpoint endpoint)
        {
            foreach (string filename in myfiles)
            {
                await myWorker.DeleteFile(Path.GetFileName(filename), endpoint);
                Console.WriteLine("Deleted file: " + filename);
            }
        }

        private static void HandleClient(TcpClient socket, string path)
        {
            Console.WriteLine("Client");

            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);

            string command = reader.ReadLine().ToLower();
            //string command = "get test.txt";
            Console.WriteLine(command);

            if (command == "shutdown")
            {
                ShouldRun = false;
                //We tell the listener to stop listening otherwise it will wait for the next client to connect before registering the shutdown
                //because the AcceptTcpClient blocks the execution, and this is in another thread
                listener.Stop();
            }
            else if (command.StartsWith("get "))
            {
                string filename = command.Substring(command.IndexOf(' ') + 1);
                Console.WriteLine(filename);
                SendFile(socket, path, filename);
                Console.WriteLine("File is sent");
            }
            socket.Close();
        }

        private static void SendFile(TcpClient socket, string path, string filename)
        {
            NetworkStream ns = socket.GetStream();
            Console.WriteLine(path + filename);
            using (FileStream inputStream = File.OpenRead(path + "\\" + filename))
            {
                inputStream.CopyTo(ns);
                ns.Flush();
            }
        }
    }
}
