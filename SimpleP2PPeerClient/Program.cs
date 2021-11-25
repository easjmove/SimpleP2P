using SimpleP2PLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

namespace SimpleP2PPeerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool shouldRun = true;
            string path = "c:\\temp\\downloads";
            RestWorker myWorker = new RestWorker("http://localhost:30304/Files");
            while (shouldRun)
            {
                Console.WriteLine("Enter GetFiles and empty line for all available files");
                Console.WriteLine("Enter GetEndpoints and newline with filename for available endpoints for the file");
                Console.WriteLine("Enter GetFile and newline with filename for downloading the file from a random endpoint");
                Console.WriteLine("Enter shutdownpeer for stopping a peer server, and nextline containing the Endpoint as Json");
                Console.WriteLine("Enter shutdown for stopping and empty line");
                string command = Console.ReadLine().ToLower();
                string value = Console.ReadLine();
                switch (command)
                {
                    case "getfiles":
                        ShowFilenames(myWorker);
                        break;
                    case "getendpoints":
                        ShowEndPoints(myWorker, value);
                        break;
                    case "getfile":
                        DownloadFile(myWorker, value, path);
                        break;
                    case "shutdown":
                        shouldRun = false;
                        break;
                    case "shutdownpeer":
                        FileEndpoint endpoint = JsonSerializer.Deserialize<FileEndpoint>(value);
                        ShutdownPeer(endpoint);
                        break;
                }
            }
        }

        private static void ShutdownPeer(FileEndpoint endpoint)
        {
            TcpClient socket = new TcpClient(endpoint.IPAddress, endpoint.Port);

            NetworkStream ns = socket.GetStream();
            StreamWriter writer = new StreamWriter(ns);
            writer.WriteLine("shutdown");
            writer.Flush();
            socket.Close();
        }

        private static void ShowFilenames(RestWorker myWorker)
        {
            foreach (string filename in myWorker.GetFilenames().Result)
            {
                Console.WriteLine(filename);
            }
        }

        private static void ShowEndPoints(RestWorker myWorker, string filename)
        {
            List<FileEndpoint> endpoints = myWorker.GetEndpoints(filename).Result;
            string serializedEndpoints = JsonSerializer.Serialize(endpoints);
            Console.WriteLine(serializedEndpoints);
        }

        private static void DownloadFile(RestWorker myWorker, string filename, string path)
        {
            List<FileEndpoint> endpoints = myWorker.GetEndpoints(filename).Result;
            Random rand = new Random();
            FileEndpoint endpoint = endpoints[rand.Next(endpoints.Count - 1)];

            TcpClient socket = new TcpClient(endpoint.IPAddress, endpoint.Port);

            NetworkStream ns = socket.GetStream();
            StreamWriter writer = new StreamWriter(ns);
            writer.WriteLine("GET " + filename);
            //writer.WriteLine("shutdown");
            writer.Flush();

            Console.WriteLine("creating file");
            using (Stream stream = File.OpenWrite(path + "\\" + filename))
            {
                ns.CopyTo(stream);
            }
            socket.Close();
            Console.WriteLine("File downloaded");
        }
    }
}
