using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SimpleP2PLibrary
{
    public class RestWorker
    {
        private string _baseURL;

        //When initializing the RestWorker it expects the base url of the Rest Service
        //each instance of a RestWorker only works toward one RestService
        public RestWorker(string baseURL)
        {
            _baseURL = baseURL;
        }

        public async Task AddFile(string filename, FileEndpoint endpoint)
        {
            //The using statement makes sure that it cleans up when finished
            //by calling the dispose method 
            using (HttpClient client = new HttpClient())
            {
                //Serializes the endpoint and readies it to be send using the HttpClient
                JsonContent jsonEndpoint = JsonContent.Create(endpoint);
                //Calls the Rest Service with the base URL and add the /endpoints/filename to the URI
                HttpResponseMessage response = await client.PostAsync(_baseURL + "/endpoints/" + filename, jsonEndpoint);
            }
        }

        public async Task DeleteFile(string filename, FileEndpoint endpoint)
        {
            //The using statement makes sure that it cleans up when finished
            //by calling the dispose method 
            using (HttpClient client = new HttpClient())
            {
                //Serializes the endpoint and readies it to be send using the HttpClient
                JsonContent jsonEndpoint = JsonContent.Create(endpoint);
                //Calls the Rest Service with the base URL and add the /endpoints/filename to the URI
                await client.PutAsync(_baseURL + "/endpoints/" + filename, jsonEndpoint);
            }
        }

        public async Task<List<FileEndpoint>> GetEndpoints(string filename)
        {
            //The using statement makes sure that it cleans up when finished
            //by calling the dispose method
            using (HttpClient client = new HttpClient())
            {
                //Calls the Rest Service with the base URL and add the /endpoints/filename to the URI
                //Recieves all endpoints for the specific filename
                HttpResponseMessage response = await client.GetAsync(_baseURL + "/endpoints/" + filename);
                //Deserializes the result
                List<FileEndpoint> endpoints = new List<FileEndpoint>(await response.Content.ReadFromJsonAsync<IEnumerable<FileEndpoint>>());
                return endpoints;
            }
        }

        public async Task<IEnumerable<string>> GetFilenames()
        {
            //The using statement makes sure that it cleans up when finished
            //by calling the dispose method 
            using (HttpClient client = new HttpClient())
            {
                //Calls the Rest service using only the base URL recieving all filenames
                HttpResponseMessage response = await client.GetAsync(_baseURL);
                //Deserializes the result
                IEnumerable<string> endpoints = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
                return endpoints;
            }
        }
    }
}
