using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._11_AMS;
using HRApiLibrary.Modules._11003O;

namespace HRMvc.Applications.AMS.Vars;

public class V12002Model
{
    // --- Payroll Group ---
    public List<M11003_Payrollgrp> PayrollGrps { get; set; } = [];
    public int? SelectedPayrollGrpId { get; set; }

    // --- BioManHourHdr (Coverage) ---
    public BioManHourHdrModel Hdr { get; set; } = new();

    // --- BioManHour Grid (upper) ---
    public List<BioManHourModel> ManHours { get; set; } = [];

    // --- BioDailyPunches Grid (lower) ---
    public List<BioDailyPunchesModel> DailyPunches { get; set; } = [];

    // --- Load Attendance Modal ---
    public bool IsLoadModalOpen                     { get; set; } = false;
    public string DeviceNo                          { get; set; } = "1";
    public string? AttlogContent                    { get; set; }
    public List<BiologModel> ParsedLogs             { get; set; } = [];

    // --- Add Employee Modal ---
    public bool IsAddEmpModalOpen                   { get; set; } = false;
    public List<M11003_Empmas> SearchedEmps         { get; set; } = [];
    public string EmpSearchKeyword                  { get; set; } = string.Empty;

    // --- Remove Employee Confirmation ---
    public bool IsRemoveEmpConfirmOpen              { get; set; } = false;

    // --- Upload Excel Modal ---
    public bool IsUploadExcelModalOpen              { get; set; } = false;
    public bool AllowAddFromExcel                   { get; set; } = false;
    public bool AllowRemoveFromExcel                { get; set; } = false;
    public List<BioManHourModel> ExcelToAdd         { get; set; } = [];
    public List<BioManHourModel> ExcelToUpdate      { get; set; } = [];
    public List<BioManHourModel> ExcelToRemove      { get; set; } = [];

    // --- Busy / Notification ---
    public bool IsBusy { get; set; } = false;
    public (string Type, string Message) Notification { get; set; } = (string.Empty, string.Empty);
}
