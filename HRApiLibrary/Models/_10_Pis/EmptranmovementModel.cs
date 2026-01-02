namespace HRApiLibrary.Models._10_Pis;

public class EmptranmovementModel
{
    public int id { get; set; }

    public int EmpmasId { get; set; }

    public DateTime? MovDate { get; set; }

    public string? MovNumber { get; set; }
    public int UserId { get; set; }

    public DateTime? DateRecorded { get; set; }
    public DateTime? TranStart { get; set; }

    public DateTime? TranEnd { get; set; }

    public string? Remarks { get; set; }

    public int EmpStatusId { get; set; }
    public string? EmpStatus { get; set; }
}
