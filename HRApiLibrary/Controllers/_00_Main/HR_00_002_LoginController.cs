using HRApiLibrary.DataAccess._00_Login.Interface;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;

namespace HRApiLibrary.Controllers._00_Main;
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class HR_00_002_LoginController : ControllerBase
{
    private readonly I_00UsersAccess _users;
    private readonly I_00_001_LoginAccess _login;

    public HR_00_002_LoginController(I_00UsersAccess users, I_00_001_LoginAccess login)
    {
        _users = users;
        _login = login;
    }

    [HttpGet("loginByLoginName/{loginname}/{password}/{schema}/{conn}")]
    public async Task<ActionResult<UsersModel>> _02LoginByLoginName(string loginname, string password, string schema = "Main", string conn = "MySqlConn")
    {
        try
        {
            var output = await _users._02LoginLoginName(loginname, password, schema, conn);
            return Ok(output);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("loginByEmail/{email}/{password}/{schema}/{conn}")]
    public async Task<ActionResult<UsersModel>> _02LoginByEmail(string email, string password, string schema = "Main", string conn = "MySqlConn")
    {
        try
        {
            var output = await _users._02LoginEmail(email, password, schema, conn);
            return Ok(output);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("Token/{id}/{userName}/{schema}")]
    public async Task<ActionResult<string>> _02Token(int id, string userName, string schema = "Main")
    {
        return await _login.CreateToken(id, userName, schema);
    }


}
