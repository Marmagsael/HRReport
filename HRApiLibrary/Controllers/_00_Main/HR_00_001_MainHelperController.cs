using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HRApiLibrary.Controllers._00_Main;
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class HR_00_001_MainHelperController : ControllerBase
{
    // GET: api/<HR_00_001_MainHelperController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    //// GET api/<HR_00_001_MainHelperController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<HR_00_001_MainHelperController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<HR_00_001_MainHelperController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<HR_00_001_MainHelperController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
