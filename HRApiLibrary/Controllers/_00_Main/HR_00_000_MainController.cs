using Blazorise;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.Models._00_Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

namespace HRApiLibrary.Controllers._00_Main;
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class HR_00_000_MainController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly I_00MainTblMakerAccess _mainTblMaker;
    private readonly I_00MainDataMakerAccess _mainDataMaker;
    private readonly I_00MainPisTblMakerAccess _mainPisTblMaker;
    private readonly I_20_002_PayTblMaker _mainPayTblMaker;
    private readonly I_00MainDA _mainDa;

    public HR_00_000_MainController(IConfiguration config,
                                    I_00MainTblMakerAccess mainTblMaker,
                                    I_00MainDataMakerAccess mainDataMaker,
                                    I_00MainPisTblMakerAccess mainPisTblMaker,
                                    I_20_002_PayTblMaker mainPayTblMaker, 
                                    I_00MainDA mainDa)
    {
        _config             = config;
        _mainTblMaker       = mainTblMaker;
        _mainDataMaker      = mainDataMaker;
        _mainPisTblMaker    = mainPisTblMaker;
        _mainPayTblMaker    = mainPayTblMaker; 
        _mainDa = mainDa;
    }

    [HttpHead("01MainSchema/{schema}/{conn}")]
    public void CreateMainSchema(string schema = "Main", string conn = "MySqlConn")
    {
        _mainTblMaker._01MainTable(schema, conn);
        _mainDataMaker._01MainDefaultDatas(schema, conn);

    }

    [HttpHead("01MainPisSchemaInternal/{schema}/{conn}")]
    public void CreateMainPisSchemaInternal(string schema, string conn = "MySqlConn")
    {
        _mainPisTblMaker._01MainPisTableInternal(schema, conn);
        _mainPisTblMaker._01MainPisTable(schema, conn);

    }

    [HttpHead("01MainPisSchema/{schema}/{conn}")]
    public void CreateMainPisSchema(string schema = "MainPis", string conn = "MySqlConn")
    {
        _mainPisTblMaker._01MainPisTable(schema, conn);
        //return "Hello world"; 
    }


    [HttpHead("01UsersCompany/{userId}/{companyCode}/{companyName}/{currencyId}/{schema}/{conn}")]
    public void InsertDefaultCompany(int userId = 1,
                                     string companyCode = "GSIA",
                                     string companyName = "GSIA Inc.",
                                     int currencyId = 1,
                                     string schema = "Main",
                                     string conn = "MySqlConn")
    {
        _mainDataMaker._01UsersCompany_DefaultDatas(userId, companyCode, companyName, currencyId, schema, conn);
    }

    // -------------------------------------------------------------------------
    [HttpGet("02UserCompany/{id}/{schema}/{conn}")]
    public async Task<List<UserCompanyModel?>> _02UserCompany(int id, string schema = "Main", string conn = "MySqlConn")
    {

        var uc = await _mainDa._02UserCompanyPerUser(id, schema, conn);
        return uc;

    }



  


    //// GET: api/<HR_00_000_MainController>
    //[HttpGet]
    //public IEnumerable<string> Get()
    //{
    //    return new string[] { "value1", "value2" };
    //}

    //// GET api/<HR_00_000_MainController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    //// POST api/<HR_00_000_MainController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<HR_00_000_MainController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<HR_00_000_MainController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
