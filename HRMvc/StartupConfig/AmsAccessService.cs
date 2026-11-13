using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._90_Utils;

namespace HRMvc.StartupConfig;

public class AmsAccessService : PisAccessService
{
    public AmsAccessService(IOPisDomainaccessDataAccess access, IOEmpmasDataAccess oldEmpmas, IMainmenuDataAccess menu) : base(access, oldEmpmas, menu) { }

    protected override string Module => "ams";
}
