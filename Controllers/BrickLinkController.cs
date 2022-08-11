using System;
using brickLinkApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace brickLinkApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrickLinkController : ControllerBase
    {
        [HttpGet("{numItem}")]
        public string Get(string numItem)
        {
            Parsing parsing = new Parsing(numItem);
            var el = parsing.parseHTML();
            return el;
        }
    }
}

