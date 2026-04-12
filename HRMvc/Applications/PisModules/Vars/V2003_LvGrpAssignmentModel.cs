using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class V2003_LvGrpAssignmentModel
{
    public LeavegrpModel                    Leavegrp                { get; set; } = new(); 
    public List<LeavegrpModel>?             Leavegrps               { get; set; } = [];
    
    public EmpmasgrpModel                   Empmasgrp               { get; set; } = new(); 
    public List<EmpmasgrpModel>?            Empmasgrps              { get; set; } = [];
    
    public LvcreditModel                   LvCredit                 { get; set; } = new(); 
    public List<LvcreditModel>?             LvCredits               { get; set; } = [];
    public LeavetypeModel                   LvType                  { get; set; } = new(); 
    public List<LeavetypeModel>?            LvTypes                 { get; set; } = [];


    
    //-------------------------------------------------------------------------------------
    public string?                  ErrorMsg            { get; set; } = string.Empty;
    public bool                     ShowAddMember       { get; set; } = false;
    public bool                     ShowApproverEntry   { get; set; } = false;
    public bool                     WithChanges         { get; set; } = false;



}
