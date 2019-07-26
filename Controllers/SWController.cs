using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using RestEase;
using StarWars.Models;
using StarWars.Proxies;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarWars.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class SWController : Controller
    {

        private readonly ISWApi characterClient;
        private readonly IOptionsSnapshot<CharacterAPIOptions> characterOptions;

        public SWController(ISWApi client,IOptionsSnapshot<CharacterAPIOptions> characterOptions)
        {

            this.characterClient = client;
            this.characterOptions = characterOptions;
        }


        // GET api/sw/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public ActionResult<Character> Get(int id)
        {
       



            Character result = null;

            try
            {
                //string baseUrl = characterOptions.Value.BaseUrl;
                //ISWApi api = RestClient.For<ISWApi>(baseUrl);
                result = characterClient.GetCharacterAsync($"{id}").Result;


            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Failed request to external resource.");
            }
            catch (TimeoutRejectedException)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, "Timeout on external web request.");
            }
            catch (Exception)
            {
                // Exception shielding for all other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "Request could not be processed.");
            }
            return Ok(result);

        }
    }
}
