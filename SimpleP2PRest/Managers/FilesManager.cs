using SimpleP2PLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleP2PRest.Managers
{
    public class FilesManager
    {
        //Stores all the endpoints in a dictionary which key is the filename, and the value is a List of endpoints for that filename
        private static readonly Dictionary<string, List<FileEndpoint>> _fileDictionary
            = new Dictionary<string, List<FileEndpoint>>();

        public IEnumerable<string> GetAllFileNames()
        {
            return _fileDictionary.Keys;
        }

        public IEnumerable<FileEndpoint> GetAllFileEndpoints(string filename)
        {
            if (_fileDictionary.ContainsKey(filename))
            {
                return _fileDictionary[filename];
            }
            else
            {
                return null;
            }
        }

        public void AddEndpoint(string filename, FileEndpoint endpoint)
        {
            //Checks if the dictionary already has the filename, if not, initializes a list for that filename
            if (!_fileDictionary.ContainsKey(filename))
            {
                _fileDictionary.Add(filename, new List<FileEndpoint>());
            }
            //Because of the If statement above, we can be sure that the dictionary has the filename, and that the list is initialized
            _fileDictionary[filename].Add(endpoint);
        }

        //returns a bool telling if the endpoint was deleted or not
        public bool DeleteEndpoint(string filename, FileEndpoint endpoint)
        {
            
            if (_fileDictionary.ContainsKey(filename))
            {
                List<FileEndpoint> files = _fileDictionary[filename];
                FileEndpoint toBeRemoved = files.Find(existingEndpoint =>
                                                      existingEndpoint.IPAddress == endpoint.IPAddress &&
                                                      existingEndpoint.Port == endpoint.Port);
                files.Remove(toBeRemoved);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
