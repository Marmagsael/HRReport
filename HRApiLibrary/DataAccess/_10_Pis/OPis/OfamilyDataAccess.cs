using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using System.Xml.Linq;

namespace HRApiLibrary.DataAccess._10_Pis.OPis
{
    public class OFamilyDataAccess : IOFamilyDataAccess
    {
        private readonly I_90_001_MySqlDataAccess _sql;

        public OFamilyDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<OFamilyModel?> _01(OFamilyModel family, string? schema, string? conn)
        {
            string? sql = $@"Insert into {schema}.Family (EMPNUMBER, NAME, BIRTH, RELATION) values (@EMPNUMBER, @NAME, @BIRTH, @RELATION)";
            await _sql.ExecuteCmd<dynamic>(sql, family, conn);

            sql = $@"SELECT * FROM {schema}.Family WHERE EMPNUMBER = @EMPNUMBER";

            var res = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { family.EmpNumber}, conn);

            return res.FirstOrDefault();
        }


        public async Task<List<OFamilyModel?>?> _02(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, NAME, BIRTH, RELATION from {schema}.Family where Empnumber = @Empnumber";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data;
        }


        public async Task<List<OFamilyModel?>?> _02CheckExisting(string? empnumber, string? name, string? Relation, string? schema, string? conn)
        {
            string? sql = $@"select  EMPNUMBER, NAME, BIRTH, RELATION from {schema}.Family  where Empnumber = @Empnumber AND NAME = @Name AND Relation = @Relation";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Empnumber = empnumber, Name = name, Relation = Relation }, conn);
            return data;
        }


        public async Task<OFamilyModel?> _03(int? id, OFamilyModel family, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Family set EMPNUMBER = @EMPNUMBER, NAME = @NAME, BIRTH = @BIRTH, RELATION = @RELATION where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, family, conn);

            sql = $@" select  * from {schema}.Family x where x.Id = @Id ;";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OFamilyModel?> _03(string? empnumber, string name, string Relation,  OFamilyModel family, string? schema, string? conn)
        {
            string? sql = $@"Update {schema}.Family set EMPNUMBER = @EMPNUMBER, NAME = @NAME, BIRTH = @BIRTH, RELATION = @RELATION where Empnumber = @Empnumber AND Name = @OldName AND Relation = @OldRela;";
            var parameters = new
            {
                family.EmpNumber,
                family.Relation,
                family.Name,
                OldEmpnumber = empnumber,
                OldName = name,
                OldRela = Relation
            };
            await _sql.ExecuteCmd<dynamic>(sql, family, conn);

            sql = $@" select  * from {schema}.Family x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OFamilyModel?> _04(string? empnumber, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Family where Empnumber = @Empnumber;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber }, conn);

            sql = $@" select  * from {schema}.Family x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<OFamilyModel?> _04(string? empnumber, string? name, string? Relation, string? schema, string? conn)
        {
            string? sql = $@"Delete from {schema}.Family where Empnumber = @Empnumber AND NAME = @Name AND Relation = @Relation;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Empnumber = empnumber, Name = name, Relation = Relation }, conn);

            sql = $@" select  * from {schema}.Family x where x.Empnumber = @Empnumber ;";
            var data = await _sql.FetchData<OFamilyModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
            return data?.FirstOrDefault();
        }
    }
}

public interface IOFamilyDataAccess
{
    Task<OFamilyModel?> _01(OFamilyModel family, string? schema, string? conn);
    Task<List<OFamilyModel?>?> _02(string? empnumber, string? schema, string? conn);
    Task<List<OFamilyModel?>?> _02CheckExisting(string? empnumber, string? name, string? Relation, string? schema, string? conn);
    Task<OFamilyModel?> _03(int? id, OFamilyModel family, string? schema, string? conn);
    Task<OFamilyModel?> _03(string? empnumber, string name, string Relation, OFamilyModel family, string? schema, string? conn);
    Task<OFamilyModel?> _04(string? empnumber, string? schema, string? conn);
    Task<OFamilyModel?> _04(string? empnumber, string? name, string? Relation, string? schema, string? conn);
}
