using HRApiLibrary.DataAccess._10_Pis.OPis;

namespace HRMvc.StartupConfig;

public class AmsAccessService : PisAccessService
{
    public AmsAccessService(IOPisDomainaccessDataAccess access, IOEmpmasDataAccess oldEmpmas) : base(access, oldEmpmas) { }

    protected override string Module => "ams";
}
