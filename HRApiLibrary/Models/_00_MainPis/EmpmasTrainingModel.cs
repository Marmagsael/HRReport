namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasTrainingModel
{
    public int Id { get; set; }

    public int EmpmasId { get; set; }

    public string? ProgramName { get; set; }

    public string? DateTaken { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public string? TotalHrs { get; set; }

    public string? TotalDays { get; set; }

    public string? School { get; set; }

    public string? Trainor { get; set; }
}
