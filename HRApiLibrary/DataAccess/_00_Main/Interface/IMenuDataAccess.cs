using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface IMenuDataAccess
    {
        Task<MenuModel?> _01(MenuModel menu, string schema, string conn);
        Task<MenuModel?> _02(int id, string schema, string conn);
        Task<List<MenuModel?>?> _02ByType(string type, string schema, string conn);
        Task<List<MenuModel?>?> _02ByIdParent(int? idParent, string schema, string conn);
        Task<MenuModel?> _03(int id, MenuModel menu, string schema, string conn);
        Task<MenuModel?> _04(int id, string schema, string conn);
    }
}