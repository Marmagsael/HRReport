using System;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._90_Utils;
using System.Globalization;
using DocumentFormat.OpenXml.Bibliography;

namespace HRMvc.Applications.Vars;

public class V00_RepVars
{
    public bool                         Isloaded                { get; set; } = false; 
    public int                          RefreshToken            { get; set; } = 0;
    public string                       SelectedFormat          { get; set; } = "PDF";
    public string                       RelativePath            { get; set; } = "Reports/_03Payroll/mSample.trdp";
    public IEnumerable<object>          ReportData              { get; set; } = [];

    public Dictionary<string, object>   ReportParams            { get; set; } = new()
    {
        ["CoName"]     = "Morpheusbox Inc.",
        ["pPrintedBy"] = "SYSTEM",
        ["pPrintDate"] = DateTime.Now
    };
    public List<string>                 ReportFormats           { get; set; } = ["PDF","XLSX"];


    //Newly Added ------------------------------------------------
    public int                          SelectedFilter          { get; set; } = 0;
    public List<DataFilterModel>        DataFilters             { get; set; } = [];
    public class DataFilterModel
    {
        public int      Code { get; set; } = 0;
        public string?  Name { get; set; } = "";
    }


    //-----------------------------------------------------------

    //--------------------------------------------------------------------------------------
    public int                  Year            { get; set; } = DateTime.Now.Year;
    public string               Month           { get; set; } = DateTime.Now.ToString("MM");
    public string               Period          { get; set; } = DateTime.Now.Day <= 15 ? "01" : "02";
    public DateTime?            DStart          { get; set; } = DateTime.Now;
    public DateTime?            DEnd            { get; set; } = DateTime.Now;


    //---------------------------------------------------------------------------------------
    // ✅ Integrated defaults
    public List<YearsModel>     Years           { get; set; } = GetDefaultYears();
    public List<MonthsModel>    Months          { get; set; } = GetDefaultMonths();
    public List<PeriodModel>    Periods         { get; set; } = GetDefaultPeriods();

    //---------------------------------------------------------------------------------------
    public string               ClNumber        { get; set; } = "";
    public List<OClientModel>   Clients         { get; set; } = [];
    public string               Empstat_        { get; set; } = "";
    public List<OEmpstatModel?>  Empstats       { get; set; } = [];


    //--- Modal Details ----------------------------------------------
    public bool                 ShowModal       { get; set; } = false;  
    public bool                 ShowReport      { get; set; } = false;  
    public string               MCaption        { get; set; } = "-";  

    


    //---------------------------------------------------------------------------------------
    // Helpers
    //=======================================================================================
    private static List<YearsModel> GetDefaultYears() =>
            Enumerable.Range(0, 10).Select(i =>
            { var y = DateTime.Now.Year - i; return new YearsModel { Year = y, Name = y.ToString() };
            }).ToList();


    
    private static List<MonthsModel> GetDefaultMonths() =>
            Enumerable.Range(1, 12).Select(m =>
            { var d = CultureInfo.InvariantCulture.DateTimeFormat;
            return new MonthsModel { Month = m, Code = m.ToString("D2"), SName = d.GetAbbreviatedMonthName(m), Name = d.GetMonthName(m) };
            }).ToList();

    private static List<PeriodModel> GetDefaultPeriods() =>
            Enumerable.Range(1, 5).Select(p =>
            { return new PeriodModel { Id = p, PdNumber = p.ToString("D2"), PdName = $"Period {p}" }; }
            ).ToList();


    

}
