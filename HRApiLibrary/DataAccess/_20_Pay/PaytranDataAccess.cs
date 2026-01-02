
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PaytranDataAccess : IPaytranDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PaytranDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(PaytranModel paytran, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Paytran 
                             (Trn, EmpmasId, PayrollgrpId, AttStart, AttEnd, E000U, E000R,  
                             E000M, E000A, E001U, E001R, E001M, E001A, E002U, E002R, E002M,  
                             E002A, E003U, E003R, E003M, E003A, E004U, E004R, E004M, E004A,  
                             E005U, E005R, E005M, E005A, E006U, E006R, E006M, E006A, E007U,  
                             E007R, E007M, E007A, E008U, E008R, E008M, E008A, E009U, E009R,  
                             E009M, E009A, E010U, E010R, E010M, E010A, E011U, E011R, E011M,  
                             E011A, E012U, E012R, E012M, E012A, E013U, E013R, E013M, E013A,  
                             E014U, E014R, E014M, E014A, E015U, E015R, E015M, E015A, E016U,  
                             E016R, E016M, E016A, E017U, E017R, E017M, E017A, E018U, E018R,  
                             E018M, E018A, E019U, E019R, E019M, E019A, E020U, E020R, E020M,  
                             E020A, E021U, E021R, E021M, E021A, E022U, E022R, E022M, E022A,  
                             E023U, E023R, E023M, E023A, E024U, E024R, E024M, E024A, E025U,  
                             E025R, E025M, E025A, E026U, E026R, E026M, E026A, E027U, E027R,  
                             E027M, E027A, E028U, E028R, E028M, E028A, E029U, E029R, E029M,  
                             E029A, E030U, E030R, E030M, E030A, E031U, E031R, E031M, E031A,  
                             E032U, E032R, E032M, E032A, E033U, E033R, E033M, E033A, E034U,  
                             E034R, E034M, E034A, E035U, E035R, E035M, E035A, E036U, E036R,  
                             E036M, E036A, E037U, E037R, E037M, E037A, E038U, E038R, E038M,  
                             E038A, E039U, E039R, E039M, E039A, E040U, E040R, E040M, E040A,  
                             E041U, E041R, E041M, E041A, E042U, E042R, E042M, E042A, E043U,  
                             E043R, E043M, E043A, E044U, E044R, E044M, E044A, E045U, E045R,  
                             E045M, E045A, E046U, E046R, E046M, E046A, E047U, E047R, E047M,  
                             E047A, E048U, E048R, E048M, E048A, E049U, E049R, E049M, E049A,  
                             E050U, E050R, E050M, E050A, E051U, E051R, E051M, E051A, E052U,  
                             E052R, E052M, E052A, E053U, E053R, E053M, E053A, E054U, E054R,  
                             E054M, E054A, E055U, E055R, E055M, E055A, E056U, E056R, E056M,  
                             E056A, E057U, E057R, E057M, E057A, E058U, E058R, E058M, E058A,  
                             E059U, E059R, E059M, E059A, E060U, E060R, E060M, E060A, E061U,  
                             E061R, E061M, E061A, E062U, E062R, E062M, E062A, E063U, E063R,  
                             E063M, E063A, E064U, E064R, E064M, E064A, E065U, E065R, E065M,  
                             E065A, E066U, E066R, E066M, E066A, E067U, E067R, E067M, E067A,  
                             E068U, E068R, E068M, E068A, E069U, E069R, E069M, E069A, E070U,  
                             E070R, E070M, E070A, E071U, E071R, E071M, E071A, E072U, E072R,  
                             E072M, E072A, E073U, E073R, E073M, E073A, E074U, E074R, E074M,  
                             E074A, E075U, E075R, E075M, E075A, E076U, E076R, E076M, E076A,  
                             E077U, E077R, E077M, E077A, E078U, E078R, E078M, E078A, E079U,  
                             E079R, E079M, E079A, E080U, E080R, E080M, E080A, E081U, E081R,  
                             E081M, E081A, E082U, E082R, E082M, E082A, E083U, E083R, E083M,  
                             E083A, E084U, E084R, E084M, E084A, E085U, E085R, E085M, E085A,  
                             E086U, E086R, E086M, E086A, E087U, E087R, E087M, E087A, E088U,  
                             E088R, E088M, E088A, E089U, E089R, E089M, E089A, E090U, E090R,  
                             E090M, E090A, E091U, E091R, E091M, E091A, E092U, E092R, E092M,  
                             E092A, E093U, E093R, E093M, E093A, E094U, E094R, E094M, E094A,  
                             E095U, E095R, E095M, E095A, E096U, E096R, E096M, E096A, E097U,  
                             E097R, E097M, E097A, E098U, E098R, E098M, E098A, E099U, E099R,  
                             E099M, E099A, E100U, E100R, E100M, E100A, E101U, E101R, E101M,  
                             E101A, E102U, E102R, E102M, E102A, E103U, E103R, E103M, E103A,  
                             E104U, E104R, E104M, E104A, E105U, E105R, E105M, E105A, E106U,  
                             E106R, E106M, E106A, E107U, E107R, E107M, E107A, E108U, E108R,  
                             E108M, E108A, E109U, E109R, E109M, E109A, E110U, E110R, E110M,  
                             E110A, E111U, E111R, E111M, E111A, E112U, E112R, E112M, E112A,  
                             E113U, E113R, E113M, E113A, E114U, E114R, E114M, E114A, E115U,  
                             E115R, E115M, E115A, E116U, E116R, E116M, E116A, E117U, E117R,  
                             E117M, E117A, E118U, E118R, E118M, E118A, E119U, E119R, E119M,  
                             E119A) values 
                            (@Trn, @EmpmasId, @PayrollgrpId, @AttStart, @AttEnd, @E000U, @E000R, @E000M, @E000A, @E001U, @E001R, @E001M, @E001A,  
                             @E002U, @E002R, @E002M, @E002A, @E003U, @E003R, @E003M, @E003A, @E004U, @E004R, @E004M, @E004A, @E005U, @E005R, @E005M,  
                             @E005A, @E006U, @E006R, @E006M, @E006A, @E007U, @E007R, @E007M, @E007A, @E008U, @E008R, @E008M, @E008A, @E009U, @E009R,  
                             @E009M, @E009A, @E010U, @E010R, @E010M, @E010A, @E011U, @E011R, @E011M, @E011A, @E012U, @E012R, @E012M, @E012A, @E013U,  
                             @E013R, @E013M, @E013A, @E014U, @E014R, @E014M, @E014A, @E015U, @E015R, @E015M, @E015A, @E016U, @E016R, @E016M, @E016A,  
                             @E017U, @E017R, @E017M, @E017A, @E018U, @E018R, @E018M, @E018A, @E019U, @E019R, @E019M, @E019A, @E020U, @E020R, @E020M,  
                             @E020A, @E021U, @E021R, @E021M, @E021A, @E022U, @E022R, @E022M, @E022A, @E023U, @E023R, @E023M, @E023A, @E024U, @E024R,  
                             @E024M, @E024A, @E025U, @E025R, @E025M, @E025A, @E026U, @E026R, @E026M, @E026A, @E027U, @E027R, @E027M, @E027A, @E028U,  
                             @E028R, @E028M, @E028A, @E029U, @E029R, @E029M, @E029A, @E030U, @E030R, @E030M, @E030A, @E031U, @E031R, @E031M, @E031A,  
                             @E032U, @E032R, @E032M, @E032A, @E033U, @E033R, @E033M, @E033A, @E034U, @E034R, @E034M, @E034A, @E035U, @E035R, @E035M,  
                             @E035A, @E036U, @E036R, @E036M, @E036A, @E037U, @E037R, @E037M, @E037A, @E038U, @E038R, @E038M, @E038A, @E039U, @E039R,  
                             @E039M, @E039A, @E040U, @E040R, @E040M, @E040A, @E041U, @E041R, @E041M, @E041A, @E042U, @E042R, @E042M, @E042A, @E043U,  
                             @E043R, @E043M, @E043A, @E044U, @E044R, @E044M, @E044A, @E045U, @E045R, @E045M, @E045A, @E046U, @E046R, @E046M, @E046A,  
                             @E047U, @E047R, @E047M, @E047A, @E048U, @E048R, @E048M, @E048A, @E049U, @E049R, @E049M, @E049A, @E050U, @E050R, @E050M,  
                             @E050A, @E051U, @E051R, @E051M, @E051A, @E052U, @E052R, @E052M, @E052A, @E053U, @E053R, @E053M, @E053A, @E054U, @E054R,  
                             @E054M, @E054A, @E055U, @E055R, @E055M, @E055A, @E056U, @E056R, @E056M, @E056A, @E057U, @E057R, @E057M, @E057A, @E058U,  
                             @E058R, @E058M, @E058A, @E059U, @E059R, @E059M, @E059A, @E060U, @E060R, @E060M, @E060A, @E061U, @E061R, @E061M, @E061A,  
                             @E062U, @E062R, @E062M, @E062A, @E063U, @E063R, @E063M, @E063A, @E064U, @E064R, @E064M, @E064A, @E065U, @E065R, @E065M,  
                             @E065A, @E066U, @E066R, @E066M, @E066A, @E067U, @E067R, @E067M, @E067A, @E068U, @E068R, @E068M, @E068A, @E069U, @E069R,  
                             @E069M, @E069A, @E070U, @E070R, @E070M, @E070A, @E071U, @E071R, @E071M, @E071A, @E072U, @E072R, @E072M, @E072A, @E073U,  
                             @E073R, @E073M, @E073A, @E074U, @E074R, @E074M, @E074A, @E075U, @E075R, @E075M, @E075A, @E076U, @E076R, @E076M, @E076A,  
                             @E077U, @E077R, @E077M, @E077A, @E078U, @E078R, @E078M, @E078A, @E079U, @E079R, @E079M, @E079A, @E080U, @E080R, @E080M,  
                             @E080A, @E081U, @E081R, @E081M, @E081A, @E082U, @E082R, @E082M, @E082A, @E083U, @E083R, @E083M, @E083A, @E084U, @E084R,  
                             @E084M, @E084A, @E085U, @E085R, @E085M, @E085A, @E086U, @E086R, @E086M, @E086A, @E087U, @E087R, @E087M, @E087A, @E088U,  
                             @E088R, @E088M, @E088A, @E089U, @E089R, @E089M, @E089A, @E090U, @E090R, @E090M, @E090A, @E091U, @E091R, @E091M, @E091A,  
                             @E092U, @E092R, @E092M, @E092A, @E093U, @E093R, @E093M, @E093A, @E094U, @E094R, @E094M, @E094A, @E095U, @E095R, @E095M,  
                             @E095A, @E096U, @E096R, @E096M, @E096A, @E097U, @E097R, @E097M, @E097A, @E098U, @E098R, @E098M, @E098A, @E099U, @E099R,  
                             @E099M, @E099A, @E100U, @E100R, @E100M, @E100A, @E101U, @E101R, @E101M, @E101A, @E102U, @E102R, @E102M, @E102A, @E103U,  
                             @E103R, @E103M, @E103A, @E104U, @E104R, @E104M, @E104A, @E105U, @E105R, @E105M, @E105A, @E106U, @E106R, @E106M, @E106A,  
                             @E107U, @E107R, @E107M, @E107A, @E108U, @E108R, @E108M, @E108A, @E109U, @E109R, @E109M, @E109A, @E110U, @E110R, @E110M,  
                             @E110A, @E111U, @E111R, @E111M, @E111A, @E112U, @E112R, @E112M, @E112A, @E113U, @E113R, @E113M, @E113A, @E114U, @E114R,  
                             @E114M, @E114A, @E115U, @E115R, @E115M, @E115A, @E116U, @E116R, @E116M, @E116A, @E117U, @E117R, @E117M, @E117A, @E118U,  
                             @E118R, @E118M, @E118A, @E119U, @E119R, @E119M, @E119A) on duplicate key update E0000M=@E0000M";
        await _sql.ExecuteCmd<dynamic>(sql, paytran, conn);

    }

    
    public async Task _01New(PaytranModel paytran, string schema, string conn)
    {
        var  sql = $@"Insert into {schema}.Paytran (Trn, EmpmasId, PayrollgrpId) values (@Trn, @EmpmasId, @PayrollgrpId) 
                            on duplicate key update E0000M=@E0000M";
        await _sql.ExecuteCmd<dynamic>(sql, paytran, conn);
    }
    
    public async Task<List<PaytranModel?>?> _01New(string trn, List<PaytranModel?> paytrans, string paydb, string pisdb, string conn)
    {
        foreach (var paytran in paytrans)
        {
            var  sql = $@"Insert into {paydb}.Paytran (Trn, EmpmasId, PayrollgrpId) values (@Trn, @EmpmasId, @PayrollgrpId) 
                            on duplicate key update E0000M=@E0000M";
            if (paytran != null) await _sql.ExecuteCmd<dynamic>(sql, paytran, conn);
        }
        var res = await _02ByTrn(trn, paydb, pisdb, conn);
        return res; 
    }

    public async Task<List<PaytranModel?>?> _02ByTrn(string trn, string paydb, string pisdb, string conn)
    {
        string sql = $@"select  concat(trim(e.empLastnm), ', ', trim(e.empfirstNm), '', trim(e.empMidNm)) EmpName, p.* from {paydb}.Paytran p 
                        left join {pisdb}.Empmas e on e.Id = p.EmpmasId 
                        where Trn = @Trn";
        var data = await _sql.FetchData<PaytranModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }


    public async Task _03(PaytranModel paytran, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paytran set E000U = @E000U, E000R = @E000R, E000M = @E000M, E000A = @E000A,  
                                                    E001U = @E001U, E001R = @E001R, E001M = @E001M, E001A = @E001A, 
                                                    E002U = @E002U, E002R = @E002R, E002M = @E002M, E002A = @E002A, 
                                                    E003U = @E003U, E003R = @E003R, E003M = @E003M, E003A = @E003A, 
                                                    E004U = @E004U, E004R = @E004R, E004M = @E004M, E004A = @E004A, 
                                                    E005U = @E005U, E005R = @E005R, E005M = @E005M, E005A = @E005A, 
                                                    E006U = @E006U, E006R = @E006R, E006M = @E006M, E006A = @E006A, 
                                                    E007U = @E007U, E007R = @E007R, E007M = @E007M, E007A = @E007A, 
                                                    E008U = @E008U, E008R = @E008R, E008M = @E008M, E008A = @E008A, 
                                                    E009U = @E009U, E009R = @E009R, E009M = @E009M, E009A = @E009A, 
                                                    E010U = @E010U, E010R = @E010R, E010M = @E010M, E010A = @E010A, 
                                                    E011U = @E011U, E011R = @E011R, E011M = @E011M, E011A = @E011A, 
                                                    E012U = @E012U, E012R = @E012R, E012M = @E012M, E012A = @E012A, 
                                                    E013U = @E013U, E013R = @E013R, E013M = @E013M, E013A = @E013A, 
                                                    E014U = @E014U, E014R = @E014R, E014M = @E014M, E014A = @E014A, 
                                                    E015U = @E015U, E015R = @E015R, E015M = @E015M, E015A = @E015A, 
                                                    E016U = @E016U, E016R = @E016R, E016M = @E016M, E016A = @E016A, 
                                                    E017U = @E017U, E017R = @E017R, E017M = @E017M, E017A = @E017A, 
                                                    E018U = @E018U, E018R = @E018R, E018M = @E018M, E018A = @E018A, 
                                                    E019U = @E019U, E019R = @E019R, E019M = @E019M, E019A = @E019A, 
                                                    E020U = @E020U, E020R = @E020R, E020M = @E020M, E020A = @E020A, 
                                                    E021U = @E021U, E021R = @E021R, E021M = @E021M, E021A = @E021A, 
                                                    E022U = @E022U, E022R = @E022R, E022M = @E022M, E022A = @E022A, 
                                                    E023U = @E023U, E023R = @E023R, E023M = @E023M, E023A = @E023A, 
                                                    E024U = @E024U, E024R = @E024R, E024M = @E024M, E024A = @E024A, 
                                                    E025U = @E025U, E025R = @E025R, E025M = @E025M, E025A = @E025A, 
                                                    E026U = @E026U, E026R = @E026R, E026M = @E026M, E026A = @E026A, 
                                                    E027U = @E027U, E027R = @E027R, E027M = @E027M, E027A = @E027A, 
                                                    E028U = @E028U, E028R = @E028R, E028M = @E028M, E028A = @E028A, 
                                                    E029U = @E029U, E029R = @E029R, E029M = @E029M, E029A = @E029A, 
                                                    E030U = @E030U, E030R = @E030R, E030M = @E030M, E030A = @E030A, 
                                                    E031U = @E031U, E031R = @E031R, E031M = @E031M, E031A = @E031A, 
                                                    E032U = @E032U, E032R = @E032R, E032M = @E032M, E032A = @E032A, 
                                                    E033U = @E033U, E033R = @E033R, E033M = @E033M, E033A = @E033A, 
                                                    E034U = @E034U, E034R = @E034R, E034M = @E034M, E034A = @E034A, 
                                                    E035U = @E035U, E035R = @E035R, E035M = @E035M, E035A = @E035A,
                                                    E036U = @E036U, E036R = @E036R, E036M = @E036M, E036A = @E036A, 
                                                    E037U = @E037U, E037R = @E037R, E037M = @E037M, E037A = @E037A, 
                                                    E038U = @E038U, E038R = @E038R, E038M = @E038M, E038A = @E038A, 
                                                    E039U = @E039U, E039R = @E039R, E039M = @E039M, E039A = @E039A, 
                                                    E040U = @E040U, E040R = @E040R, E040M = @E040M, E040A = @E040A, 
                                                    E041U = @E041U, E041R = @E041R, E041M = @E041M, E041A = @E041A, 
                                                    E042U = @E042U, E042R = @E042R, E042M = @E042M, E042A = @E042A, 
                                                    E043U = @E043U, E043R = @E043R, E043M = @E043M, E043A = @E043A, 
                                                    E044U = @E044U, E044R = @E044R, E044M = @E044M, E044A = @E044A, 
                                                    E045U = @E045U, E045R = @E045R, E045M = @E045M, E045A = @E045A, 
                                                    E046U = @E046U, E046R = @E046R, E046M = @E046M, E046A = @E046A, 
                                                    E047U = @E047U, E047R = @E047R, E047M = @E047M, E047A = @E047A, 
                                                    E048U = @E048U, E048R = @E048R, E048M = @E048M, E048A = @E048A, 
                                                    E049U = @E049U, E049R = @E049R, E049M = @E049M, E049A = @E049A, 
                                                    E050U = @E050U, E050R = @E050R, E050M = @E050M, E050A = @E050A, 
                                                    E051U = @E051U, E051R = @E051R, E051M = @E051M, E051A = @E051A, 
                                                    E052U = @E052U, E052R = @E052R, E052M = @E052M, E052A = @E052A, 
                                                    E053U = @E053U, E053R = @E053R, E053M = @E053M, E053A = @E053A, 
                                                    E054U = @E054U, E054R = @E054R, E054M = @E054M, E054A = @E054A, 
                                                    E055U = @E055U, E055R = @E055R, E055M = @E055M, E055A = @E055A, 
                                                    E056U = @E056U, E056R = @E056R, E056M = @E056M, E056A = @E056A,
                                                    E057U = @E057U, E057R = @E057R, E057M = @E057M, E057A = @E057A, 
                                                    E058U = @E058U, E058R = @E058R, E058M = @E058M, E058A = @E058A, 
                                                    E059U = @E059U, E059R = @E059R, E059M = @E059M, E059A = @E059A, 
                                                    E060U = @E060U, E060R = @E060R, E060M = @E060M, E060A = @E060A, 
                                                    E061U = @E061U, E061R = @E061R, E061M = @E061M, E061A = @E061A, 
                                                    E062U = @E062U, E062R = @E062R, E062M = @E062M, E062A = @E062A, 
                                                    E063U = @E063U, E063R = @E063R, E063M = @E063M, E063A = @E063A, 
                                                    E064U = @E064U, E064R = @E064R, E064M = @E064M, E064A = @E064A, 
                                                    E065U = @E065U, E065R = @E065R, E065M = @E065M, E065A = @E065A, 
                                                    E066U = @E066U, E066R = @E066R, E066M = @E066M, E066A = @E066A, 
                                                    E067U = @E067U, E067R = @E067R, E067M = @E067M, E067A = @E067A, 
                                                    E068U = @E068U, E068R = @E068R, E068M = @E068M, E068A = @E068A, 
                                                    E069U = @E069U, E069R = @E069R, E069M = @E069M, E069A = @E069A, 
                                                    E070U = @E070U, E070R = @E070R, E070M = @E070M, E070A = @E070A, 
                                                    E071U = @E071U, E071R = @E071R, E071M = @E071M, E071A = @E071A, 
                                                    E072U = @E072U, E072R = @E072R, E072M = @E072M, E072A = @E072A, 
                                                    E073U = @E073U, E073R = @E073R, E073M = @E073M, E073A = @E073A, 
                                                    E074U = @E074U, E074R = @E074R, E074M = @E074M, E074A = @E074A, 
                                                    E075U = @E075U, E075R = @E075R, E075M = @E075M, E075A = @E075A, 
                                                    E076U = @E076U, E076R = @E076R, E076M = @E076M, E076A = @E076A, 
                                                    E077U = @E077U, E077R = @E077R, E077M = @E077M, E077A = @E077A, 
                                                    E078U = @E078U, E078R = @E078R, E078M = @E078M, E078A = @E078A, 
                                                    E079U = @E079U, E079R = @E079R, E079M = @E079M, E079A = @E079A, 
                                                    E080U = @E080U, E080R = @E080R, E080M = @E080M, E080A = @E080A,
                                                    E081U = @E081U, E081R = @E081R, E081M = @E081M, E081A = @E081A, 
                                                    E082U = @E082U, E082R = @E082R, E082M = @E082M, E082A = @E082A, 
                                                    E083U = @E083U, E083R = @E083R, E083M = @E083M, E083A = @E083A, 
                                                    E084U = @E084U, E084R = @E084R, E084M = @E084M, E084A = @E084A, 
                                                    E085U = @E085U, E085R = @E085R, E085M = @E085M, E085A = @E085A, 
                                                    E086U = @E086U, E086R = @E086R, E086M = @E086M, E086A = @E086A, 
                                                    E087U = @E087U, E087R = @E087R, E087M = @E087M, E087A = @E087A, 
                                                    E088U = @E088U, E088R = @E088R, E088M = @E088M, E088A = @E088A, 
                                                    E089U = @E089U, E089R = @E089R, E089M = @E089M, E089A = @E089A, 
                                                    E090U = @E090U, E090R = @E090R, E090M = @E090M, E090A = @E090A, 
                                                    E091U = @E091U, E091R = @E091R, E091M = @E091M, E091A = @E091A, 
                                                    E092U = @E092U, E092R = @E092R, E092M = @E092M, E092A = @E092A, 
                                                    E093U = @E093U, E093R = @E093R, E093M = @E093M, E093A = @E093A, 
                                                    E094U = @E094U, E094R = @E094R, E094M = @E094M, E094A = @E094A, 
                                                    E095U = @E095U, E095R = @E095R, E095M = @E095M, E095A = @E095A, 
                                                    E096U = @E096U, E096R = @E096R, E096M = @E096M, E096A = @E096A, 
                                                    E097U = @E097U, E097R = @E097R, E097M = @E097M, E097A = @E097A, 
                                                    E098U = @E098U, E098R = @E098R, E098M = @E098M, E098A = @E098A, 
                                                    E099U = @E099U, E099R = @E099R, E099M = @E099M, E099A = @E099A, 
                                                    E100U = @E100U, E100R = @E100R, E100M = @E100M, E100A = @E100A, 
                                                    E101U = @E101U, E101R = @E101R, E101M = @E101M, E101A = @E101A, 
                                                    E102U = @E102U, E102R = @E102R, E102M = @E102M, E102A = @E102A, 
                                                    E103U = @E103U, E103R = @E103R, E103M = @E103M, E103A = @E103A, 
                                                    E104U = @E104U, E104R = @E104R, E104M = @E104M, E104A = @E104A, 
                                                    E105U = @E105U, E105R = @E105R, E105M = @E105M, E105A = @E105A, 
                                                    E106U = @E106U, E106R = @E106R, E106M = @E106M, E106A = @E106A, 
                                                    E107U = @E107U, E107R = @E107R, E107M = @E107M, E107A = @E107A, 
                                                    E108U = @E108U, E108R = @E108R, E108M = @E108M, E108A = @E108A, 
                                                    E109U = @E109U, E109R = @E109R, E109M = @E109M, E109A = @E109A, 
                                                    E110U = @E110U, E110R = @E110R, E110M = @E110M, E110A = @E110A, 
                                                    E111U = @E111U, E111R = @E111R, E111M = @E111M, E111A = @E111A, 
                                                    E112U = @E112U, E112R = @E112R, E112M = @E112M, E112A = @E112A, 
                                                    E113U = @E113U, E113R = @E113R, E113M = @E113M, E113A = @E113A, 
                                                    E114U = @E114U, E114R = @E114R, E114M = @E114M, E114A = @E114A, 
                                                    E115U = @E115U, E115R = @E115R, E115M = @E115M, E115A = @E115A, 
                                                    E116U = @E116U, E116R = @E116R, E116M = @E116M, E116A = @E116A, 
                                                    E117U = @E117U, E117R = @E117R, E117M = @E117M, E117A = @E117A, 
                                                    E118U = @E118U, E118R = @E118R, E118M = @E118M, E118A = @E118A, 
                                                    E119U = @E119U, E119R = @E119R, E119M = @E119M, E119A = @E119A 
                            where Trn=@Trn and EmpmasId = @EmpmasId;";
        await _sql.ExecuteCmd<dynamic>(sql, paytran, conn);
    }

    public async Task _03AttDuration(string trn, DateTime attStart, DateTime attEnd, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paytran set AttStart = @AttStart, AttEnd = @AttEnd where Trn=@Trn;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, AttStart = attStart, AttEnd = attEnd }, conn);
    }

    public async Task _03PayrollgrpId(string trn, int payrollgrpId, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paytran set AttStart = @AttStart, AttEnd = @AttEnd where Trn=@Trn;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn, PayrollgrpId = payrollgrpId }, conn);
    }

    public async Task _04ByTrn(string trn, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paytran where Trn = @Trn;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn }, conn);

    }
}