using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IOptionsSnapshot<CharacterAPIOptions> characterOptions;

        public SWController(IOptionsSnapshot<CharacterAPIOptions> characterOptions)
        {
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
                string baseUrl = characterOptions.Value.BaseUrl;
                ISWApi api = RestClient.For<ISWApi>(baseUrl);
                result = api.GetCharacterAsync($"{id}").Result;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);

        }
    }
}
