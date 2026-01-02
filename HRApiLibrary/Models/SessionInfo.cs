namespace HRApiLibrary.Models; 

public class SessionField
{
    public const string AMSSchema       = "AMSSchema";
    public const string ApplicantSchema = "ApplicantSchema";
    public const string PISSchema       = "PISSchema";
    public const string PaySchema       = "PaySchema";
}

public class SessionInfo
{
    public string? AMSSchema        { get; set; } = string.Empty;
    public string? ApplicantSchema  { get; set; } = string.Empty;
    public string? PISSchema        { get; set; } = string.Empty;
    public string? PaySchema        { get; set; } = string.Empty;

}