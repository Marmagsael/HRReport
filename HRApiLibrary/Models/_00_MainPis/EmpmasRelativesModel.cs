namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasRelativesModel
{
    public int Id { get; set; }

    public int EmpmasId { get; set; }

    public string? Name { get; set; }

    public DateTime Birth { get; set; } = DateTime.Now;

    public string? Sex { get; set; }

    public string? RelativesRefCode { get; set; }

    //Added ------------
    public string Gender        { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
}
