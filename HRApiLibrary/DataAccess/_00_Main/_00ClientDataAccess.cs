using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._00_Main;


public class _00ClientDataAccess : I_00ClientDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _00ClientDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ClientModel?> _01(ClientModel client, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Client (clnumber, clname) values (@clnumber, @clname)";
        await _sql.ExecuteCmd<dynamic>(sql, client, conn);

        sql = $@"SELECT * FROM {schema}.Client WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<ClientModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<ClientModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  clnumber, clname from {schema}.Client where Id = @Id";
        var data = await _sql.FetchData<ClientModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<ClientModel?> _03(int id, ClientModel client, string schema, string conn)
    {
        string sql = $@"Update {schema}.Client set clnumber = @clnumber, clname = @clname where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, client, conn);

        sql = $@" select  * from {schema}.Client x where x.Id = @Id ;";
        var data = await _sql.FetchData<ClientModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ClientModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Client where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Client x where x.Id = @Id ;";
        var data = await _sql.FetchData<ClientModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}