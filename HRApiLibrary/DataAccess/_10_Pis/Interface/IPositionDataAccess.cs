using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface; 

public interface IPositionDataAccess
{
    Task<PositionModel?> _01(PositionModel position, string schema, string conn);
    Task<PositionModel?> _02(int id, string schema, string conn);
    Task<List<PositionModel?>?> _02(string schema, string conn);
    Task<PositionModel?> _03(int id, PositionModel position, string schema, string conn);
    Task<PositionModel?> _04(int id, string schema, string conn);
}