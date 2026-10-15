using HRApiLibrary.Models._11_AMS;
using HRApiLibrary.Modules._11003O;

namespace HRMvc.Applications.AMS.Vars;

public class V11003CSModel
{
    // Payroll Group
    public List<M11003_Payrollgrp>          PayrollGrps          { get; set; } = [];
    public int?                             SelectedPayrollGrpId { get; set; } = null;

    // Employee List
    public List<M11003CS_Atttemplate>       Employees            { get; set; } = [];
    public List<M11003CS_Atttemplate>       SelectedEmployees    { get; set; } = [];

    // Weekly Schedule Headers (for Template dropdown)
    public List<AttschedweeklyhdrModel>     WeeklyHdrs           { get; set; } = [];

    // Change Schedule Modal
    public bool                             IsModalVisible       { get; set; } = false;
    public bool                             IsModalVisible1      { get; set; } = false;
    public int?                             SelectedTemplateId   { get; set; } = null;
    public string?                          SelectedTemplateName { get; set; } = null;
    public DateTime                         Effectivity          { get; set; } = DateTime.Today;
    public DateTime                         EffectivityEnd       { get; set; } = DateTime.Today.AddDays(7);
    public AttschedweeklydtlModel?          SelectedDtl          { get; set; } = null;

    // UI Flags
    public bool                             IsBusy               { get; set; } = false;

    // Cross-component notification
    public (string Type, string Message) Notification { get; set; } = (string.Empty, string.Empty);
}
