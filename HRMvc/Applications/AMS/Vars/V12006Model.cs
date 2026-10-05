using HRApiLibrary.Modules._12006O;

namespace HRMvc.Applications.AMS.Vars;

public class V12006Model
{
    // ─── Sites List ───────────────────────────────────────────
    public List<M12006_Site>    Sites           { get; set; } = [];
    public M12006_Site          SelectedSite    { get; set; } = new();

    // ─── Form ─────────────────────────────────────────────────
    public M12006_Site          Form            { get; set; } = new();
    public bool                 IsFormVisible   { get; set; } = false;
    public bool                 IsEditMode      { get; set; } = false;

    // ─── Delete ───────────────────────────────────────────────
    public bool                 IsDeleteVisible { get; set; } = false;

    // ─── Filter ───────────────────────────────────────────────
    public string               StatusFilter    { get; set; } = "A";

    // ─── Notification ─────────────────────────────────────────
    public (string Type, string Message) Notification { get; set; }

    // ─── Busy ─────────────────────────────────────────────────
    public bool IsBusy { get; set; } = false;
}
