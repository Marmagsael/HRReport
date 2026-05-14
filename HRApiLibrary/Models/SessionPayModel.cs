using HRApiLibrary.DataAccess._00_Main.Interface;

namespace HRApiLibrary.Models; 



public class SessionPayModel
{
    public const string Id              = "Id";
    public const string Username        = "Username";
    public const string AMSSchema       = "AMSSchema";
    public const string ApplicantSchema = "ApplicantSchema";
    public const string PISSchema       = "PISSchema";
    public const string PaySchema       = "PaySchema";


    public const string? SessionKeyUsername = "SessionKeyUsername";
    public const string? SessionKeyId       = "SessionKeyId";
    public const string? Schema             = "DB";
    public const string? Conn               = "MySqlConn";

    
}

public enum SessionPayEnum
{
    SessionKeyUsername  = 0, 
    SessionKeyId        = 1, 
    Schema              = 0,
    Conn                = 0
}
