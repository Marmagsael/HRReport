using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using System;

namespace HRApiLibrary.DataAccess._10_Pis;

public class ParaDataAccess : IParaDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ParaDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ParaModel?> _01(ParaModel para, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Para (Year, Month, DepCtr) values (@Year, @Month, @DepCtr)";
        await _sql.ExecuteCmd<dynamic>(sql, para, conn);

        sql = $@"SELECT * FROM {schema}.Para WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<ParaModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<ParaModel?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Para ";
        var data = await _sql.FetchData<ParaModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ParaModel?> _02(string mode, string schema, string conn)
    {
        // Step 1: Describe the table
        string descSql = $@"DESC {schema}.Para";
        var columns = await _sql.FetchData<dynamic, dynamic>(descSql, new { }, conn);
        var columnName = mode + "Ctr";

        // Step 2: Check if the column exists
        bool columnExists = columns.Any(c => c.Field == columnName);

        // Step 3: If it doesn't exist, alter the table
        if (!columnExists)
        {
            string alterSql = $@"ALTER TABLE {schema}.Para ADD {columnName} INT DEFAULT 0;";
            await _sql.ExecuteCmd<dynamic>(alterSql, new { }, conn);
        }

        // Step 4: Fetch the data
        string sql = $@"SELECT id, year, month, {columnName} ctrName FROM {schema}.Para";
        var datas = await _sql.FetchData<ParaModel?, dynamic>(sql, new { }, conn);


        if (!datas.Any())
        {
             sql = $@"Insert INTO {schema}.PARA (year, month, {columnName}) VALUES ('0','0',0)"; 
             sql = $@"{sql}; SELECT id, year, month, {columnName} ctrName FROM {schema}.Para";
             datas = await _sql.FetchData<ParaModel?, dynamic>(sql, new { }, conn);
        }

            string yy = DateTime.Now.Year.ToString().Substring(2);
            string mm = DateTime.Now.Month.ToString();

            var data = datas.FirstOrDefault();
            if (yy != data?.Year || mm != data?.Month)
            {
                sql = $@"Update {schema}.para set year ={yy}  , month = {mm}, {columnName} = 1";
            }
            else
            {
                sql = $@"Update {schema}.para set year = {yy}  , month = {mm}, {columnName} = {columnName} + 1";

            }

            sql = $@"{sql}; SELECT id, year, month, {columnName} ctrName FROM {schema}.Para";
            datas = await _sql.FetchData<ParaModel?, dynamic>(sql, new { }, conn);
            return datas.FirstOrDefault();
        

            
    }

    public async Task<ParaModel?> _03(int id, ParaModel para, string schema, string conn)
    {
        string sql = $@"Update {schema}.Para set Year = @Year, Month = @Month, DepCtr = @DepCtr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, para, conn);

        sql = $@" select  * from {schema}.Para x where x.Id = @Id ;";
        var data = await _sql.FetchData<ParaModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<ParaModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Para where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Para x where x.Id = @Id ;";
        var data = await _sql.FetchData<ParaModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
