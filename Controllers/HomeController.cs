using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace StarWars.Controllers
{
    [OpenApiIgnore]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
