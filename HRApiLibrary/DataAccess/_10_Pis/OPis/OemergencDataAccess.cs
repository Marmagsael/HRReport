using Blazorise;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using Org.BouncyCastle.Ocsp;
using System.Xml.Linq;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OEmergencDataAccess : IOEmergencDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OEmergencDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OEmergencModel?> _01(OEmergencModel emergenc, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Emergenc (EMPNUMBER, NAME, ADDR, RELA, TEL) values (@EMPNUMBER, @NAME, @ADDR, @RELA, @TEL)";
            await _sql.ExecuteCmd<dynamic>(sql, emergenc, conn);

            sql = $@"SELECT * FROM {schema}.Emergenc WHERE EMPNUMBER = @EMPNUMBER";

            var res = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new { emergenc.EmpNumber }, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OEmergencModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, NAME, ADDR, RELA, TEL from {schema}.Emergenc where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new {Empnumber =  empnumber  }, conn);
            return data;
        }


        public async Task<List<OEmergencModel?>?> _02CheckExisting(string? empnumber, string? name, string? relationship, string? schema, string? conn)
        {
            string? sql = $@"select  * from {schema}.Emergenc where Empnumber = @Empnumber and Name = @Name and Rela = @Rela";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new { Empnumber = empnumber, Name = name , Rela = relationship }, conn);
            return data;
        }


        public async Task<OEmergencModel?> _03(int? id, OEmergencModel emergenc, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Emergenc set EMPNUMBER = @EMPNUMBER, NAME = @NAME, ADDR = @ADDR, RELA = @RELA, TEL = @TEL where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, emergenc, conn);

            sql = $@" select  * from {schema}.Emergenc x where x.Id = @Id ;";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OEmergencModel?> _03(string? empnumber, string? name, string? relationship,  OEmergencModel emergenc, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Emergenc set EMPNUMBER = @EMPNUMBER, NAME = @NAME, ADDR = @ADDR, RELA = @RELA, TEL = @TEL where Empnumber = @Empnumber AND Name = @Name AND Rela = @Rela;";
            var parameters = new
            {
                emergenc.EmpNumber,
                emergenc.Name,
                emergenc.Addr,
                emergenc.Rela,
                emergenc.Tel,
                OldEmpnumber = empnumber,
                OldName = name,
                OldRela = relationship
            };
            await _sql.ExecuteCmd<dynamic>(sql, parameters, conn);

            sql = $@" select  * from {schema}.Emergenc x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new {Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OEmergencModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Emergenc where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Emergenc x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }


        public async Task<OEmergencModel?> _04(string? empnumber, string? name, string? relationship, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Emergenc where  Empnumber = @Empnumber AND Name = @Name AND Rela = @Rela;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Emergenc x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OEmergencModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }
}


public interface IOEmergencDataAccess
{
    Task<OEmergencModel?> _01(OEmergencModel emergenc, string? schema, string? conn);
    Task<List<OEmergencModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OEmergencModel?>?> _02CheckExisting(string? empnumber, string? name, string? relationship, string? schema, string? conn);
    Task<OEmergencModel?> _03(int? id, OEmergencModel emergenc, string? schema, string? conn);
    Task<OEmergencModel?> _03(string? empnumber, string? name, string? relationship, OEmergencModel emergenc, string? schema, string? conn);
    Task<OEmergencModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OEmergencModel?> _04(string? empnumber, string? name, string? relationship, string? schema, string? conn);
}