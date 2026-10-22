namespace HRApiLibrary.Models._10_Pis;

// DutyType : R - Regular Working, RN - Regular Normal Working, RD - Restday
public class ObapplicationdtlModel
{
    public int?             Id                  { get; set; } = 0;
    public int?             ObApplicationId     { get; set; } = 0;
    public int?             EmpmasId            { get; set; } = 0;
    public DateTime?        Date                { get; set; } = DateTime.Now.Date;
    public string?          DutyType            { get; set; } = "R";
    public int?             ObDurationId        { get; set; } = 1;
    public int?             IsPayable           { get; set; } = 1;

    //---------------------------------------------------------------------------------
    public bool             IsPayableB          { get => IsPayable == 1; set => IsPayable = value ? 1 : 0; }
}
