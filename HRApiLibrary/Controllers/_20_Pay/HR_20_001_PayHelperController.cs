using Microsoft.AspNetCore.Mvc;


namespace HRApiLibrary.Controllers._20_Pay;

[Route("api/[controller]")]
[ApiController]
public class HR_20_001_PayHelperController : ControllerBase
{
    // GET: api/<HR_02_001_PayHelperController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    //// GET api/<HR_02_001_PayHelperController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<HR_02_001_PayHelperController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<HR_02_001_PayHelperController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<HR_02_001_PayHelperController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
