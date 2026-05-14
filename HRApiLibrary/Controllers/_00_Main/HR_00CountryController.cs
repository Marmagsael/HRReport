using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRApiLibrary.Controllers._00_Main
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HR_00CountryController : ControllerBase
    {
        private readonly I_00CountryDataAccess _country;

        public HR_00CountryController(I_00CountryDataAccess country)
        {
            _country = country;
        }



        [HttpGet("02/{schema}/{conn}")]
        public async Task<ActionResult<List<CountryModel>>> _02(string schema = "Main", string conn = "MySqlConn")
        {
            var res = await  _country._02(schema, conn);
            return Ok(res); 
        }

        [HttpGet("02/{id}/{schema}/{conn}")]
        public async Task<ActionResult<CountryModel>> _02(int id, string schema = "Main", string conn = "MySqlConn")
        {
            var res = await _country._02(id, schema, conn);
            return Ok(res);
        }

    }
}
