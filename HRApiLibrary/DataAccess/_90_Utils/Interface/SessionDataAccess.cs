using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._90_Utils.Interface;

public class SessionDataAccess : ControllerBase, ISessionDataAccess
{
    private readonly I_00UserscompanyDataAccess _usercompany;

    public SessionDataAccess(I_00UserscompanyDataAccess usercompany)
    {
        _usercompany = usercompany;
    }

    public async Task<SessionInfo> _01Session_Schema_ByUsercompanyId(int id)
    {
        SessionInfo sessionInfo    = new();

        var res = await _usercompany._02ByUserDefaultCoId(id); 
        //if (res != null)
        //{
            //HttpContext.Session.SetString(SessionField.AMSSchema,       res.AmsSchema);
            //HttpContext.Session.SetString(SessionField.ApplicantSchema, res.ApplicantSchema);
            //HttpContext.Session.SetString(SessionField.PISSchema,       res.PisSchema);
            //HttpContext.Session.SetString(SessionField.PaySchema,       "res.PaySchema");
        //}

        return sessionInfo;
    }

    public SessionInfo _02Schema()
    {
        SessionInfo? s      = new(); 
        s.PaySchema         = HttpContext.Session.GetString(SessionField.PaySchema);
        return s;
    }
}
