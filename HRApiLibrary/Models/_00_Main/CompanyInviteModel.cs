namespace HRApiLibrary.Models._00_Main;

// -- main.CompanyInvite : same columns as main.CompanyUsers --
public class CompanyInviteModel
{
    public int          UserId                  { get; set; }
    public int          CompanyId               { get; set; }
    public string?      Status                  { get; set; }   // I invited, A accepted, D declined
    public DateTime?    DateInvited             { get; set; }
    public DateTime?    DateAccepted            { get; set; }
    public int          CompanyUserTypeId       { get; set; } = 2;  // 1 Owner, 2 User
    public int          InvitedById             { get; set; }
    public bool         WithPISAccess           { get; set; }
    public bool         WithPayAccess           { get; set; }
    public bool         WithAMSAccess           { get; set; }
    public bool         WithAccountingAccess    { get; set; }
    public bool         WithApplicantAccess     { get; set; }
    public int          EmpmasId                { get; set; }
}

// -- 12.003 My Invitations : an invite sent to the logged-in user --
public class MyInvitationModel
{
    public int          CompanyId               { get; set; }
    public string?      CompanyName             { get; set; }
    public string?      CompanyCode             { get; set; }
    public string?      InvitedBy               { get; set; }   // inviter's email
    public DateTime?    DateInvited             { get; set; }
}

// -- 01.08 Registration Requests : an employee registered himself, waiting for the owner (CompanyInvite P) --
public class RegistrationRequestModel
{
    public int          UserId                  { get; set; }
    public int          EmpmasId                { get; set; }
    public string?      EmpNumber               { get; set; }
    public string?      LastName                { get; set; }
    public string?      FirstName               { get; set; }
    public string?      MiddleName              { get; set; }
    public string?      Email                   { get; set; }   // users.Email
    public DateTime?    DateInvited             { get; set; }   // date registered
}

// -- 01.09 Company Users : a member of the current company (main.CompanyUsers) --
public class CompanyMemberModel
{
    public int          UserId                  { get; set; }
    public string?      Email                   { get; set; }
    public string?      EmpNumber               { get; set; }
    public string?      LastName                { get; set; }
    public string?      FirstName               { get; set; }
    public int          CompanyUserTypeId       { get; set; }   // 1 Owner, 2 User, 5 System Admin
    public string?      UserType                { get; set; }
    public string?      Status                  { get; set; }
    public DateTime?    DateAccepted            { get; set; }
    public bool         WithPISAccess           { get; set; }
    public bool         WithPayAccess           { get; set; }
    public bool         WithAMSAccess           { get; set; }
    public bool         WithAccountingAccess    { get; set; }
    public bool         WithApplicantAccess     { get; set; }
}

// -- Invite User : an employee from the company's old PIS (the source of truth) --
public class InviteCandidateModel
{
    // -- {OldPis}.Empmas --
    public string?      EmpNumber               { get; set; }
    public string?      LastName                { get; set; }
    public string?      FirstName               { get; set; }
    public string?      MiddleName              { get; set; }
    public string?      Suffix                  { get; set; }
    public string?      Email                   { get; set; }
    public string?      EmpStatus               { get; set; }   // {OldPis}.empstat.NAME
    public int          IsResigned              { get; set; }   // {OldPis}.empstat.ISRESIGNED
    public int          UserId                  { get; set; }   // users.Id with that email, 0 = no account

    // -- {PISSchema}.Empmas of the same EmpNumber, 0 / null when there is no row yet --
    public int          EmpmasId                { get; set; }
    public int          LinkedUserId            { get; set; }   // Empmas.SystemId, 0 = not linked yet
    public string?      PisLastName             { get; set; }
    public string?      PisFirstName            { get; set; }
    public string?      PisMiddleName           { get; set; }
    public string?      PisSuffix               { get; set; }

    public string?      MemberStatus            { get; set; }   // companyusers.Status when the user is already in the company
    public string?      InviteStatus            { get; set; }   // companyinvite.Status of the last invite

    public V01EmpmasLookup OldPis() => new() { EmpNumber = EmpNumber, LastName = LastName, FirstName = FirstName, MiddleName = MiddleName, Suffix = Suffix };
    public V01EmpmasLookup NewPis() => new() { Id = EmpmasId, LastName = PisLastName, FirstName = PisFirstName, MiddleName = PisMiddleName, Suffix = PisSuffix };
}
