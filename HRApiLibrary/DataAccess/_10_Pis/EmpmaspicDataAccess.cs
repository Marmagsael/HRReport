using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis
{
    public class EmpmaspicDataAccess : IEmpmaspicDataAccess
    {

        private readonly I_90_001_MySqlDataAccess _sql;

        public EmpmaspicDataAccess(I_90_001_MySqlDataAccess sql)
        {
            _sql = sql;
        }

        public async Task<EmpmaspicModel?> _01(EmpmaspicModel empmaspic, string schema, string conn)
        {
            string sql = $@"Insert into {schema}.Empmaspic (EmpmasId, PicName, PicAddress, Category, Validity, Mode, Created, UploadedBy) values (@EmpmasId, @PicName, @PicAddress, @Category, @Validity, @Mode, @Created, @UploadedBy)";
            await _sql.ExecuteCmd<dynamic>(sql, empmaspic, conn);

            sql = $@"SELECT * FROM {schema}.Empmaspic WHERE ID = (SELECT @@IDENTITY)";

            var res = await _sql.FetchData<EmpmaspicModel?, dynamic>(sql, new { }, conn);

            return res.FirstOrDefault();
        }


        public async Task<EmpmaspicModel?> _02(int id, string schema, string conn)
        {
            string sql = $@"select  Id, EmpmasId, PicName, PicAddress, Category, Validity, Mode, Created, UploadedBy from {schema}.Empmaspic where Id = @Id";
            var data = await _sql.FetchData<EmpmaspicModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<EmpmaspicModel?> _02ByEmpmasId(int? empmasId, string category, string schema, string conn)
        {
            string sql = $@"select  Id, EmpmasId, PicName, PicAddress, Category, Validity, Mode, Created, UploadedBy from {schema}.Empmaspic where EmpmasId = @EmpmasId and Category = @Category" ;
            var data = await _sql.FetchData<EmpmaspicModel?, dynamic>(sql, new { EmpmasId = empmasId , Category = category}, conn);
            return data?.FirstOrDefault();
        }



        public async Task<EmpmaspicModel?> _03(int? id, EmpmaspicModel empmaspic, string schema, string conn)
        {

            Console.WriteLine($" iD >>>>>>>> {id}");
            empmaspic.Id = id;

            string sql = $@"Update {schema}.Empmaspic set EmpmasId = @EmpmasId, PicName = @PicName, PicAddress = @PicAddress, Category = @Category, Validity = @Validity, Mode = @Mode, Created = @Created, UploadedBy = @UploadedBy where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, empmaspic, conn);

            sql = $@" select  * from {schema}.Empmaspic x where x.Id = @Id ;";
            var data = await _sql.FetchData<EmpmaspicModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }

        public async Task<EmpmaspicModel?> _04(int? id, string schema, string conn)
        {
            string sql = $@"Delete from {schema}.Empmaspic where Id = @Id;";
            await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

            sql = $@" select  * from {schema}.Empmaspic x where x.Id = @Id ;";
            var data = await _sql.FetchData<EmpmaspicModel?, dynamic>(sql, new { Id = id }, conn);
            return data?.FirstOrDefault();
        }
    }

    public interface IEmpmaspicDataAccess
    {
        Task<EmpmaspicModel?> _01(EmpmaspicModel empmaspic, string schema, string conn);
        Task<EmpmaspicModel?> _02(int id, string schema, string conn);
        Task<EmpmaspicModel?> _02ByEmpmasId(int? empmasid, string category, string schema, string conn);
        Task<EmpmaspicModel?> _03(int? id, EmpmaspicModel empmaspic, string schema, string conn);
        Task<EmpmaspicModel?> _04(int? id, string schema, string conn);
    }
}
