namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasEmploymentModel
{
    public int Id { get; set; }

    public int EmpmasId { get; set; }

    public string? CompName { get; set; }

    public string? Address { get; set; }

    public string? Tel { get; set; }

    public string? Pos { get; set; }

    public DateTime From_ { get; set; }

    public DateTime To_ { get; set; }

    public Double Sal { get; set; }

    public string? Rem { get; set; }
}
