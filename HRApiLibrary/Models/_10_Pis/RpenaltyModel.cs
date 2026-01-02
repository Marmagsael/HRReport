namespace HRApiLibrary.Models._10_Pis;

public class RpenaltyModel
{

    public int Id { get; set; }
    public string? DEV_NO { get; set; }
    public string? FREQ { get; set; }
    public string? PENALTY_NO { get; set; }
    public string? DESC_ { get; set; }
    public string Resetregref { get; set; } = string.Empty;
    public string Isterminated { get; set; } = string.Empty;
    public int Days { get; set; }
}
public class RpenaltyIsModel
{

    public int Id { get; set; }
    public string? DEV_NO { get; set; }
    public string? FREQ { get; set; }
    public string? PENALTY_NO { get; set; }
    public string? DESC_ { get; set; }
    public bool Resetregref { get; set; } = false;
    public bool Isterminated { get; set; } = false;
    public int Days { get; set; }
}
