namespace HRApiLibrary.Models._00_Main; 

public class CompanyUserTypeModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int IsVisible { get; set; } = 0; // --- 1 - Basic, 2 - System User, 3 - Paid Users, 4 - Owner, 5 - Developer
}
