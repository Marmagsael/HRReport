using HRApiLibrary.DataAccess._20_Pay.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HRApiLibrary.Models;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._00_Main;
using Microsoft.IdentityModel.Tokens;

namespace HRApiLibrary.Controllers._20_Pay;

[Route("api/[controller]")]
[ApiController]
public class HR_20_000_PayController : ControllerBase
{

    private readonly I_20_002_PayTblMaker       _payTblMaker;
    private readonly I_00UsersAccess            _user;
    private readonly I_00UserscompanyDataAccess _usercompany;

    public HR_20_000_PayController(I_20_002_PayTblMaker         payTblMaker, 
                                   I_00UsersAccess              user, 
                                   I_00UserscompanyDataAccess   usercompany)
    {
        _payTblMaker = payTblMaker;
        _user = user;
        _usercompany = usercompany;
    }

    [HttpHead("01PayTbl/{schema}/{country}/{conn}")]
    public void CreateMainSchema(string schema = "U1C1Pay", string country = "PH", string conn = "MySqlConn")
    {
        _payTblMaker._01(schema, country, conn);
    }

    [HttpGet("0PaySession/{userId}")]
    public async Task<IEnumerable<string>> GetPaySessionInfo(int userId) 
    { 
        List<string> sessionInfo = new List<string>();

        if(string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionPayModel.SessionKeyUsername)))
        {
            var usr = await _user._02ById(userId); 
            if(usr != null) {

                var uc = await _usercompany._02(usr.DefaultCoId); 
                if(uc != null)
                {
                    //HttpContext.Session.SetString(SessionPayModel.SessionKeyId, usr.Id.ToString());
                    HttpContext.Session.SetString(SessionPayModel.SessionKeyId, Guid.NewGuid().ToString());
                    HttpContext.Session.SetString(SessionPayModel.SessionKeyUsername, usr.LoginName);
                    
                    HttpContext.Session.SetString(SessionPayModel.Schema, uc.PaySchema);
                    string? defConn = "MySqlConn"; 
                    
                    if(usr.Domain?.Length > 0 ) defConn = usr.Domain;

                    HttpContext.Session.SetString(SessionPayModel.Conn, defConn);
                }
            }
        }


        var sessionId   = HttpContext.Session.GetString(SessionPayModel.SessionKeyId);
        var userName    = HttpContext.Session.GetString(SessionPayModel.SessionKeyUsername);
        var schema      = HttpContext.Session.GetString(SessionPayModel.Schema);
        var conn        = HttpContext.Session.GetString(SessionPayModel.Conn);

        sessionInfo.Add(sessionId); 
        sessionInfo.Add(userName);
        sessionInfo.Add(schema);
        sessionInfo.Add(conn);

        return sessionInfo;

    }


    // GET: api/<HR_02_000PayController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }


}