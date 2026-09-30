using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2303Model
    {
        public string?      OPisdb                          { get; set; } = string.Empty;
        public string?      Maindb                          { get; set; } = string.Empty;
        public string?      Mainpisdb                       { get; set; } = string.Empty;
        public string?      Conn                            { get; set; } = string.Empty;




        public string?      Action                          { get; set; } = string.Empty;
        public bool         IsLoading                       { get; set; } = true;
        public bool?        UcLoaded                        { get; set; } = false;

        public string?      SearchEmployeeModalCaption      { get; set; } = string.Empty;
        public bool         ShowSearchEmployeeModal         { get; set; } = false;




        public OEmpmasModel? OEmpmas                        { get; set; } = new();
        public List<OEmpmasModel?>? OEmployees              { get; set; } = new();
        public List<OEmpstatModel?>? OEmpStatuses           { get; set; } = new();
        public List<OOfstaffModel?>? Preparers              { get; set; } = new();
        public List<OOfstaff2Model?>? Approvers             { get; set; } = new();

        public OOfstaffModel?  Preparer                     { get; set; } = new();
        public OOfstaff2Model? Approver                     { get; set; } = new();

        public string NewStatus                             { get; set; } =  string.Empty;

    }
}
