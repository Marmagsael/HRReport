namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasFamilyModel
{
    public int Id { get; set; }

    public int EmpmasId { get; set; }

    public string? Name { get; set; }

    public DateTime Birth { get; set; } = DateTime.Now;

    public string? Sex { get; set; }

    public string? RelationCode { get; set; }

    //Added --------------------------
    public string? SexName { get; set; }
    public string? RelationshipName { get; set; }
}
