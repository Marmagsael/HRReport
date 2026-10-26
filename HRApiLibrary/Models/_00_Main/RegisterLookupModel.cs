namespace HRApiLibrary.Models._00_Main;

// -- Register / Invite : the company found by its code or owner --
public class V01CompanyLookup
{
    public int      Id              { get; set; }
    public string?  CompanyName     { get; set; }
    public string?  PISSchema       { get; set; }
    public string?  OldPis          { get; set; }   // old PIS schema : its Empmas.email is used by Invite User
    public int      CompanyUserTypeId { get; set; } // role of the logged-in user (CurrentAdminCompany) : 1 Owner, 5 System Admin
}

// -- Register as Employee : the employee found in {OldPis}.Empmas, plus its {PISSchema}.Empmas row when there is one --
public class V01EmpmasLookup
{
    public int      Id              { get; set; }   // {PISSchema}.Empmas.Id, 0 = no row yet
    public int?     SystemId        { get; set; }   // {PISSchema}.Empmas.SystemId
    public string?  EmpNumber       { get; set; }
    public string?  LastName        { get; set; }
    public string?  FirstName       { get; set; }
    public string?  MiddleName      { get; set; }
    public string?  Suffix          { get; set; }
    public string?  Email           { get; set; }   // {OldPis}.Empmas.email
    public string?  EmpStatus       { get; set; }   // {OldPis}.empstat.NAME
    public int      IsResigned      { get; set; }   // {OldPis}.empstat.ISRESIGNED
}
