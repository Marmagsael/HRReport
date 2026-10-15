using HRApiLibrary.Models._11_AMS;

namespace HRMvc.Applications.AMS.Vars;

public class V11005Model
{
    public AttschedweeklyhdrModel               Hdr             { get; set; } = new();
    public List<AttschedweeklyhdrModel>         Hdrs            { get; set; } = [];
    public AttschedweeklyhdrModel?              SelectedHdr     { get; set; } = null;
    public IEnumerable<AttschedweeklyhdrModel>  SelectedHdrs    { get; set; } = [];

    public AttschedweeklydtlModel               Dtl             { get; set; } = new();

    public bool     IsHdrModalVisible       { get; set; } = false;
    public bool     IsDeleteModalVisible    { get; set; } = false;
    public bool     IsEditMode              { get; set; } = false;
    public bool     IsBusy                  { get; set; } = false;
    public string   HdrModalTitle           { get; set; } = "Add Schedule";

}
