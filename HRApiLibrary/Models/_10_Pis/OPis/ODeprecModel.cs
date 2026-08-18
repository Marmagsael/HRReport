public class ODeprecModel
{
    public string? EmpNumber { get; set; }
    public string? TranNumber { get; set; }
    public int? DivId { get; set; }
    public int? DepId { get; set; }
    public int? SecId { get; set; }
    public int? PositionId { get; set; }
    public int? LeavegrpId { get; set; }
    public int? PayrollgrpId { get; set; }
    public int? IdDeployment { get; set; }
    public int? EmploymentTypeId { get; set; }
    public int? EmpStatusId { get; set; }
    public DateTime? DepDate { get; set; }
    public DateTime? DHired { get; set; }
    public DateTime? DRegularization { get; set; }
    public DateTime? DTraineeStart { get; set; }
    public DateTime? DTraineeEnd { get; set; }
    public DateTime? DContractualStart { get; set; }
    public DateTime? DContractualEnd { get; set; }
    public DateTime? DProbationaryStart { get; set; }
    public DateTime? DProbationaryEnd { get; set; }
    public DateTime? DRegularizationStart { get; set; }
    public DateTime? DRegularizationEnd { get; set; }
    public DateTime? DPermanentStart { get; set; }
    public DateTime? DResigned { get; set; }
    public DateTime? DTerminated { get; set; }
    public DateTime? DSeparated { get; set; }
    public string? Remarks { get; set; }
    public int? IsOnDeviation { get; set; }
    public int? IdDeviation { get; set; }
    public int? IsOnDiciplinary { get; set; }
    public int? IsOnInvestigation { get; set; }
    public int? IdInvestigate { get; set; }

    //ADDITIONAL
    public string? EmpName { get; set; }
}