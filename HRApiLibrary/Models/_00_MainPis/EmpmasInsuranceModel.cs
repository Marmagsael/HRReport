namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasInsuranceModel
{
    public int Id { get; set; }

    public string? INSURANCE { get; set; }

    public string? PolicyNo { get; set; }

    public string? FaceValue { get; set; }

    public string? Premium { get; set; }

    public DateTime InsExpire { get; set; }
}
