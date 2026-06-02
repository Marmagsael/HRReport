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
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver)";
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
							(UserId,  EmpNumber,  DateRequested,  Effectivity,  Remarks,  Status,  EmpNumber_Approver) values 
							(@UserId, @EmpNumber, @DateRequested, @Effectivity, @Remarks, @Status, @EmpNumber_Approver); 
                        select * from {schema}.Atttemplatereqhdr where Id = (SELECT @@IDENTITY); ";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, atttemplatereqhdr, conn);   
        return data.FirstOrDefault();
    }


    public async Task<List<AtttemplatereqhdrModel?>?> _02s(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  Id, UserId, EmpNumber, DateRequested, Effectivity, Remarks, Status, EmpNumber_Approver 
                            from {schema}.Atttemplatereqhdr where Id = @Id";
        var data = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }
    
    public async Task<List<AtttemplatereqhdrModel?>?> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName, h.*
                        from      {pisdb}.Atttemplatereqhdr h
                        left join {pisdb}.empmas e on e.Id = h.UserId    
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
							EmpNumber_Approver 	= @EmpNumber_Approver where Id = @Id;
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
            SetStatusTo             = "F",
            Empnumber_Approver      = empNumber,
            Remarks                 = "Aproved"
        };

        sql = $@"Insert into {schema}.atttemplatereqhist 
                    (AtttemplateReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AtttemplateReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }
    public async Task _03AttTemplateReqhdr_to_AttTemplate(int? EmpmasId, string? pisdb, string? conn)
    {
        
        string? sql = $@"select * from {pisdb}.Atttemplatereqhdr h 
                        where h.UserId = @UserId and Status = 'A' and (Date(now()) >= Date(effectivity) and Date(now()) <= Date(h.End))
                        order by h.Effectivity desc limit 1";
        var res = await _sql.FetchData<AtttemplatereqhdrModel?, dynamic>(sql, new { UserId = EmpmasId }, conn);
        var r1 = res?.FirstOrDefault(); 
        // Console.WriteLine(@$" UserId : {EmpmasId} * res count : {res?.Count}");

        if (res == null || res.Count == 0) return;

        sql = $@"select * from {pisdb}.Atttemplatereqdtl d 
                 where d.AtttemplateReqHdrId = @AtttemplateReqHdrId 
                 order by d.Id desc limit 1";
        var res2 = await _sql.FetchData<AtttemplatereqdtlModel, dynamic>(sql, new { AtttemplateReqHdrId = res.First().Id }, conn);
        if (res2 == null || res2.Count == 0) return;

        var r = res2.First();
        AtttemplateModel at = new()
        {
            EmpmasId = EmpmasId,
            AttendancetypeId = r?.AttendanceTypeId??1,
            D1_in = r.D1_In??800, D1_hrslength = r.D1_HrsLength??900, D1_dutytype = r.D1_DutyType??"R",
            D2_in = r.D2_In??800, D2_hrslength = r.D2_HrsLength??900, D2_dutytype = r.D2_DutyType??"R",
            D3_in = r.D3_In??800, D3_hrslength = r.D3_HrsLength??900, D3_dutytype = r.D3_DutyType??"R",
            D4_in = r.D4_In??800, D4_hrslength = r.D4_HrsLength??900, D4_dutytype = r.D4_DutyType??"R",
            D5_in = r.D5_In??800, D5_hrslength = r.D5_HrsLength??900, D5_dutytype = r.D5_DutyType??"R",
            D6_in = r.D6_In??800, D6_hrslength = r.D6_HrsLength??900, D6_dutytype = r.D6_DutyType??"R",
            D7_in = r.D7_In??800, D7_hrslength = r.D7_HrsLength??900, D7_dutytype = r.D7_DutyType??"R"
        }; 
        
        sql = $@"UPDATE {pisdb}.Atttemplate set
                    AttendancetypeId = @AttendancetypeId, 
                    D1_in = @D1_in, D1_hrslength = @D1_hrslength, D1_dutytype = @D1_dutytype, 
                    D2_in = @D2_in, D2_hrslength = @D2_hrslength, D2_dutytype = @D2_dutytype, 
                    D3_in = @D3_in, D3_hrslength = @D3_hrslength, D3_dutytype = @D3_dutytype, 
                    D4_in = @D4_in, D4_hrslength = @D4_hrslength, D4_dutytype = @D4_dutytype, 
                    D5_in = @D5_in, D5_hrslength = @D5_hrslength, D5_dutytype = @D5_dutytype, 
                    D6_in = @D6_in, D6_hrslength = @D6_hrslength, D6_dutytype = @D6_dutytype, 
                    D7_in = @D7_in, D7_hrslength = @D7_hrslength, D7_dutytype = @D7_dutytype
                    WHERE EmpmasId = @EmpmasId; ";
        await _sql.ExecuteCmd<dynamic>(sql, at, conn);

        int? id = EmpmasId;
        sql = $@"select * from {pisdb}.Atttemplate d 
                 where d.EmpmasId = @EmpmasId  limit 1";
        var res3 = await _sql.FetchData<AtttemplatereqdtlModel, dynamic>(sql, new { EmpmasId = id }, conn);
        var r3 = res3.FirstOrDefault();

        // Console.WriteLine($@" Empoyee Id : {EmpmasId} * Effectivity : {r1.Effectivity?.ToString("MMM dd, yyyy")} - {r1.End?.ToString("MMM dd, yyyy")}
        //     Hdr Id : {r1.Id} * Dtl Id : {r.Id}

        //     dutytype (AttTemplate 1): {r3?.D1_DutyType} * (Detail 1) : {r.D1_DutyType}
        //     dutytype (AttTemplate 2): {r3?.D2_DutyType} * (Detail 2) : {r.D2_DutyType}
        //     dutytype (AttTemplate 3): {r3?.D3_DutyType} * (Detail 3) : {r.D3_DutyType}
        //     dutytype (AttTemplate 4): {r3?.D4_DutyType} * (Detail 4) : {r.D4_DutyType}
        //     dutytype (AttTemplate 5): {r3?.D5_DutyType} * (Detail 5) : {r.D5_DutyType}
        //     dutytype (AttTemplate 6): {r3?.D6_DutyType} * (Detail 6) : {r.D6_DutyType}
        //     dutytype (AttTemplate 7): {r3?.D7_DutyType} * (Detail 7) : {r.D7_DutyType}
        // ");

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
							EmpNumber_Approver 	= @EmpNumber_Approver where Id = @Id;
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
    Task<List<AtttemplatereqhdrModel?>?>    _02ChkMayEntry(int? userId, string? pisdb, string? conn);
    Task<List<AtttemplatereqhdrModel?>?>    _02ByUserId_ByEffectivity(int? userId, DateTime effectivity, string? pisdb, string? conn); 
    Task<AtttemplatereqhdrModel?>           _03(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task                                    _03Approve(AtttemplatereqhdrModel atrh, string? empNumber, string? schema, string? conn);
    Task                                    _03PartiallyApprove(AtttemplatereqhdrModel atrh, string? empNumber, string? schema, string? conn);
    Task<AtttemplatereqhdrModel?>           _03SendForApproval(AtttemplatereqhdrModel atttemplatereqhdr, string? schema, string? conn);
    Task                                    _03Return(AtttemplatereqhdrModel treqhdr, string? empNumber, string? schema, string? conn);
    Task                                    _03AttTemplateReqhdr_to_AttTemplate(int? EmpmasId, string? pisdb, string? conn);
    Task                                    _04(int? id, string? schema, string? conn);
}
