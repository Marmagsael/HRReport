using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Models._00_MainPis;
using Microsoft.AspNetCore.Mvc;

namespace HRApiLibrary.Controllers._10_Pis;

[Route("api/[controller]")]
[ApiController]
public class HR_10_00_PisController : ControllerBase
{
    private readonly I_10_EmpmasDataAccess _empmas;

    public HR_10_00_PisController(I_10_EmpmasDataAccess empmas)
    {
        _empmas = empmas;
    }


    //****************************************************************************************
    //--- Empmas *****************************************************************************
    //****************************************************************************************

    [HttpPut("01Empas/{schema}/{conn}")]
    public async Task<ActionResult<EmpmasModel?>> _01Empmas([FromBody] EmpmasModel empmas, string schema = "MainPis", string conn = "MySqlConn")
    {
        var res = await _empmas._01Empmas(empmas, schema, conn);
        return Ok(res);

    }

    [HttpGet("02Empas/{id}/{schema}/{conn}")]
    public async Task<ActionResult<EmpmasModel?>> _02Empmas(int id, string schema = "MainPis", string conn = "MySqlConn")
    {
        try
        {
            var res = await _empmas._02Empmas(id, schema, conn);
            return Ok(res);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    [HttpPost("03Empas/{id}/{schema}/{conn}")]
    public async Task<ActionResult<EmpmasModel?>> _03Empmas(int id, [FromBody] EmpmasModel empmas, string schema = "MainPis", string conn = "MySqlConn")
    {
        var res = await _empmas._03Empmas(id, empmas, schema, conn);
        return Ok(res);

    }

    [HttpDelete("04Empas/{id}/{schema}/{conn}")]
    public async Task<ActionResult<EmpmasModel?>> _04Empmas(int id, string schema = "MainPis", string conn = "MySqlConn")
    {
        var res = await _empmas._04Empmas(id, schema, conn);
        return Ok(res);

    }

    //// GET: api/<HR_10_00_PisController>
    //[HttpGet]
    //public IEnumerable<string> Get()
    //{
    //    return new string[] { "value1", "value2" };
    //}

    //// GET api/<HR_10_00_PisController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<HR_10_00_PisController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<HR_10_00_PisController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<HR_10_00_PisController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
