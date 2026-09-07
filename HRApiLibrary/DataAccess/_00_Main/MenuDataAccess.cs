using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;


namespace HRApiLibrary.DataAccess._00_Main;

public class MenuDataAccess : IMenuDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MenuDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MenuModel?> _01(MenuModel menu, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Menu (Type, IdParent, Indent, Icon, DispText, Action, Odr) values (@Type, @IdParent, @Indent, @Icon, @DispText, @Action, @Odr)";
        await _sql.ExecuteCmd<dynamic>(sql, menu, conn);

        sql = $@"SELECT * FROM {schema}.Menu WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MenuModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MenuModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Type, IdParent, Indent, Icon, DispText, Action, Odr from {schema}.Menu where Id = @Id";
        var data = await _sql.FetchData<MenuModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

  

    public async Task<List<MenuModel?>?> _02ByType(string type, string schema, string conn)
    {
        string sql = $@" SELECT Id, Type, IdParent, Indent, Icon, DispText, Action, Odr FROM {schema}.Menu WHERE LOWER(Type) LIKE CONCAT(LOWER(@Type), '%')";
        var data = await _sql.FetchData<MenuModel?, dynamic>(sql, new { Type = type }, conn);
        return data;
    }

    public async Task<List<MenuModel?>?> _02ByIdParent(int? idParent, string schema, string conn)
    {
        string sql = $@" SELECT Id, Type, IdParent, Indent, Icon, DispText, Action, Odr FROM {schema}.Menu WHERE IdParent = @IdParent";

        var data = await _sql.FetchData<MenuModel?, dynamic>(sql, new { IdParent = idParent }, conn);
        return data;
    }


    public async Task<MenuModel?> _03(int id, MenuModel menu, string schema, string conn)
    {
        string sql = $@"Update {schema}.Menu set Type = @Type, IdParent = @IdParent, Indent = @Indent, Icon = @Icon, DispText = @DispText, Action = @Action, Odr = @Odr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, menu, conn);

        sql = $@" select  * from {schema}.Menu x where x.Id = @Id ;";
        var data = await _sql.FetchData<MenuModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MenuModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Menu where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Menu x where x.Id = @Id ;";
        var data = await _sql.FetchData<MenuModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
