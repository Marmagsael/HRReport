using HRApiLibrary.DataAccess._90_Utils.Interface;
using Microsoft.Extensions.Configuration;

namespace HRApiLibrary.DataAccess._90_Utils;

public class _09_02_VarsGlobal : I_09_02_VarsGlobal
{
    private readonly IConfiguration _config;

    public _09_02_VarsGlobal(IConfiguration config)
    {
        _config = config;
    }


    public string DefConn()
    {
        return _config.GetSection("Schema:DefConn").Value.ToString();
    }

    public string SchemaMain()
    {
        return _config.GetSection("Schema:Main").Value.ToString(); 
    }

    public string SchemaMainPis()
    {
        return _config.GetSection("Schema:MainPis").Value.ToString(); 
    }
}