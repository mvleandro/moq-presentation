using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoqPresentation.DataContracts;
using MoqPresentation.Model;
using MoqPresentation.Repositories;

namespace MoqPresentation.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        private readonly ILogger Logger;

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>The repository.</value>
        public UserElasticsearchRepository Repository { get; set; }

        /// <summary>
        /// Constructor with IOptions.
        /// </summary>
        /// <param name="options">Options.</param>
        public UsersController(IOptions<ElasticsearchConfiguration> options, ILogger<UsersController> logger = null)
        {
            if(options != null)
            {
                Repository = new UserElasticsearchRepository(options.Value);
            }

            this.Logger = logger;
        }

        // GET api/users
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                User user = Repository.Get(id);

                if(user == null)
                {
                    response.Failed = true;
                    return StatusCode(404, response);
                }
                response.Data = user;
                return StatusCode(200, response);
            }
            catch(Exception ex)
            {   

                Logger.Log(LogLevel.Error, new EventId(1, "Exception"), "Server error.", ex, (arg1, arg2) => arg1);
                response.Failed = true;
                response.Error = ex;
                return StatusCode(500, response);
            }

        }

        // POST api/users
        [HttpPost]
        public IActionResult Post([FromBody]User value)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                Repository.Add(value);
                return StatusCode(201, response);
            }
            catch(Exception ex)
            {
                response.Failed = true;
                response.Error = ex;

                return StatusCode(409, response);
            }
            
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
