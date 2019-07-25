using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestEase;

namespace StarWars.Models
{
    // We receive a JSON response, so define a class to deserialize the json into
    public class Character
    {
        public string Name { get; set; }
        [JsonProperty("hair_color")]
        public string HairColor { get; set; }

        // This is deserialized using Json.NET, so use attributes as necessary
        [JsonProperty("height")]
        public int Height { get; set; }
    }


}
