using HRApiLibrary.DataAccess._00_Login.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._90_Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRApiLibrary.DataAccess._00_Login;

public class _00_001_LoginAccess : I_00_001_LoginAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IConfiguration _config;
    public _00_001_LoginAccess(I_90_001_MySqlDataAccess sql, IConfiguration config)
    {
        _sql = sql;
        _config = config;
    }

    public async Task<LoginOutputModel?> _001_LoginByEmpNumber_CentralizedSchema(string schema, string empNumber, string password)
    {
        string sql = @$" select  e.Empnumber, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm,
                                c.clNumber, c.ClName,
                                s.code EmpStatCd, s.Name EmpStatus, s.IsResigned,  
                                Position_ PositionCd, p.Name as Position,
                                e.DateHired,
                                e.Sss,
                                e.Tin,
                                e.SecLicense License,
                                e.MovNumber,
                                e.Email,
                                e.passwd
                            from {schema}.empmas e 
                                 left join {schema}.client c on c.ClNumber = e.Client_ 
                                 left join {schema}.empstat s on s.code = e.empstat_ 
                                 left join {schema}.Position p on p.Code = e.Position_
                            where e.Empnumber = @Empnumber and passwd = sha2(@Password,512) ";

        var data = await _sql.FetchData<LoginOutputModel?, dynamic>(sql, new { Empnumber = empNumber, Password = password });

        return data.FirstOrDefault();

    }

    public async Task<UsersModel?> _011_LoginByLoginName_Main(string loginname, string password, string schema = "Main")
    {
        string sql = @" select  LoginName, Email, Domain 
                        from " + schema + @".Users e where e.LoginName = @LoginName and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = loginname, Password = password });
        return data.FirstOrDefault();
    }

    public async Task<UsersModel?> _012_LoginByEmail_Main(string email, string password, string schema = "Main")
    {
        string sql = @" select  LoginName, Email, Domain 
                        from " + schema + @".Users e where e.Email = @Email and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email, Password = password });
        return data.FirstOrDefault();
    }

    //*************************************************************************************************************
    // ---- Token Process Generation ------------------------------------------------------------------------------
    //*************************************************************************************************************
    public async Task<string> CreateToken(int id, string loginName, string schema="Main")
    {
        UsersModel? user = await ValidateUserCredential(id, loginName, schema);
        string token = string.Empty;
        if (user is not null) token = GenerateToken(user);
        return token;
    }


    //--- Create Token ------------------------------------------------------
    private async Task<UsersModel?> ValidateUserCredential(int id, string? loginName, string schema)
    {
        UsersModel? user = null;
        if (!string.IsNullOrWhiteSpace(loginName) && id > 0)
        {
            string sql  = @$"Select * from {schema}.Users e where Id = @Id and LoginName = @LoginName";
            var res     = await _sql.FetchData<UsersModel,dynamic>(sql,new {Id=id, LoginName = loginName });
            user        = res.FirstOrDefault();
        }
        return user;
    }

    //--- Token Generation --------------------------------------------------------------------------------------------------
    private string GenerateToken(UsersModel user)
    {
        string? skey            = _config.GetSection("Authentication:SecretKey").Value;
        var secretKey           = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(skey));
        var signingCredentials  = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.LoginName!)
        };

        string? issuer      = _config.GetSection("Authentication:Issuer").Value;
        string? audience    = _config.GetSection("Authentication:Audience").Value;
        string? expires     = _config.GetSection("Authentication:SecondsExpires").Value ?? "1";
        int ExpireInSeconds = int.Parse(expires);

        var token = new JwtSecurityToken(issuer,
                                        audience,
                                        claims,
                                        DateTime.UtcNow,
                                        DateTime.UtcNow.AddSeconds(ExpireInSeconds),
                                        signingCredentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
