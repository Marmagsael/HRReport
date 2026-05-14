namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasInsuranceModel
{
    public int Id { get; set; }

    public string? INSURANCE { get; set; }

    public string? PolicyNo { get; set; }

    public double? FaceValue { get; set; }

    public double? Premium { get; set; }

    public DateTime InsExpire { get; set; }
}
