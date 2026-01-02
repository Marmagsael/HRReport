namespace HRApiLibrary.Models._10_Pis;

public class LeavecreditModel
{
    public int Year { get; set; }

    public string? EmpmasId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateTime AnnivStart { get; set; }

    public DateTime AnnivEnd { get; set; }

    public double Credit { get; set; }

    public double Consumed { get; set; }
}
