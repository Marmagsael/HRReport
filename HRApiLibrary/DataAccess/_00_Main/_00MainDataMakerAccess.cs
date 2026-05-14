using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._90_Utils;
using Microsoft.Extensions.Configuration;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00MainDataMakerAccess : I_00MainDataMakerAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IConfiguration _config;

    public _00MainDataMakerAccess(I_90_001_MySqlDataAccess sql, IConfiguration config)
    {
        _sql = sql;
        _config = config;
    }

    public async Task _01MainDefaultDatas(string schema = "Main", string connName = "MySqlConn")
    {
        await _01Menus10User(schema, connName);
        await _01Users_DefaulDatas(schema, connName);
        await _01PayrollMenus(schema, connName);
        //_01Country_DefaultDatas(schema, connName);
        //_01Currency_DefaultDatas(schema, connName);
    }

    public async void _01UsersCompany_DefaultDatas(int userId, string companyCode, string companyName, int currencyId, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $"select * from {schema}.UsersCompany where OwnerId = {userId} and companySName = '{companyCode}'";
        var res = await _sql.FetchData<UserCompanyModel, dynamic>(sql, new { });

        if (res.Count == 0)
        {
            string AmsSchema        = _config.GetSection("Schema:ams").Value;
            string ApplicantSchema  = _config.GetSection("Schema:Applicant").Value;
            string PisSchema        = _config.GetSection("Schema:Pis").Value;
            string PaySchema        = _config.GetSection("Schema:Pay").Value;


            // --- StorageId not yet included -----------------------------------------
            sql = @$"Insert into {schema}.userscompany 
                        (OwnerId,  CompanySName,    CompanyName,     CurrencyId,    AMSSchema,     ApplicantSchema,     PISSchema,     PaySchema) Values 
                        ({userId}, '{companyCode}', '{companyName}', {currencyId},  '{AmsSchema}', '{ApplicantSchema}', '{PisSchema}', '{PaySchema}'); ";
            await _sql.ExecuteCmd(sql, new { });
        }

    }


    // *** Private Functions ----------------------------------------------------
    private async Task _01Menus10User(string schema, string connName)
    {
        string sql = $"select * from {schema}.menus10User limit 1";
        var menu = await _sql.FetchData<MenusModel, dynamic>(sql, new { });
        if (menu.Count == 0)
        {
            sql = @$"insert into {schema}.menus10user
                    (Odr,   Id,     IdParent,	Indent,     Type,    Code,       Icon1,                              Icon2,  DispText,	                    IsWithChild,  IsSelected,	Controller,         Action) values 
                    (1,	    1,      0,			0,		    'MHdr',  'MH001',   'fa-solid fa-user-large',            '',     'My Profile',                  0,				  0,          'Main',		    '_001MyProfile'), 
                    (2,	    2,      0,			0,		    'MHdr',  'MH001',   'fa-solid fa-users-gear',            '',     'Employee Mgt',                0,				  0,          'Main',		    '_002EmployeeMgt'), 
                    (3,	    3,      0,			0,		    'MHdr',  'MH001',   'fa-solid fa-hand-holding-dollar',   '',     'Payroll',                     0,				  0,          'Main',		    '_003Payroll'), 
                    (4,	    4,      0,			0,		    'MHdr',  'MH001',   'fa-solid fa-calculator',            '',     'Accounting',                  0,				  0,          'Main',		    '_004Accounting'), 
                
                    (100,	100,    0,			0,		    'Hdr',   'H001',     '',                                 '',     '',                            1,				  0,          null,		        null), 
                    (200,	200,    0,			0,		    'Hdr',   'H002',     '',                                 '',     '',                            1,				  0,          null,		        null),
                    (300,	300,    0,			0,		    'Hdr',   'H003',     '',                                 '',     '',                            1,				  0,          null,		        null),
                    (400,	400,    0,			0,		    'Hdr',   'H004',     '',                                 '',     '',                            1,				  0,          null,		        null),
            
                    (98,	198,    100,		0,		    'SHdr',  'SH198',    'fa-solid fa-house',                 '',     'Home',                       1,				  0,          null,		        null),  
                    (99,	199,    198,        1,		    'Dtl',   'D199',     'fa-solid fa-house',                 '',     'Home',		                0,				  0,         '00',			    ''), 
                    
                    (110,	110,    100,		0,		    'SHdr',  'SH110',    'fa-solid fa-user',                 '',     'My Profile',                  1,				  0,          null,		        null),  
                    (111,	111,    110,        1,		    'Dtl',   'D111',     'fa-solid fa-circle',               '',     'My 201 Records',		        0,				  0,         'Main',			'_111My201Records'), 
                    (112,	112,    110,        1,		    'Dtl',   'D112',     'fa-solid fa-circle',               '',     'Employment',		            0,				  0,         'Main',			'_112Employment'), 
                    (113,	113,    110,        1,		    'Dtl',   'D113',     'fa-solid fa-circle',               '',     'Trainings',		            0,				  0,         'Main',			'_113Trainings'), 
                    (114,	114,    110,        1,		    'Dtl',   'D114',     'fa-solid fa-circle',               '',     'Uploadables',		            0,				  0,         'Main',			'_114Uploadables'), 
                    
                    (120,	120,    100,		0,		    'SHdr',  'SH120',    'fa fa-cogs',                       '',     'References',                  1,				  0,          null,		        null),  
                    (121,	121,    120,        1,		    'Dtl',   'D121',     'fa fa-university',                 '',     'My Engagement',		        0,				  0,         'Main',			'_121Engagement'), 
                    
                    (130,	130,    100,		0,		    'SHdr',  'SH130',    'fa fa-drivers-license',            '',     'HR Records',                  1,				  0,          null,		        null),  
                    (131,	131,    130,        1,		    'Dtl',   'D131',     'fa fa-reply-all',                  '',     'My Attendance',		        0,				  0,         'Main',			'_131MyAttendance'), 
                    (132,	132,    130,        1,		    'Dtl',   'D132',     'fa fa-reply-all',                  '',     'Leave Application',		    0,				  0,         'Main',			'_132LeaveApplication'), 


                    (150,	150,    100,		0,		    'SHdr',  'SH150',    'fa fa-calculator',                 '',     'Payroll',                     1,				  0,          null,		        null),  
                    (151,	151,    150,        1,		    'Dtl',   'D151',     'fa fa-reply-all',                  '',     'Payslip',		                0,				  0,         'Main',			'_151PaySlip'), 
                    (152,	152,    150,        1,		    'Dtl',   'D152',     'fa fa-reply-all',                  '',     'Schedule Deductions',		    0,				  0,         'Main',			'_152ScheduleDeductions'), 
                    
                    (98,	298,    200,		0,		    'SHdr',  'SH298',    'fa-solid fa-house',                 '',     'Home',                       1,				  0,          null,		        null),  
                    (99,	299,    298,        1,		    'Dtl',   'D299',     'fa-solid fa-house',                 '',     'Home',		                0,				  0,         '00',			    ''), 
                    
                    (210,	210,    200,		0,		    'SHdr',  'SH210',    'fa-solid fa-gears',                '',     'System',                      1,				  0,          null,		        null),                    
                    (211,	211,    210,        1,		    'Dtl',   'D211',     'fa-solid fa-circle',               '',     'Dashboard',	                0,				  0,         'Main',			'_211Dashboard'), 
                    (212,	212,    210,        1,		    'Dtl',   'D212',     'fa-solid fa-circle',               '',     'User Management',		        0,				  0,         'Main',			'_212UserManagement'), 
                    (213,	213,    210,        1,		    'Dtl',   'D213',     'fa-solid fa-circle',               '',     'Password Management',		    0,				  0,         'Main',			'_213PasswordManagement'),
                    (215,	215,    210,        1,		    'Dtl',   'D214',     'fa-solid fa-circle',               '',     'About the System',		    0,				  0,         'Main',			'_214AboutTheSystem'),
                    
                    (220,	220,    200,		0,		    'SHdr',  'SH220',    'fa-solid fa-file-pen',             '',     'Data Entry',                  1,				  0,          null,		        null),
                    (221,	221,    220,        1,		    'Dtl',   'D221',     'fa-solid fa-circle',               '',     'Employee Entry',		        0,				  0,         'Main',			'_221EmployeeEntry'),
                    (222,	222,    220,        1,		    'Dtl',   'D222',     'fa-solid fa-circle',               '',     'Division',    		        0,				  0,         'Main',			'_222Division'),
                    (223,	223,    220,        1,		    'Dtl',   'D223',     'fa-solid fa-circle',               '',     'Department',    		        0,				  0,         'Main',			'_223Department'),
                    (224,	224,    220,        1,		    'Dtl',   'D224',     'fa-solid fa-circle',               '',     'Section',    		            0,				  0,         'Main',			'_224Section'),
                    (225,	225,    220,        1,		    'Dtl',   'D225',     'fa-solid fa-circle',               '',     'Deployment',    		        0,				  0,         'Main',			'_225Deployment'),
                    (226,	226,    220,        1,		    'Dtl',   'D226',     'fa-solid fa-circle',               '',     'Position',    		        0,				  0,         'Main',			'_226Position'),
                    (237,	227,    220,        1,		    'Dtl',   'D227',     'fa-solid fa-circle',               '',     'Designation',    		        0,				  0,         'Main',			'_227Designation'),
                    (228,	228,    220,        1,		    'Dtl',   'D228',     'fa-solid fa-circle',               '',     'Status',    		            0,				  0,         'Main',			'_228Status'),
                    (229,	229,    220,        1,		    'Dtl',   'D229',     'fa-solid fa-circle',               '',     'Leave Mgt',    		        0,				  0,         'Main',			'_229LeaveMgt'),
                    (230,	230,    220,        1,		    'Dtl',   'D230',     'fa-solid fa-circle',               '',     'Penalty Mgt',    		        0,				  0,         'Main',			'_230PenaltyMgt'),
                    (231,	231,    220,        1,		    'Dtl',   'D231',     'fa-solid fa-circle',               '',     'Deviation Mgt',    	        0,				  0,         'Main',			'_231DeviationMgt'),
                    (232,	232,    220,        1,		    'Dtl',   'D231',     'fa-solid fa-circle',               '',     'Employment Type Mgt',    	    0,				  0,         'Main',			'_232EmpTypeMgt'),

                    (240,	240,    200,		0,		    'SHdr',  'SH240',    'fa-solid fa-arrow-right-arrow-left','',     'Transaction',                1,				  0,          null,		        null),
                    (241,	241,    240,        1,		    'Dtl',   'D241',     'fa-solid fa-circle',                '',     'Status Management',		    0,				  0,         'Main',			'_241StatusMgt'),
                    (242,	242,    240,        1,		    'Dtl',   'D242',     'fa-solid fa-circle',                '',     'Employee Deployment',	    0,				  0,         'Main',			'_242EmployeeDeployment'),
                    (243,	243,    240,        1,		    'Dtl',   'D243',     'fa-solid fa-circle',                '',     'Deviation',	                0,				  0,         'Main',			'_243Deviation'),
                    (244,	244,    240,        1,		    'Dtl',   'D244',     'fa-solid fa-circle',                '',     'Disciplinary Action',	    0,				  0,         'Main',			'_244DisciplinaryAction'),
                    (245,	245,    240,        1,		    'Dtl',   'D245',     'fa-solid fa-circle',                '',     'Reinstatement',	            0,				  0,         'Main',			'_245Reinstatement'),
                    (246,	246,    240,        1,		    'Dtl',   'D246',     'fa-solid fa-circle',                '',     'Change Employment',		    0,				  0,         'Main',			'_246ChangeEmployment'),
                    (247,	247,    240,        1,		    'Dtl',   'D247',     'fa-solid fa-circle',                '',     'Recall Employee',            0,				  0,         'Main',			'_247RecallEmployee'),
                    (248,	248,    240,        1,		    'Dtl',   'D248',     'fa-solid fa-circle',                '',     'Group Recall',		        0,				  0,         'Main',			'_248GrpRecall'),
                    (249,	249,    240,        1,		    'Dtl',   'D249',     'fa-solid fa-circle',                '',     'Leave of Absence',		    0,				  0,         'Main',			'_249LeaveOfAbsence'),

                    (260,	260,    200,		0,		    'SHdr',  'SH260',    'fa-solid fa-user-check',            '',     'Accountability',             1,				  0,          null,		        null),
                    (261,	261,    260,        1,		    'Dtl',   'D261',     'fa-solid fa-circle',                '',     'Inventory Entry',	        0,				  0,         'Main',			'_261Inventory'),
                    (262,	262,    260,        1,		    'Dtl',   'D262',     'fa-solid fa-circle',                '',     'Deployment Entry',	        0,				  0,         'Main',			'_262InventoryDeployment'),
                    (263,	263,    260,        1,		    'Dtl',   'D263',     'fa-solid fa-circle',                '',     'Inventory Status Entry',	    0,				  0,         'Main',			'_263InventoryStatus'),
                    (264,	264,    260,        1,		    'Dtl',   'D254',     'fa-solid fa-circle',                '',     'Equipment Report',	        0,				  0,         'Main',			'_264InventoryReport'),

                    (270,	270,    200,		0,		    'SHdr',  'SH270',    'fa-solid fa-chart-line',             '',     'Reports',                   1,				  0,          null,		        null),
                    (271,	271,    270,        1,		    'Dtl',   'D271',     'fa-solid fa-circle',                 '',     'Employee Master List',	    0,				  0,         'Main',			'_281EmployeeMasterList'),
                    (272,	272,    270,        1,		    'Dtl',   'D272',     'fa-solid fa-circle',                 '',     'Employee Status Report',    0,				  0,         'Main',			'_282EmployeeStatusRep'),
                    (273,	273,    270,        1,		    'Dtl',   'D273',     'fa-solid fa-circle',                 '',     'Manpower Movement',	        0,				  0,         'Main',			'_283ManpowerMovement'),
                    (274,	274,    270,        1,		    'Dtl',   'D274',     'fa-solid fa-circle',                 '',     'On Leave Employee',	        0,				  0,         'Main',			'_284OnLeaveEmployee'),
                    (275,	275,    270,        1,		    'Dtl',   'D275',     'fa-solid fa-circle',                 '',     'Newly Hired for the Month', 0,				  0,         'Main',			'_285NewlyHired'),
                    (276,	276,    270,        1,		    'Dtl',   'D276',     'fa-solid fa-circle',                 '',     'For Regularization',	    0,				  0,         'Main',			'_286ForRegularization'),
                    (277,	277,    270,        1,		    'Dtl',   'D277',     'fa-solid fa-circle',                 '',     'Resigned For the Month',    0,				  0,         'Main',			'_287ResignedForTheMonth'),
                    

                    (310,	310,    300,		0,		    'SHdr',  'SH310',    'fa-solid fa-gears',                   '',     'System',                   1,				  0,          null,		        null),                    
                    (311,	311,    310,        1,		    'Dtl',   'D311',     'fa-solid fa-circle',                  '',     'Dashboard',	            0,				  0,         '03',			    '_311Dashboard'), 
                    (312,	312,    310,        1,		    'Dtl',   'D312',     'fa-solid fa-circle',                  '',     'User Management',		    0,				  0,         '03',			    '_312UserManagement'), 
                    (313,	313,    310,        1,		    'Dtl',   'D313',     'fa-solid fa-circle',                  '',     'Password Management',		0,				  0,         '03',			    '_313PasswordManagement'),
                    (314,	314,    310,        1,		    'Dtl',   'D314',     'fa-solid fa-circle',                  '',     'About the System',		    0,				  0,         '03',			    '_315AboutTheSystem'),
                    
                    (320,	320,    300,		0,		    'SHdr',  'SH320',    'fa-solid fa-arrow-right-arrow-left',  '',     'Transaction',               1,				  0,          null,		        null),                    
                    (321,	321,    320,        1,		    'Dtl',   'D321',     'fa-solid fa-circle',                  '',     'Payroll Entry',	         0,				  0,         '03',			    '_321PayrollEntry'), 
                    (322,	322,    320,        1,		    'Dtl',   'D322',     'fa-solid fa-circle',                  '',     'Payroll Groupings',		 0,				  0,         '03',			    '_322PayrollGroupings'),                     
                    (323,	323,    320,        1,		    'Dtl',   'D323',     'fa-solid fa-circle',                  '',     'Employee Earnings',		 1,				  0,          null,			    null),                     
                    (324,	324,    320,        1,		    'Dtl',   'D324',     'fa-solid fa-circle',                  '',     'Earnings Entry',		     0,				  0,         '03',			    '_324EarningsEntry'),                     
                    (325,	325,    320,        1,		    'Dtl',   'D325',     'fa-solid fa-circle',                  '',     'Grp Earnings Entry',		 0,				  0,         '03',			    '_325GrpEarningsEntry'),                     
                    (326,	326,    320,        1,		    'Dtl',   'D326',     'fa-solid fa-circle',                  '',     'Fixed Earnings ',		     0,				  0,         '03',			    '_326FixedEarnings'),                     
                    (327,	327,    320,        1,		    'Dtl',   'D327',     'fa-solid fa-circle',                  '',     'Grp Fixed Earnings',		 0,				  0,         '03',			    '_327GrpFixedEarnings'),                     
                    (328,	328,    320,        1,		    'Dtl',   'D328',     'fa-solid fa-circle',                  '',     'Employee Deductions',		 1,				  0,          null,			    'null'),                     
                    (329,	329,    320,        1,		    'Dtl',   'D329',     'fa-solid fa-circle',                  '',     'Deductions Entry',		     0,				  0,         '03',			    '_329DeductionsEntry'),                     
                    (330,	330,    320,        1,		    'Dtl',   'D330',     'fa-solid fa-circle',                  '',     'Grp Deductions Entry',	     0,				  0,         '03',			    '_330GrpDeductionsEntry'),                     
                    (331,	331,    320,        1,		    'Dtl',   'D331',     'fa-solid fa-circle',                  '',     'Mandatory Deductions ',	 0,				  0,         '03',			    '_331MandatoryDeductions'),                     
                    (332,	332,    320,        1,		    'Dtl',   'D332',     'fa-solid fa-circle',                  '',     'Schedule Deductions',		 0,				  0,         '03',			    '_332ScheduleDeductions'),                     
                    (333,	333,    320,        1,		    'Dtl',   'D333',     'fa-solid fa-circle',                  '',     'Hospitalization',		     0,				  0,         '03',			    '_333Hospitalization'),                     
                   
                    (340,	340,    300,		0,		    'SHdr',  'SH340',    'fa-solid fa-chart-line',              '',     'Reports',                   1,				  0,          null,		        null),                    
                    (341,	341,    340,        1,		    'Dtl',   'D341',     'fa-solid fa-circle',                  '',     'Payslip',	                 0,				  0,         '03',			    '_341Payslip'), 
                    (342,	342,    340,        1,		    'Dtl',   'D342',     'fa-solid fa-circle',                  '',     'Payroll Register',	         0,				  0,         '03',			    '_342PayrollRegister'),


                    (410,	410,    400,		0,		    'SHdr',  'SH410',    'donut_small',                      '',     'Dashboard',                   1,				  0,          null,		        null),  
                    (411,	411,    410,        1,		    'Dtl',   'D411',     'view_module',                        '',     'Transaction Dashboard',		0,				  0,         '04',			    '411'), 

                    (420,	420,    400,		0,		    'SHdr',  'SH420',    'credit_score',                     '',     'Sales and Payments',          1,				  0,          null,		        null),  
                    (421,	421,    420,        1,		    'Dtl',   'D421',     'web_stories',                      '',     'Products and Services',		0,				  0,         '04',			    '421'), 
                    (422,	422,    420,        1,		    'Dtl',   'D422',     'groups',                           '',     'Customers',		            0,				  0,         '04',			    '422'), 
                    (423,	423,    420,        1,		    'Dtl',   'D423',     'document_scanner',                 '',     'Estimates',		            0,				  0,         '04',			    '423'), 
                    (424,	424,    420,        1,		    'Dtl',   'D424',     'description',                      '',     'Invoices',		            0,				  0,         '04',			    '424'), 
                    (425,	425,    420,        1,		    'Dtl',   'D425',     'fact_check',                       '',     'Customer Statements',		    0,				  0,         '04',			    '425'), 
                    
                    (430,	430,    400,		0,		    'SHdr',  'SH430',    'villa',                            '',     'Purchases',                     1,				  0,          null,		        null),  
                    (431,	431,    430,        1,		    'Dtl',   'D431',     'web_stories',                      '',     'Products and Services',		0,				  0,         '04',			    '431'), 
                    (432,	432,    430,        1,		    'Dtl',   'D432',     'aspect_ratio',                     '',     'Bills',		                0,				  0,         '04',			    '432'), 
                    
                    (440,	440,    400,		0,		    'SHdr',  'SH440',    'query_stats',                      '',     'Accounting',                  1,				  0,          null,		        null),  
                    (441,	441,    440,        1,		    'Dtl',   'D441',     'insert_comment'  ,                 '',     'Chart of Accounts',		    0,				  0,         '04',			    '441'), 
                    (442,	442,    440,        1,		    'Dtl',   'D442',     'align_vertical_top',               '',     'Transaction Reconciliation',	0,				  0,         '04',			    '442'), 
                    (443,	443,    440,        1,		    'Dtl',   'D443',     'insert_page_break',                '',     'Transaction',		            0,				  0,         '04',			    '443'), 
                    
                    (450,	450,    400,		0,		    'SHdr',  'SH450',    'widgets',                          '',     'Reports',                     1,				  0,          null,		        null),  
                    (451,	451,    450,        1,		    'Dtl',   'D451',     'widgets',                          '',     'Reports',		                0,				  0,         '04',			    '451')
                    ";
            await _sql.ExecuteCmd(sql, new { }, connName);
            //(133,	133,    130,        1,		    'Dtl',   'D133',     'fa fa-reply-all',                  '',     'Appointment',		            0,				  0,         'Main',			'_133Appointment'), 
        }
    }

    private async Task _01PayrollMenus(string schema, string connName)
    {
        string sql = $"select * from {schema}.menus50Pay limit 1";
        var menu = await _sql.FetchData<MenusModel, dynamic>(sql, new { });
        if (menu.Count == 0)
        {
            sql = @$"insert into {schema}.menus50Pay
                    (Odr,   Id,     IdParent,	Indent,     Type,    Code,       Icon1,                              Icon2,  DispText,	                    IsWithDivider,  IsWithChild,  IsSelected,	Controller,         Action) values 
                    
                    (310,	310,    300,		0,		    'SHdr',  'SH310',    'fa-solid fa-gears',                   '',     'System',                   0,              1,				  0,          null,		        null),                    
                    (311,	311,    310,        1,		    'Dtl',   'D311',     'fa-solid fa-circle',                  '',     'Dashboard',	            0,              0,				  0,         '03',			    '_311Dashboard'), 
                    (312,	312,    310,        1,		    'Dtl',   'D312',     'fa-solid fa-circle',                  '',     'User Management',		    0,              0,				  0,         '03',			    '_312UserManagement'), 
                    (313,	313,    310,        1,		    'Dtl',   'D313',     'fa-solid fa-circle',                  '',     'Password Management',		0,              0,				  0,         '03',			    '_313PasswordManagement'),
                    (314,	314,    310,        1,		    'Dtl',   'D314',     'fa-solid fa-circle',                  '',     'About the System',		    1,              0,				  0,         '03',			    '_315AboutTheSystem'),
                    
                    (320,	320,    300,		0,		    'SHdr',  'SH320',    'fa-solid fa-arrow-right-arrow-left',  '',     'Transaction',              0,              1,				  0,          null,		        null),                    
                    (321,	321,    320,        1,		    'Dtl',   'D321',     'fa-solid fa-circle',                  '',     'Payroll Entry',	        0,              0,				  0,         '03',			    '_321PayrollEntry'), 
                    (322,	322,    320,        1,		    'Dtl',   'D322',     'fa-solid fa-circle',                  '',     'Payroll Groupings',		1,              0,				  0,         '03',			    '_322PayrollGroupings'),                     
                    (323,	323,    320,        1,		    'Dtl',   'D323',     'fa-solid fa-circle',                  '',     'Employee Earnings',		0,              1,				  0,          null,			    null),                     
                    (324,	324,    320,        1,		    'Dtl',   'D324',     'fa-solid fa-circle',                  '',     'Earnings Entry',		    1,              0,				  0,         '03',			    '_324EarningsEntry'),                     
                    (325,	325,    320,        1,		    'Dtl',   'D325',     'fa-solid fa-circle',                  '',     'Grp Earnings Entry',		0,              0,				  0,         '03',			    '_325GrpEarningsEntry'),                     
                    (326,	326,    320,        1,		    'Dtl',   'D326',     'fa-solid fa-circle',                  '',     'Fixed Earnings ',		    0,              0,				  0,         '03',			    '_326FixedEarnings'),                     
                    (327,	327,    320,        1,		    'Dtl',   'D327',     'fa-solid fa-circle',                  '',     'Grp Fixed Earnings',		0,              0,				  0,         '03',			    '_327GrpFixedEarnings'),                     
                    (328,	328,    320,        1,		    'Dtl',   'D328',     'fa-solid fa-circle',                  '',     'Employee Deductions',		1,              1,				  0,          null,			    'null'),                     
                    (329,	329,    320,        1,		    'Dtl',   'D329',     'fa-solid fa-circle',                  '',     'Deductions Entry',		    0,              0,				  0,         '03',			    '_329DeductionsEntry'),                     
                    (330,	330,    320,        1,		    'Dtl',   'D330',     'fa-solid fa-circle',                  '',     'Grp Deductions Entry',	    0,              0,				  0,         '03',			    '_330GrpDeductionsEntry'),                     
                    (331,	331,    320,        1,		    'Dtl',   'D331',     'fa-solid fa-circle',                  '',     'Mandatory Deductions ',	0,              0,				  0,         '03',			    '_331MandatoryDeductions'),                     
                    (332,	332,    320,        1,		    'Dtl',   'D332',     'fa-solid fa-circle',                  '',     'Schedule Deductions',		0,              0,				  0,         '03',			    '_332ScheduleDeductions'),                     
                    (333,	333,    320,        1,		    'Dtl',   'D333',     'fa-solid fa-circle',                  '',     'Hospitalization',		    0,              0,				  0,         '03',			    '_333Hospitalization'),                     
                   
                    (340,	340,    300,		0,		    'SHdr',  'SH340',    'fa-solid fa-chart-line',              '',     'Reports',                  0,              1,				  0,          null,		        null),                    
                    (341,	341,    340,        1,		    'Dtl',   'D341',     'fa-solid fa-circle',                  '',     'Payslip',	                0,              0,				  0,         '03',			    '_341Payslip'), 
                    (342,	342,    340,        1,		    'Dtl',   'D342',     'fa-solid fa-circle',                  '',     'Payroll Register',	        0,              0,				  0,         '03',			    '_342PayrollRegister')
                    ";
            await _sql.ExecuteCmd(sql, new { }, connName);

        }
    }

    private async Task _01Users_DefaulDatas(string schema, string connName)
    {
        string sql = $"select * from {schema}.users limit 1";
        var user = await _sql.FetchData<UsersModel, dynamic>(sql, new { }, connName);
        if (user.Count == 0)
        {
            sql = @$"insert into {schema}.users 
                  (LoginName,    Password,           Email) values 
                  ('marmagsael', sha2('635421',512), 'marmagsael@gmail.com'), 
                  ('judith',     sha2('123456',512), 'jreyes@gmail.com'), 
                  ('001',        sha2('123456',512), '001@email.com')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async void _01Country_DefaultDatas(string schema, string connName)
    {
        string sql = $"select * from {schema}.Country limit 1";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, connName);
        if (res.Count == 0)
        {
            sql = @$"insert into {schema}.Country 
                  (Code, Name) values 
                  ('CAD', 'Canada'), 
		          ('PHL', 'Philippines'); ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }
    private async void _01Currency_DefaultDatas(string schema, string connNam)
    {
        string sql = $"select * from {schema}.Currency limit 1";
        var res = await _sql.FetchData<CurrencyModel, dynamic>(sql, new { }, connNam);
        if (res.Count == 0)
        {
            sql = @$"insert into {schema}.Currency 
                  (Code, Name, Symbol) values 
                  ('CAD', 'Canada', 'C$'), 
		          ('PHL', 'Philippines','Php'); ";
            await _sql.ExecuteCmd(sql, new { }, connNam);
        }

    }



}
