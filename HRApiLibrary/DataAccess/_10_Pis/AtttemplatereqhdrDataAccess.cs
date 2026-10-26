using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class AtttemplatereqhdrDataAccess : IAtttemplatereqhdrDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public AtttemplatereqhdrDataAccess(I_90_001_MySqlDataAccess sql)     { _sql = sql; }

    public async Task _01(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Atttemplatereqhdr 
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver,  AttSchedWeeklyHdrId) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver, @AttSchedWeeklyHdrId)";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplatereqhdr, conn);
    }

    public async Task _01Initial(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Atttemplatereqhdr 
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  End,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @End, @Remarks, @Status, @EmpNumber_Approver); 
                        Select * from {schema}.Atttemplatereqhdr where Id = (SELECT @@IDENTITY) ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);

        var idHdr = data?.FirstOrDefault()?.Id ?? 0;
        var empNumber = atttemplatereqhdr.EmpNumber ?? "00000";
        int? userId = atttemplatereqhdr?.UserId ?? 0;

        string? msql = @$"Insert into {schema}.Atttemplate 
                            (EmpmasId, AttendanceTypeId, 
                             D1_In, D1_HrsLength, D1_DutyType, 
                             D2_In, D2_HrsLength, D2_DutyType, 
                             D3_In, D3_HrsLength, D3_DutyType, 
                             D4_In, D4_HrsLength, D4_DutyType, 
                             D5_In, D5_HrsLength, D5_DutyType, 
                             D6_In, D6_HrsLength, D6_DutyType, 
                             D7_In, D7_HrsLength, D7_DutyType) values 
                             (@UserId, 1, 
                              0,   0,   'RD', 
                              800, 900, 'R', 
                              800, 900, 'R', 
                              800, 900, 'R', 
                              800, 900, 'R', 
                              800, 900, 'R', 
                              800, 900, 'RN'); 
                              
                         Insert into {schema}.Atttemplatereqdtl 
                            (AtttemplateReqHdrId, EmpmasId, AttendanceTypeId, 
                             D1_In, D1_HrsLength, D1_DutyType, 
                             D2_In, D2_HrsLength, D2_DutyType, 
                             D3_In, D3_HrsLength, D3_DutyType, 
                             D4_In, D4_HrsLength, D4_DutyType, 
                             D5_In, D5_HrsLength, D5_DutyType, 
                             D6_In, D6_HrsLength, D6_DutyType, 
                             D7_In, D7_HrsLength, D7_DutyType) values 
                             (@AtttemplateReqHdrId, @UserId, 1, 
                              0,   0,   'RD', 
                              800, 900, 'R',  
                              800, 900, 'R', 
                              800, 900, 'R',
                              800, 900, 'R',
                              800, 900, 'R',
                              800, 900, 'RN');";


        await _sql.ExecuteCmd<dynamic>(msql, new { AtttemplateReqHdrId = idHdr, EmpmasId = empNumber, UserId = userId }, conn);
    }
    

    public async Task<AtttemplatereqhdrModel?> _01_02(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Atttemplatereqhdr 
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver,  AttSchedWeeklyHdrId) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver, @AttSchedWeeklyHdrId); 
                        select * from {schema}.Atttemplatereqhdr where Id = (SELECT @@IDENTITY); ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);   
        return data.FirstOrDefault();
    }


    public async Task<List<AtttemplatereqhdrModel?>?> _02s(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  Id, UserId, EmpNumber, DateRequested, Effectivity, Remarks, ApprRemarks, Status, EmpNumber_Approver, AttSchedWeeklyHdrId 
                            from {schema}.Atttemplatereqhdr where Id = @Id";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }
    
    public async Task<List<AtttemplatereqhdrModel?>?> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName, h.*
                        from      {pisdb}.Atttemplatereqhdr h
                        left join {pisdb}.empmas e on e.SystemId = h.UserId    
                        where h.Empnumber_Approver = @EmpNumber_Approver and Status in ('F', 'FA') ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql,
                        new { EmpNumber_Approver = approver_empnumber }, conn);
        return data ?? [];
    }
    
    
    public async Task<List<AtttemplatereqhdrModel?>?> _02ByUserIds(int? userId, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from {pisdb}.Atttemplatereqhdr h 
                        left join {opisdb}.Empmas e on e.empnumber = h.empnumber_Approver 
                        where h.UserId = @UserId 
                        order by h.DateRequested ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }
    
    public async Task<List<AtttemplatereqhdrModel?>?> _02ByUserId_ByEffectivity(int? userId, DateTime effectivity, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from {pisdb}.Atttemplatereqhdr h 
                        left join {pisdb}.Empmas e on e.empnumber = h.empnumber 
                        where h.UserId = @UserId and Date(h.Effectivity) = Date(@Effectivity)
                        order by h.DateRequested ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = userId, Effectivity = effectivity }, conn);
        return data;
    }
    
    // --- approvals ( partial + final ) of a request, oldest first ; only those after the latest Return
    public async Task<List<AtttemplatereqhistModel?>?> _02ApprovalsByHdrId(int? hdrId, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from {pisdb}.Atttemplatereqhist h
                        left join {opisdb}.Empmas e on e.empnumber = h.Empnumber_Approver
                        where h.AtttemplateReqHdrId = @HdrId
                          and (h.SetStatusTo = 'A' or h.Remarks like 'Partially%' or h.Remarks = 'Aproved')
                          and h.DActionTaken >= ifnull((select max(r.DActionTaken) from {pisdb}.Atttemplatereqhist r
                                                        where r.AtttemplateReqHdrId = @HdrId and r.SetStatusTo = 'R'), '1900-01-01')
                        order by h.DActionTaken, h.Id";
        var data = await _sql.FetchData<AtttemplatereqhistModel?, dynamic>(sql, new { HdrId = hdrId }, conn);
        return data ?? [];
    }

    public async Task<List<AtttemplatereqhdrModel?>?> _02ChkMayEntry(int? userId, string? pisdb, string? conn)
    {
        string? sql = $@"select * from {pisdb}.Atttemplatereqhdr h 
                        where h.UserId = @UserId limit 1";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }



    public async Task<AtttemplatereqhdrModel?> _03(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Atttemplatereqhdr set 
							DateRequested 		= @DateRequested, 
							Effectivity 		= @Effectivity, 
							Remarks 			= @Remarks, 
							Status 				= @Status, 
							EmpNumber_Approver 	= @EmpNumber_Approver, 
							AttSchedWeeklyHdrId 	= @AttSchedWeeklyHdrId where Id = @Id;
						select  * from {schema}.Atttemplatereqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);
        return data?.FirstOrDefault();
    }

    public async Task _03Approve(AtttemplatereqhdrModel atrh, string? empNumber, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Atttemplatereqhdr set Status  = 'A' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = atrh.Id }, conn);

        AtttemplatereqhistModel h = new()
        {
            AtttemplateReqHdrId     = atrh.Id,
            DActionTaken            = DateTime.Now,
            SetStatusTo             = "A",
            Empnumber_Approver      = empNumber,
            Remarks                 = "Aproved"
        };

        sql = $@"Insert into {schema}.atttemplatereqhist 
                    (AtttemplateReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AtttemplateReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }
    // --- runs at login : approved request whose Effectivity has arrived is applied to Atttemplate,
    //     then hdr / dtl are moved to AtttemplateReqHdr_Applied / AtttemplateReqdtl_Applied
    //     userId   : Users.Id   ( AtttemplateReqHdr.UserId )
    //     empmasId : Empmas.Id  ( Atttemplate.EmpmasId )
    public async Task _03AttTemplateReqhdr_to_AttTemplate(int? userId, int? empmasId, string? pisdb, string? conn)
    {
        if ((userId ?? 0) <= 0 || (empmasId ?? 0) <= 0) return;

        string? sql = $@"select * from {pisdb}.Atttemplatereqhdr h 
                        where h.UserId = @UserId and h.Status = 'A' and Date(h.Effectivity) <= Date(now())
                        order by h.Effectivity desc, h.Id desc";
        var hdrs = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = userId }, conn);
        if (hdrs == null || hdrs.Count == 0) return;

        // --- 1) apply the latest one still within its End date
        var latest = hdrs.FirstOrDefault(h => (h?.End ?? DateTime.MaxValue).Date >= DateTime.Now.Date);
        if (latest != null)
        {
            sql = $@"select * from {pisdb}.Atttemplatereqdtl d 
                     where d.AtttemplateReqHdrId = @AtttemplateReqHdrId 
                     order by d.Id desc limit 1";
            var dtl = (await _sql.FetchData<AtttemplatereqdtlModel, dynamic>(sql, new { AtttemplateReqHdrId = latest.Id }, conn))?.FirstOrDefault();

            if (dtl != null)
            {
                sql = $@"INSERT IGNORE INTO {pisdb}.Atttemplate (EmpmasId) values (@EmpmasId);

                         UPDATE {pisdb}.Atttemplate t
                         INNER JOIN {pisdb}.Atttemplatereqdtl d on d.Id = @DtlId
                         INNER JOIN {pisdb}.Atttemplatereqhdr h on h.Id = d.AtttemplateReqHdrId
                         set t.AttendanceTypeId     = ifnull(d.AttendanceTypeId, 1),
                             t.D1_In = d.D1_In, t.D1_HrsLength = d.D1_HrsLength, t.D1_DutyType = d.D1_DutyType,
                             t.D2_In = d.D2_In, t.D2_HrsLength = d.D2_HrsLength, t.D2_DutyType = d.D2_DutyType,
                             t.D3_In = d.D3_In, t.D3_HrsLength = d.D3_HrsLength, t.D3_DutyType = d.D3_DutyType,
                             t.D4_In = d.D4_In, t.D4_HrsLength = d.D4_HrsLength, t.D4_DutyType = d.D4_DutyType,
                             t.D5_In = d.D5_In, t.D5_HrsLength = d.D5_HrsLength, t.D5_DutyType = d.D5_DutyType,
                             t.D6_In = d.D6_In, t.D6_HrsLength = d.D6_HrsLength, t.D6_DutyType = d.D6_DutyType,
                             t.D7_In = d.D7_In, t.D7_HrsLength = d.D7_HrsLength, t.D7_DutyType = d.D7_DutyType,
                             t.AttschedweeklyhdrId  = ifnull(h.AttSchedWeeklyHdrId, 0)
                         where t.EmpmasId = @EmpmasId;";
                await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = empmasId, DtlId = dtl.Id }, conn);
            }
        }

        // --- 2) move all approved + effective requests to the _Applied tables ( save first, then delete )
        var ids = hdrs.Where(h => h?.Id != null).Select(h => h!.Id).ToList();
        sql = $@"INSERT INTO {pisdb}.AtttemplateReqHdr_Applied select * from {pisdb}.AtttemplateReqHdr where Id in @Ids;
                 INSERT INTO {pisdb}.AtttemplateReqdtl_Applied select * from {pisdb}.AtttemplateReqdtl where AtttemplateReqHdrId in @Ids;
                 DELETE from {pisdb}.AtttemplateReqdtl where AtttemplateReqHdrId in @Ids;
                 DELETE from {pisdb}.AtttemplateReqHdr where Id in @Ids;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Ids = ids }, conn);
    }


    public async Task _03PartiallyApprove(AtttemplatereqhdrModel atrh, string? empNumber_Approver, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Atttemplatereqhdr set EmpNumber_Approver  = @EmpNumber_Approver where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = atrh.Id, EmpNumber_Approver = empNumber_Approver }, conn);

        AtttemplatereqhistModel h = new()
        {
            AtttemplateReqHdrId = atrh.Id,
            DActionTaken = DateTime.Now,
            SetStatusTo = "F",
            Empnumber_Approver = atrh.EmpNumber_Approver??"",
            Remarks = $"Partially Aprove [{empNumber_Approver ?? ""}] "
        };
        

        sql = $@"Insert into {schema}.atttemplatereqhist 
                    (AtttemplateReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AtttemplateReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }

    public async Task _03Return(AtttemplatereqhdrModel treqhdr, string? empNumber, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Atttemplatereqhdr set ApprRemarks = @AppRemarks, Status = 'R' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = treqhdr.Id, AppRemarks = treqhdr.ApprRemarks }, conn);

        AtttemplatereqhistModel h = new()
        {
            AtttemplateReqHdrId = treqhdr.Id,
            DActionTaken        = DateTime.Now,
            SetStatusTo         = "R",
            Empnumber_Approver  = empNumber,
            Remarks             = "Return Request"
        };

        sql = $@"Insert into {schema}.atttemplatereqhist 
                    (AtttemplateReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AtttemplateReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }

    public async Task<AtttemplatereqhdrModel?> _03SendForApproval(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Atttemplatereqhdr set 
							DateRequested 		= @DateRequested, 
							Effectivity 		= @Effectivity, 
							Remarks 			= @Remarks, 
							Status 				= 'F', 
							EmpNumber_Approver 	= @EmpNumber_Approver, 
							AttSchedWeeklyHdrId 	= @AttSchedWeeklyHdrId where Id = @Id;
						select  * from {schema}.Atttemplatereqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);

        // *************************************************************************************************
        var h = atttemplatereqhdr; 
        AtttemplatereqhistModel hist = new()
        {
            AtttemplateReqHdrId = h.Id, 
            DActionTaken        = DateTime.Now, 
            Empnumber_Approver  = h.EmpNumber_Approver, 
            Remarks             = h.Remarks??"For Approval", 
            SetStatusTo         = "F" 
        }; 




        return data?.FirstOrDefault();
    }


    public async Task _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Atttemplatereqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }
}

public interface IAtttemplatereqhdrDataAccess
{
    Task                                    _01(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task<AtttemplatereqhdrModel?>           _01_02(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task                                    _01Initial(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02s(int? id, string? schema, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ByUserIds(int? userId, string? pisdb, string? opisdb, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn);
    Task<List<AtttemplatereqhistModel?>?>   _02ApprovalsByHdrId(int? hdrId, string? pisdb, string? opisdb, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ChkMayEntry(int? userId, string? pisdb, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ByUserId_ByEffectivity(int? userId, DateTime effectivity, string? pisdb, string? conn); 
    Task<AtttemplatereqhdrModel?>           _03(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task                                    _03Approve(AtttemplatereqhdrModel atrh, string? empNumber, string? schema, string? conn);
    Task                                    _03PartiallyApprove(AtttemplatereqhdrModel atrh, string? empNumber, string? schema, string? conn);
    Task<AtttemplatereqhdrModel?>           _03SendForApproval(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task                                    _03Return(AtttemplatereqhdrModel treqhdr, string? empNumber, string? schema, string? conn);
    Task                                    _03AttTemplateReqhdr_to_AttTemplate(int? userId, int? empmasId, string? pisdb, string? conn);
    Task                                    _04(int? id, string? schema, string? conn);
}
