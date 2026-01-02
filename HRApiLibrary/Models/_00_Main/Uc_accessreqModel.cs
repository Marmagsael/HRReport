using Blazorise;
using System;

namespace HRApiLibrary.Models._00_Main;

public class Uc_accessreqModel
{
    public int          Id                  { get; set; }
    public int          Empmasid            { get; set; }
    public int          Requestedbyid       { get; set; }
    public int          Ucrequestingid      { get; set; }
    public DateTime     Daterequested       { get; set; }
    public DateTime     DateApproved        { get; set; }
    public int          Allowed             { get; set; }
    public int          Ainfo               { get; set; }
    public int          ApersonalData       { get; set; }
    public int          Aaddress            { get; set; }
    public int          Aeducaion           { get; set; }
    public int          Afamily             { get; set; }
    public int          Areferences         { get; set; }
    public int          Aemployment         { get; set; }
    public int          Atrainings          { get; set; }
    
    //------------------------------------------------------------------------
    public bool         Selected             { get; set; } = false; 
                                                 
    public bool         BAllowed             { get; set; } = false;
    public bool         BAinfo               { get; set; } = false;
    public bool         BApersonalData       { get; set; } = false;
    public bool         BAaddress            { get; set; } = false;
    public bool         BAeducaion           { get; set; } = false;
    public bool         BAfamily             { get; set; } = false;
    public bool         BAreferences         { get; set; } = false;
    public bool         BAemployment         { get; set; } = false;
    public bool         BAtrainings          { get; set; } = false;

    //------------------------------------------------------------------------
    public string?      RequestorName       { get; set; } = string.Empty;
    public string?      EmpmasName          { get; set; } = string.Empty;
    public string?      CompanyName         { get; set; } = string.Empty;
    public string?      RequestorFirstName  { get; set; } = string.Empty;
    public string?      RequestorEmail      { get; set; } = string.Empty;
    public string?      EmpmasFirstName     { get; set; } = string.Empty;
    public string?      EmpmasEmail         { get; set; } = string.Empty;
   
}