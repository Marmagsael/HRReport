using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._11_AMS;
using HRApiLibrary.Modules._11003O;

namespace HRMvc.Applications.AMS.Vars;

public class V11003ASModel
{
    // Payroll Group
    public List<M11003_Payrollgrp> PayrollGrps { get; set; } = [];
    public int? SelectedPayrollGrpId { get; set; } = null;

    // Employee List
    public List<M11003_Empmas> Employees { get; set; } = [];
    public M11003_Empmas? SelectedEmployee { get; set; } = null;
    public IEnumerable<M11003_Empmas> SelectedEmployees { get; set; } = [];

    // Date Range
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(6);

    // Advance Schedules
    public List<AttadvancescheduleModel> AdvScheds { get; set; } = [];

    // Daily Schedule Templates (for Template dropdown)
    public List<AttscheddailyModel> DailyTemplates { get; set; } = [];

    // Current Schedule
    public AtttemplateModel CurrSched { get; set; } = new();
    public bool HasCurrSched { get; set; } = false;

    // UI Flags
    public bool IsBusy { get; set; } = false;
    public bool IsLoadingEmps { get; set; } = false;
    public bool IsLoadingAdvSched { get; set; } = false;

    // Add Schedule Modal
    public bool IsAddModalVisible { get; set; } = false;
    public DateTime NewDate { get; set; } = DateTime.Today;
    public string DateError { get; set; } = string.Empty;
    public List<DateTime> UsedDates { get; set; } = [];

    // Delete Confirmation
    public bool IsDeleteAdvVisible { get; set; } = false;
    public AttadvancescheduleModel? DeleteAdvRow { get; set; } = null;

    // Cross-component notification
    public (string Type, string Message) Notification { get; set; } = (string.Empty, string.Empty);
}
