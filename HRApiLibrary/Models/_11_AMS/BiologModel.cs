namespace HRApiLibrary.Models._11_AMS;

public class BiologModel
{
    public long     Id              { get; set; }
    public string   DeviceNo        { get; set; } = string.Empty;
    public string   BiometricEmpNo  { get; set; } = string.Empty;
    public DateTime LogDatetime     { get; set; }
    public DateOnly PunchDate       { get; set; }
    public int      VerifyMode      { get; set; }
    public int      AttState        { get; set; }
    public string?  WorkCode        { get; set; }
    public string   ImportBatch     { get; set; } = string.Empty;
    public DateTime ImportedOn      { get; set; }
    public bool     IsProcessed     { get; set; }

    public string VerifyModeLabel => VerifyMode switch
    {
        0  => "Password",
        1  => "Fingerprint",
        2  => "Card",
        3  => "PW + FP",
        4  => "PW + Card",
        5  => "FP + Card",
        6  => "PW + FP + Card",
        10 => "Face",
        11 => "Face + PW",
        12 => "Face + Card",
        13 => "Face + FP",
        14 => "Face + FP + Card",
        15 => "Face + FP + PW + Card",
        _  => $"Unknown ({VerifyMode})"
    };

    public string AttStateLabel => AttState switch
    {
        0 => "Check-In",
        1 => "Check-Out",
        2 => "Break-Out",
        3 => "Break-In",
        _ => $"Unknown ({AttState})"
    };
}
