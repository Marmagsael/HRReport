using Blazorise;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;

public class AttreqhdrDataAccess : IAttreqhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqhdrModel?> _01(AttreqhdrModel attreqhdr, string? schema, string? conn )
	{
		string sql = $@"Insert into {schema}.Attreqhdr 
                            (UserId,  EmpNumber,  DateRequested,  CovStart,  CovEnd,  AttReqTypeId,  Remarks,  Status,  EmpNumber_Approver,  TotHrs) values 
                            (@UserId, @EmpNumber, @DateRequested, @CovStart, @CovEnd, @AttReqTypeId, @Remarks, @Status, @EmpNumber_Approver, @TotHrs)" ; 
		await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);
		sql = $@"SELECT * FROM {schema}.Attreqhdr WHERE ID = (SELECT @@IDENTITY)"; 
		var res = await _sql.FetchData<AttreqhdrModel?,dynamic>(sql,new { },conn);

		return res.FirstOrDefault();
	}


    public async Task<List<AttreqhdrModel?>> _02s(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  Id, UserId, EmpNumber, DateRequested, CovStart, CovEnd, 
                                AttReqTypeId, Remarks, Status, EmpNumber_Approver, TotHrs 
                        from {schema}.Attreqhdr where Id = @Id";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic> (sql, new { Id = id }, conn);
        return data ?? [];
    }
    
    public async Task<List<AttreqhdrModel?>> _02ByUserId_ByTypeId_ByStatus
    (int? userid, int? typeId, List<string> status, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*
                        from      {pisdb}.Attreqhdr h
                        left join {opisdb}.empmas e on e.Empnumber = h.EmpNumber_Approver    
                        where h.UserId = @UserId and h.AttReqTypeId = @TypeId and h.Status in @Status ";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, 
                        new { UserId=userid, TypeId=typeId, Status=status }, conn);
        return data ?? [];
    }
    
    public async Task<List<AttreqhdrModel?>> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS RequestorName, h.*
                        from      {pisdb}.Attreqhdr h
                        left join {pisdb}.empmas e on e.Id = h.UserId    
                        where h.Empnumber_Approver = @EmpNumber_Approver and Status in ('F', 'FA') ";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, 
                        new { EmpNumber_Approver =approver_empnumber }, conn);
        return data ?? [];
    }

    public async Task<AttreqhdrModel?> _03(int? id,AttreqhdrModel attreqhdr, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set 
                            DateRequested       = @DateRequested, 
                            CovStart            = @CovStart, 
                            CovEnd              = @CovEnd, 
                            Remarks             = @Remarks, 
                            EmpNumber_Approver  = @EmpNumber_Approver
                        where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, attreqhdr, conn);
		
		sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
		var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

    public async Task _03Approve(AttreqhdrModel atrh, string? empNumber, int approverId, string? schema, string? conn)
    {
        int id = atrh.Id??0;
        string? sql = ""; 

        // --- Request Details -------------------------------------------------------------------
        int attReqHdrId = atrh.Id??0; 
        sql = $@"SELECT * FROM {schema}.`attreqdtl` where AttReqHdrId =  @AttReqHdrId";
        var resDtls = await _sql.FetchData<AttreqdtlModel?, dynamic>(sql, new { AttReqHdrId = attReqHdrId } , conn);

        // --- Punches ( Attpunches1 ) ------------------------------------------------------------
        DateTime dstart = atrh.CovStart.Date;
        DateTime dend   = atrh.CovEnd.Date.AddDays(1);
        int userId      = atrh.UserId??0; 
        
        sql = $@" SELECT *  FROM {schema}.attpunches1 WHERE PunchInDate >= @DStart AND PunchInDate < @DEnd AND EmpmasId = @UserId ";
        var AP1 = await _sql.FetchData<Attpunches1Model?, dynamic>(sql, new { DStart = dstart, DEnd = dend, UserId = userId}, conn );

        foreach(var dtl in resDtls??[])
        {
            await InsertPunch(dtl??new(), userId, approverId, schema??"", conn??""); 
        }


        sql = $@"Update {schema}.Attreqhdr set Status  = 'A' where Id = @Id;";
        // await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        AttreqhistModel h = new()
        {
            AttReqHdrId         = id,
            DActionTaken        = DateTime.Now,
            SetStatusTo         = "F",
            Empnumber_Approver  = empNumber,
            Remarks             = "Aproved"
        };






        sql = $@"Insert into {schema}.atttemplatereqhist 
                    (AtttemplateReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AtttemplateReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        // await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }

        private async Task InsertPunch(AttreqdtlModel dtl, int userId, int approverId, string schema,  string conn )
        {
            var date = dtl.DStart?.Date ?? dtl.DEnd;
            if (date == null) return;

            // --- Attendance Template ---------------------------------------------------------
            string sql = $@"select * from  {schema}.atttemplatereqdtl 
                            where AttTemplateReqhdrId in 
                                ( select Id from {schema}.atttemplatereqhdr 
                                    where status = 'A' and 
                                        @Date between Effectivity and End and 
                                        UserId = @UserId ) ";
            var ts = await _sql.FetchData<AtttemplatereqdtlModel, dynamic>(sql, new { Date = date, UserId = userId }, conn);

            var t = new AtttemplatereqdtlModel();
            if (ts != null || ts.Count > 0) t = ts.First();

            var id = TimeZoneInfo.Local.Id;

            Attpunches1Model ap = new()
            {
                EmpmasId        = userId, 
                DayNo           = (int)(date?.DayOfWeek ?? DayOfWeek.Sunday),

                PunchInDate = dtl.DStart, 
                TimeZoneIdIn    = MsdsDataAccess.Get_TimeZone_Id(id), 
                IpAddressIn     = MsdsDataAccess.GetIPAddress(), 
                MacAddressIn    = MsdsDataAccess.GetMacAddress(), 
                UserIdIn        = approverId, 

                PunchOutDate    = dtl.DEnd, 
                TimeZoneIdOut   = MsdsDataAccess.Get_TimeZone_Id(id),
                IpAddressOut    = MsdsDataAccess.GetIPAddress(),
                MacAddressOut   = MsdsDataAccess.GetMacAddress(),
                

                Status = "L"
            };


        (ap.PunchT, ap.SchedDuration, ap.DutyTypeCode) = ap.DayNo switch
        {
            1 => (t.D7_In ?? 0, t.D7_HrsLength ?? 0, t.D7_DutyType,"RN"),
            2 => (t.D1_In ?? 0, t.D1_HrsLength ?? 0, t.D1_DutyType,"RN"),
            3 => (t.D2_In ?? 0, t.D2_HrsLength ?? 0, t.D2_DutyType,"RN"),
            4 => (t.D3_In ?? 0, t.D3_HrsLength ?? 0, t.D3_DutyType,"RN"),
            5 => (t.D4_In ?? 0, t.D4_HrsLength ?? 0, t.D4_DutyType,"RN"),
            6 => (t.D5_In ?? 0, t.D5_HrsLength ?? 0, t.D5_DutyType,"RN"),
            7 => (t.D6_In ?? 0, t.D6_HrsLength ?? 0, t.D6_DutyType,"RN"),
            _ => (0, 0, "RN")
        };

        sql=$@"insert into {schema}.attpunches1 
                (EmpmasId, DayNo, 
                 PunchInDate, PunchT, SchedDuration, DutyTypeId, TimeZoneIdIn, IpAddressIn, MacAddressIn, UserIdIn, 
                 PunchOutDate, TimeZoneIdOut, IpAddressOut, MacAddressOut, UserIdOut, Status) values 
                 (@EmpmasId, @DayNo, 
                 @PunchInDate, @PunchT, @SchedDuration, @DutyTypeId, @TimeZoneIdIn, @IpAddressIn, @MacAddressIn, @UserIdIn, 
                 @PunchOutDate, @TimeZoneIdOut, @IpAddressOut, @MacAddressOut, @UserIdOut, @Status) 
                 on duplicate key update  
                    PunchT          = @PunchT, 
                    SchedDuration   = @SchedDuration, 
                    DutyTypeId      = @DutyTypeId, 
                    TimeZoneIdIn    = @TimeZoneIdIn, 
                    IpAddressIn     = @IpAddressIn, 
                    MacAddressIn    = @MacAddressIn, 
                    UserIdIn        = @UserIdIn, 
                    PunchOutDate    = @PunchOutDate, 
                    TimeZoneIdOut   = @TimeZoneIdOut, 
                    IpAddressOut    = @IpAddressOut, 
                    MacAddressOut   = @MacAddressOut, 
                    UserIdOut       = @UserIdOut, 
                    Status          = @Status";
        if(dtl.AttReqTypeId==3)  // punch-in and punch-out 
        {   await _sql.ExecuteCmd<dynamic>(sql, ap, conn);  }
        
        if(dtl.AttReqTypeId==2) // punch-out only  
        {
           ap.PunchInDate = ap.PunchInDate?.Date;      

            sql = @$"insert into {schema}.attpunches1 
                        (EmpmasId, DayNo,
                            PunchInDate, PunchT, SchedDuration, DutyTypeId, TimeZoneIdIn, IpAddressIn, MacAddressIn, UserIdIn,
                            PunchOutDate, TimeZoneIdOut, IpAddressOut, MacAddressOut, UserIdOut, Status) values
                        (@EmpmasId, @DayNo,
                            @PunchInDate, @PunchT, @SchedDuration, @DutyTypeId, @TimeZoneIdIn, @IpAddressIn, @MacAddressIn, @UserIdIn,
                            @PunchOutDate, @TimeZoneIdOut, @IpAddressOut, @MacAddressOut, @UserIdOut, @Status)
                         on duplicate key update
                            PunchT = @PunchT, 
                            SchedDuration = @SchedDuration, 
                            DutyTypeId = @DutyTypeId, 
                            TimeZoneIdIn = @TimeZoneIdIn, 
                            IpAddressIn = @IpAddressIn, 
                            MacAddressIn = @MacAddressIn, 
                            UserIdIn = @UserIdIn, 
                            PunchOutDate = @PunchOutDate, 
                            TimeZoneIdOut = @TimeZoneIdOut, 
                            IpAddressOut = @IpAddressOut, 
                            MacAddressOut = @MacAddressOut, 
                            UserIdOut = @UserIdOut, 
                            Status = @Status";
        }
    

        Console.WriteLine($" AttReqTypeId : {dtl.AttReqTypeId} * DStart : {dtl.DStart} * DEnd : {dtl.DEnd}");
        }

        void GetPuncinTemplate(int empmasId, DateTime dte)
        {

        } 




    public async Task _03Return(AttreqhdrModel arh, string? empNumber, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set ApprRemarks = @ApprRemarks, Status = 'R' where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, new { Id = arh.Id, AppRemarks=arh.ApprRemarks }, conn);

        AttreqhistModel h = new()
        { AttReqHdrId = arh.Id,  DActionTaken = DateTime.Now,  SetStatusTo="R",  Empnumber_Approver = empNumber,  Remarks = "Return Request" }; 
        
        sql = $@"Insert into {schema}.attreqhist 
                    (AttReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }
    
    public async Task _03PartiallyApprove(AttreqhdrModel arh, string? empNumber, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Attreqhdr set EmpNumber_Approver  = @EmpNumber_Approver where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, new { Id = arh.Id, EmpNumber_Approver = empNumber }, conn);

        AttreqhistModel h = new()
        { 
            AttReqHdrId = arh.Id, 
            DActionTaken = DateTime.Now, 
            SetStatusTo = "F", 
            Empnumber_Approver = arh.EmpNumber_Approver, 
            Remarks = $"Partially Aprove [{empNumber??""}]" 
        };

        sql = $@"Insert into {schema}.attreqhist 
                    (AttReqHdrId,  DActionTaken,  SetStatusTo,  Empnumber_Approver,  Remarks) values 
                    (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Empnumber_Approver, @Remarks);";
        await _sql.ExecuteCmd<dynamic>(sql, h, conn);
    }

    
    public async Task<AttreqhdrModel?> _03SendForApproval(AttreqhdrModel attreqhdr, string? schema, string? conn)
	{
        string? sql = $@"Update {schema}.Attreqhdr set 
                            DateRequested       = @DateRequested, 
                            CovStart            = @CovStart, 
                            CovEnd              = @CovEnd, 
                            Remarks             = @Remarks, 
                            EmpNumber_Approver  = @EmpNumber_Approver, 
                            Status              = 'F' 
                        where Id = @Id;
                        select  * from {schema}.Attreqhdr x where x.Id = @Id ;"; 
		var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, attreqhdr, conn);

        // *****************************************************************************************************
        AttreqhistModel attreqhist = new() { AttReqHdrId = attreqhdr.Id, DActionTaken = DateTime.Now, Empnumber_Approver = attreqhdr.EmpNumber_Approver, 
                                             Remarks = attreqhdr.Remarks??"For Approval", SetStatusTo = "F" }; 

        var sql1 =  $@"Insert into {schema}.Attreqhist 
                            (AttReqHdrId,  DActionTaken,  SetStatusTo,  Remarks,             Empnumber_Approver) values 
                            (@AttReqHdrId, @DActionTaken, @SetStatusTo, 'Send For Approval', @Empnumber_Approver)" ; 
		await _sql.ExecuteCmd<dynamic>(sql1, attreqhist, conn);
        // *****************************************************************************************************


		return data?.FirstOrDefault();
        
	}
    
    public async Task<AttreqhdrModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attreqhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttreqhdrDataAccess
{
    Task<AttreqhdrModel?>       _01(AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task<List<AttreqhdrModel?>> _02s(int? id, string? schema, string? conn);
    Task<List<AttreqhdrModel?>> _02ByUserId_ByTypeId_ByStatus(int? userid, int? typeId, List<string> status, string? pisdb, string? opisdb, string? conn);
    Task<List<AttreqhdrModel?>> _02ForApproval_PerApprover(string? approver_empnumber, string? pisdb, string? conn); 
    Task<AttreqhdrModel?>       _03(int? id, AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task<AttreqhdrModel?>       _03SendForApproval(AttreqhdrModel attreqhdr, string? schema, string? conn);
    Task                        _03Approve(AttreqhdrModel atrh, string? empNumber, int approverId, string? schema, string? conn); 
    Task                        _03Return(AttreqhdrModel arh, string? empNumber, string? schema, string? conn);
    Task                        _03PartiallyApprove(AttreqhdrModel arh, string? empNumber, string? schema, string? conn); 
    Task<AttreqhdrModel?>       _04(int? id, string? schema, string? conn);
}
