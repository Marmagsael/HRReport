using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using Microsoft.OpenApi.Models;

namespace HRMvc.Applications._02HR._02Library; 

public class DA222
{
    private readonly I_10_EmpmasDataAccess _empmas;

    public DA222(I_10_EmpmasDataAccess empmas)
    {
        _empmas = empmas;
    }

    //public async Task<DeprecModel> _02Deprecs(int id, string db, string conn) 
    //{
    //    DeprecModel dr = new() { Empmasid = id};

    //    var res = await _empmas._02Deprecs(id, db, conn);
    //    if(res==null)   {   await _empmas._01Deprec(dr, db, conn); } 
    //    else
    //    {
    //        if(res.Count < 1)   {   await _empmas._01Deprec(dr, db, conn); } 
    //        else
    //        {   dr = res.FirstOrDefault()!;  }
    //    }

    //    return dr; 
    //}
    
    public async Task<EmpmasPIModel> _02EmpmasPI(int id, string db, string conn) 
    {
        EmpmasPIModel empmasPI = new() { Id = id};

        var res = await _empmas._02EmpmasPIs(id, db, conn);
        if(empmasPI == null)   { await _empmas._01EmpmasPI(id, empmasPI!, db, conn);  } 
        else
        {
            if(res?.Count < 1)   { await _empmas._01EmpmasPI(id, empmasPI!, db, conn); } 
            else
            {   empmasPI = res?.FirstOrDefault()!;  }
        }
        return empmasPI!; 
    }

    public async Task<DeprecModel> _02Deprec(int id, string db, string conn) 
    {
        DeprecModel dp  = new() { Empmasid = id , Empstatusid = 12 }; // 12 = For Deployment
        var deprecs     = await _empmas._02Deprecs(id, db, conn);
        if(deprecs==null) { await _empmas._01Deprec(dp, db, conn); }
        else
        {
            if (deprecs?.Count < 1) { await _empmas._01Deprec(dp, db, conn); }
            else                    { dp = deprecs?.FirstOrDefault()!; }
        }

        return dp; 
    }

    public async Task<EmpmasAddressModel> _02EmpmasAddress(int id, string db, string conn)
    {
        EmpmasAddressModel ea = new() { Id=id };
        var res = await _empmas._02EmpmasAddresss(id, db, conn); 
        if(res==null)  { await _empmas._01EmpmasAddress(id, ea, db, conn); }
        else
        {
            if (res.Count < 1)  { await _empmas._01EmpmasAddress(id, ea, db, conn); }
            else                { ea = res?.FirstOrDefault()!; }
        }

        return ea; 
    }

    


}
