using HRApiLibrary.DataAccess._00_CT.Interfaces;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_CT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._00_CT;

public class _00_CTDataAccess : I_00_CTDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00_CTDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async Task<List<string?>?> _02Dbs(string conn)
    {

        string sql  = $@"show databases";
        var data    = await _sql.FetchData<string?, dynamic>(sql, new { }, conn);
        return data;

    }
    
    public async Task<List<string?>?> _02Tbls(string schema, string conn)
    {

        string sql  = $@"show tables from {schema}";
        var data    = await _sql.FetchData<string?, dynamic>(sql, new { }, conn);
        return data;

    }

    public async Task<List<TableFieldsModel?>?> _02TblFields(string tblName, string schema, string conn)
    {

        string sql  = $@"desc {schema}.{tblName}";
        var data    = await _sql.FetchData<TableFieldsModel?, dynamic>(sql, new { }, conn);
        return data;

    }

}
