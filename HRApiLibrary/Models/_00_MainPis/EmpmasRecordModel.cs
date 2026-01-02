namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasRecordModel
{
    //-- Single Record --------------------------------------------
    public EmpmasModel? Empmas { get; set; }
    public EmpmasAddressModel? EmpmasAddress { get; set; }
    public EmpmasFamilyModel? EmpmasFamily { get; set; }
    public EmpmasInsuranceModel? EmpmasInsuranceModel { get; set; }
    public EmpmasPIModel? EmpmasPI { get; set; }
    public EmpmasSecLicModel? EmpmasSecLic { get; set; }

    //-- List --------------------------------------------------
    public EmpmasCharRefModel? EmpmasCharRef { get; set; }
    public EmpmasClearancePhModel? EmpmasClearancePh { get; set; }
    public EmpmasEducateModel? EmpmasEducate { get; set; }
    public EmpmasEmploymentModel? EmpmasEmployment { get; set; }
    public EmpmasRelativesModel? EmpmasRelativesModel { get; set; }
    public EmpmasTrainingModel? EmpmasTraining { get; set; }
}
