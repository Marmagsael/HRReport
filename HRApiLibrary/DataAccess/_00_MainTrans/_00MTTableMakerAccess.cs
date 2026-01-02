using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._00_MainTrans;

public class _00MTTableMakerAccess : I_00MTTableMakerAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00MTTableMakerAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async void _01MainTransactionTable(string connName = "MySqlConn")
    {
        await _01CreateSchema("MainTrans", connName);
        await _01InfoAccessRequest("MainTrans",connName);
    }
    
    
    
    
    /**** Private Class ********************************************************/
    // --- Schema Main Pis ------------------------------------------------------
    private async Task _01CreateSchema(string schema, string connName)
    {
        string? sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01InfoAccessRequest(string db, string connName) {
        string? sql = $@"create table  IF NOT EXISTS {db}.InfoAccessRequest
                            (   Id             int auto_increment,
                                UsercompanyId  int         null,
                                EmpmasSystemId int         null,
                                EmpmasId       int         null,
                                Email          VARCHAR(60) null,
                                ApprovedByEmpmasId   int         null,
                                DateRequested  DateTime,
                                DateApproved   DateTime, );";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }


}
