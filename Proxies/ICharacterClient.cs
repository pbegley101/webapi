using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestEase;
using StarWars.Models;

namespace StarWars.Proxies
{

    // Define an interface representing the API
    // GitHub requires a User-Agent header, so specify one
    [Header("User-Agent", "RestEase")]
    public interface ISWApi
    {
        // The [Get] attribute marks this method as a GET request
        // The "users" is a relative path the a base URL, which we'll provide later
        // "{userId}" is a placeholder in the URL: the value from the "userId" method parameter is used
        [Get("people/{peopleId}")]
        Task<Character> GetCharacterAsync([Path] string peopleId);
    }

}