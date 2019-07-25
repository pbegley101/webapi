using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        // GET api/sw/5
        [HttpGet("{id}")]
        public ActionResult<Character> Get(int id)
        {
            // Create an implementation of that interface
            // We'll pass in the base URL for the API


            ISWApi api = RestClient.For<ISWApi>("https://swapi.co/api/");

            // Now we can simply call methods on it
            // Normally you'd await the request, but this is a console app
            Character character = api.GetCharacterAsync($"{id}").Result;
            return character;
        }
    }
}
