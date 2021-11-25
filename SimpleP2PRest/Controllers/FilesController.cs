using Microsoft.AspNetCore.Mvc;
using SimpleP2PLibrary;
using SimpleP2PRest.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleP2PRest.Controllers
{
    //Status codes should be implemented in this for more useability for consumers
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private FilesManager _manager = new FilesManager();

        // GET /Files
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _manager.GetAllFileNames();
        }

        // GET /Files/endpoints/test.txt
        [HttpGet("endpoints/{filename}")]
        public IEnumerable<FileEndpoint> Get(string filename)
        {
            return _manager.GetAllFileEndpoints(filename);
        }

        // POST /Files/endpoints/test.txt
        [HttpPost("endpoints/{filename}")]
        public void Post(string filename, [FromBody] FileEndpoint value)
        {
            _manager.AddEndpoint(filename, value);
        }


        //We use put to delete the files, becauase a standard HttpClient doesn't support the sending of objects in the DeleteMethod
        //And we don't need an update method in this project (at least this version)
        // PUT /Files/endpoints/test.txt
        [HttpPut("endpoints/{filename}")]
        public void Delete(string filename, [FromBody] FileEndpoint value)
        {
            _manager.DeleteEndpoint(filename, value);
        }
    }
}
