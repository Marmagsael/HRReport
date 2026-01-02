using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.M0605;
using HRApiLibrary.Models._90_Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace HRApiLibrary.DataAccess._90_Utils;


public class MsdsDataAccess : IMsdsDataAccess
{

    public async Task SendEmail(EmailModel email)
    {


        var message = new MimeMessage();
        // Sender's email 
        message.From.Add(new MailboxAddress("MSDS", email.SenderEmail));
        //message.ReplyTo.Add(new MailboxAddress(hrName, hrEmail));


        // Recipient's email 
        message.To.Add(MailboxAddress.Parse(email.RecipientEmail));
        message.Subject = email.Subject;
        var bodyBuilder = new BodyBuilder { };


        switch (email.Type)
        {
            case "Profile Request":
                bodyBuilder = new BodyBuilder
                {

                    HtmlBody = $@"
                        <html>
                            <body style='margin: 0; padding: 40px; background-color: #f4f4f4; font-family: Roboto-Regular,Helvetica,Arial, sans-serif; font-size:13px; line-height:20px; color: #333;'>
                                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                                    <div style='text-align: center; margin-bottom: 20px;'>
                                        <img src='LOGO' alt='Company Logo' style='height: 60px;' />
                                    </div>
                                    <h2>Profile Link Request</h2>
                                    <p>Hi {email.RecipientName},</p>
                                    <p><strong>{email.CompanyName}</strong> has invited you to link your profile to their company account.</p>
                                    <p>By granting access, you authorize {email.CompanyName} to view and use your personal information for the purpose of integrating and maintaining your official HR records within the {email.CompanyName} system.</p>
                                    <div style='text-align: center; margin: 30px 0;'>
                                        <a href='https://localhost:7065/00' style='background-color: #047959; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 5px;'>Link My Profile</a>
                                    </div>
                                    <p>  If you did not expect this request, you may deny access or  <a href='mailto:{email.SenderEmail}' >contact HR</a>  to have your email removed.</p>
                                    <p>Thank you,<br />MSDS</p>
                                </div>
                            </body>
                        </html>",
                    TextBody = $@"
                            Profile Link Request

                            Hi {email.RecipientName},

                            {email.CompanyName} has invited you to link your profile to their company account.

                            By granting access, you authorize {email.CompanyName} to view and use your personal information for the purpose of integrating and maintaining your official HR records within the {email.CompanyName} system.

                            To link your profile, click the button below:
                            https://localhost:7065/00

                            If you did not expect this request, you may deny access or contact HR to have your email removed.

                            Thank you,
                            MSDS"


                };
                break;

            case "Profile Approved":

                var allowedModules = new List<string>();
                if (email.Modules?.Info == 1) allowedModules.Add("Info");
                if (email.Modules?.PersonalData == 1) allowedModules.Add("Personal Data");
                if (email.Modules?.Address == 1) allowedModules.Add("Address");
                if (email.Modules?.Education == 1) allowedModules.Add("Education");
                if (email.Modules?.Family == 1) allowedModules.Add("Family");
                if (email.Modules?.References == 1) allowedModules.Add("References");
                if (email.Modules?.Employment ==1 ) allowedModules.Add("Employment");
                if (email.Modules?.Trainings == 1) allowedModules.Add("Trainings");

                string moduleChecklistHtml = string.Join("", allowedModules.Select(m =>
                    $"<li style='margin-bottom: 8px; display: flex; align-items: center;'><span style='font-size: 18px; margin-right: 8px;'>✅</span>{m}</li>"));

                string moduleChecklistText = string.Join("\n", allowedModules.Select(m => $"[✔] {m}"));

                bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                                <html>
                                     <body style='margin: 0; padding: 40px; background-color: #f4f4f4; font-family: Roboto-Regular,Helvetica,Arial, sans-serif; font-size:13px; line-height:20px; color: #333;'>

                                        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                                            <div style='text-align: center; margin-bottom: 20px;'>
                                                <img src='LOGO' alt='Company Logo' style='height: 60px;' />
                                            </div>
                                            <h2>Profile Link Approved</h2>
                                            <p>Hi {email.RecipientName},</p>
                                            <p>This is to confirm that <strong>{email.SenderName}</strong> has approved your request to link his/her profile to <strong>{email.CompanyName}</strong>.</p>
                                            <p>As part of this approval, the following modules have been made accessible to your company account:</p>
                                            {(allowedModules.Count > 0 ? $@"
                                        
                                            <ul style='list-style: none; padding-left: 0; display: grid; grid-template-columns: repeat(2, 1fr); gap: 10px;'>
                                                {moduleChecklistHtml}
                                            </ul>" : "<p>No modules were granted access at this time.</p>")}
                                            
                                            <p>Thank you,<br />MSDS</p>
                                        </div>
                                    </body>
                                </html>",

                TextBody = $@"
                            Profile Link Approved

                            Hi {email.RecipientName},

                            This is to confirm that {email.SenderName} has approved your request to link his/her profile to {email.CompanyName}.

                            As part of this approval, the following modules have been made accessible to your company account:
                            {(allowedModules.Count > 0 ? $"\n{moduleChecklistText}" : "\nNo modules were granted access at this time.")}

                            

                            Thank you,
                            MSDS"
                    };
                break;


            case "Profile Denied":
                bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                             <body style='margin: 0; padding: 40px; background-color: #f4f4f4; font-family: Roboto-Regular,Helvetica,Arial, sans-serif; font-size:13px; line-height:20px; color: #333;'>
                                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                                    <div style='text-align: center; margin-bottom: 20px;'>
                                        <img src='LOGO' alt='Company Logo' style='height: 60px;' />
                                    </div>
                                    <h2>Profile Link Request Denied</h2>
                                    <p>Hi {email.RecipientName},</p>
                                    <p>We regret to inform you that <strong>{email.SenderName}</strong> has declined your request to link his/her profile to <strong>{email.CompanyName}</strong>.</p>
                                    <p>This means the profile will remain unlinked and no access to personal or employment data will be granted at this time.</p>
                                    <p>If you believe this decision was made in error or would like to follow up, you may send another request.</p>
                                    <p>Thank you,<br />MSDS</p>
                                </div>
                            </body>
                        </html>",

                    TextBody = $@"
                        Profile Link Request Denied

                        Hi {email.RecipientName},

                        We regret to inform you that {email.SenderName} has declined your request to link his/her profile to {email.CompanyName}.

                        This means the profile will remain unlinked and no access to personal or employment data will be granted at this time.

                        If you believe this decision was made in error or would like to follow up, you may send another request.

                        Thank you,
                        MSDS"
                    };
                break;


            default:
                throw new InvalidOperationException("Unknown email type.");
        }




        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

        //Central Account 
        await client.AuthenticateAsync("judithlorrenreyes@gmail.com", "evicyxbgnvjypwpy");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }


    public Object ObjectMapper(Object src, Object des)
    {

        Type typeScr = src.GetType();
        PropertyInfo[] propertiesScr = typeScr.GetProperties();

        Type typeDes = des.GetType();
        PropertyInfo[] propertiesDes = typeDes.GetProperties();

        for (int i = 0; i < propertiesScr.Count(); i++)
        {
            for (int j = 0; j < propertiesDes.Count(); j++)
            {
                if (propertiesScr[i].Name.Equals(propertiesDes[j].Name))
                {
                    var valScr = propertiesScr[i].GetValue(src, null);
                    propertiesDes[j].SetValue(des, valScr, null);
                }
            }
        }
        return des;
    }

    public static bool IsObjectEqual(Object obj1, Object obj2)
    {
        bool isEqual = true;

        Type typeO1 = obj1.GetType();
        PropertyInfo[] propertiesO1 = typeO1.GetProperties();

        Type typeO2 = obj2.GetType();
        PropertyInfo[] propertiesO2 = typeO2.GetProperties();

        for (int i = 0; i < propertiesO1.Count(); i++)
        {
            for (int j = 0; j < propertiesO2.Count(); j++)
            {
                if (propertiesO1[i].Name.Equals(propertiesO2[j].Name))
                {
                    var val1 = propertiesO1[i].GetValue(obj1, null)?.ToString(); 
                    var val2 = propertiesO2[j].GetValue(obj2, null)?.ToString();

                    if (val1 != val2 ) {
                        isEqual = false;
                    }
                }
            }
        }

        return isEqual; 
    }
    
    public static string GetMacAddress()
    {
        
        var macAddress = string.Empty;
        
        // Get all network interfaces
        var nics = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (NetworkInterface nic in nics)
        {
            // Get the MAC address
            var address = nic.GetPhysicalAddress();
            var bytes = address.GetAddressBytes();
            
            // Convert the byte array to a string
            macAddress = string.Join(":", bytes.Select(b => b.ToString("X2")));
            if (macAddress.ToString().Length > 1) return macAddress; 
        }

        return macAddress; 
    }

    public static string GetIPAddress()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "No IPv4";
    }

    public static int Get_TimeZone_Id(string timeZoneId)
    {
        const int id = 0; 
        var ctr = 0; 
        foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones())
        {
            ctr++;
            if(timeZone.Id.Equals(timeZoneId)) return ctr;
        }
        return id;
    }

    public static int GetDayNo(string dayName)
    {
        switch (dayName)
        {
            case "Sunday"       : return 1; 
            case "Monday"       : return 2; 
            case "Tuesday"      : return 3; 
            case "Wednesday"    : return 4; 
            case "Thursday"     : return 5; 
            case "Friday"       : return 6; 
            case "Saturday"     : return 7; 
            
            default: return 0;

        }
    }

    public static List<YearsModel?> GetYears(int totYrs)
    {
        var yrs = new List<YearsModel?>();

        var yr = new YearsModel();
        yr.Year = DateTime.Now.Year;
        yr.Name = yr.Year.ToString();
        yrs.Add(yr);

        for (int i = 1; i < totYrs; i++)
        {
            yr = new YearsModel();
            yr.Year = DateTime.Now.Year - i;
            yr.Name = yr.Year.ToString();
            yrs.Add(yr);
        }

        return yrs;
    }

    public static List<MonthsModel?> GetMonths()
    {
        var mos = new List<MonthsModel?>();
        mos.Add(new MonthsModel() { Month=1,    Name = "January",   SName = "Jan" });
        mos.Add(new MonthsModel() { Month=2,    Name = "February",  SName = "Feb" });
        mos.Add(new MonthsModel() { Month=3,    Name = "March",     SName = "Mar" });
        mos.Add(new MonthsModel() { Month=4,    Name = "April",     SName = "Apr" });
        mos.Add(new MonthsModel() { Month=5,    Name = "May",       SName = "May" });
        mos.Add(new MonthsModel() { Month=6,    Name = "June",      SName = "Jun" });
        mos.Add(new MonthsModel() { Month=7,    Name = "July",      SName = "Jul" });
        mos.Add(new MonthsModel() { Month=8,    Name = "August",    SName = "Aug" });
        mos.Add(new MonthsModel() { Month=9,    Name = "September", SName = "Sep" });
        mos.Add(new MonthsModel() { Month=10,   Name = "October",   SName = "Oct" });
        mos.Add(new MonthsModel() { Month=11,   Name = "November",  SName = "Nov" });
        mos.Add(new MonthsModel() { Month=12,   Name = "December",  SName = "Dec" });
        return mos;
    }
    
    public static MonthsModel GetMonth(string moNo)
    {
        var mo = moNo switch
        {
            "02" => new MonthsModel() { Month = 2,  Name = "February",  SName = "Feb" },
            "03" => new MonthsModel() { Month = 3,  Name = "March",     SName = "Mar" },
            "04" => new MonthsModel() { Month = 4,  Name = "April",     SName = "Apr" },
            "05" => new MonthsModel() { Month = 5,  Name = "May",       SName = "May" },
            "06" => new MonthsModel() { Month = 6,  Name = "June",      SName = "Jun" },
            "07" => new MonthsModel() { Month = 7,  Name = "July",      SName = "Jul" },
            "08" => new MonthsModel() { Month = 8,  Name = "August",    SName = "Aug" },
            "09" => new MonthsModel() { Month = 9,  Name = "September", SName = "Sep" },
            "10" => new MonthsModel() { Month = 10, Name = "October",   SName = "Oct" },
            "11" => new MonthsModel() { Month = 11, Name = "November",  SName = "Nov" },
            "12" => new MonthsModel() { Month = 12, Name = "December",  SName = "Dec" },
            _ => new    MonthsModel() { Month = 1,  Name = "January",   SName = "Jan" }
        };
        return mo;
    }
    
    public static List<MyDTRModel?> GetMonthlyDTR(int yr, int mo)
    {
        var myDTR = new List<MyDTRModel?>();

        var date    = new DateTime(yr, mo, 1);
        var nxt_mo  = date.AddMonths(1);

        for (int i = 0; i < 31; i++)
        {
            var xdate = date.AddDays(i);
            if (xdate < nxt_mo)
            {
                var ad = new MyDTRModel() { Date = xdate, DayName=xdate.DayOfWeek.ToString() };
                myDTR.Add(ad);
            }   
        }
        return myDTR; 
    }

    public static string Extract_FieldPrd(string trn)
    {
        var p = string.Empty;
        if (trn.Length < 6) return p;
        var fldName = trn.Substring(4, 2);
        
        var fld = fldName switch
        {
            "02" => "P2",
            "03" => "P3",
            "04" => "P4",
            "05" => "P5",
            _ => "P1"
        };
        return fld;
    }
    
    public static async Task<string> GenerateTrnNumber(string mode, IParaDataAccess _para, string schema, string conn)
    {
        // Format: "XXXYY-MM9999"
        var yy           = string.Empty;
        var mm           = string.Empty;
        var ctrStr       = string.Empty;
        var trnNumber    = string.Empty;


        // 1. Retrieve the last saved TRN record
        ParaModel? para = await _para._02(mode, schema, conn);

        int mo = Convert.ToInt32(para?.Month);

        if (para != null)
        {
            yy          = para?.Year!;
            mm          = (mo).ToString("D2");
            ctrStr      = para?.CtrName!.ToString("D4")!;
            trnNumber   = $"{mode.ToUpper()}{yy}-{mm}{ctrStr}";
        }

     

        return trnNumber;
    }

    public static double Compute_PayCal_TotalHrs_perEmpamsId(int empmasId, List<TbltranModel?>? tbltrans, List<DutyrenderedModel?>? rdlst, SettingsModel s )
    {
        
        var hrs = 0.00; 
        foreach (var tran in tbltrans??[])
        {
            var acctNumber = tran?.AcctNumber??"---";
            var rds = rdlst?.Where(x => x?.AcctNumber == acctNumber).ToList();
            if (rds == null) continue;
            if (rds.Count < 1) continue;
            if (tran?.EmpmasId != empmasId) continue;
            var conv = tran.RateTypeId switch
            {
                2 => s.Daytohours,
                3 => s.SemiMonthtodays  * s.Daytohours,
                4 => s.Monthtodays      * s.Daytohours,
                5 => s.Semiannualtodays * s.Daytohours,
                6 => s.Yeartodays       * s.Daytohours,
                _ => 1
            };

            var res = (tran?.Qty ?? 0) * (conv);  
            if (tran?.AcctNumber == "E000") hrs -= res;
            else hrs += res;
        }

        return hrs; 
    }
    
    public static double Compute_PayCal_Footer_TotalHrs(List<TbltranModel?>? tbltrans, List<DutyrenderedModel?>? rdlst, SettingsModel s )
    {
        
        var hrs = 0.00; 
        foreach (var tran in tbltrans??[])
        {
            var acctNumber = tran?.AcctNumber??"---";
            var rds = rdlst?.Where(x => x?.AcctNumber == acctNumber).ToList();
            if (rds == null) continue;
            if (rds.Count < 1) continue;
            
            var conv = tran!.RateTypeId switch
            {
                2 => s.Daytohours,
                3 => s.SemiMonthtodays  * s.Daytohours,
                4 => s.Monthtodays      * s.Daytohours,
                5 => s.Semiannualtodays * s.Daytohours,
                6 => s.Yeartodays       * s.Daytohours,
                _ => 1
            };

            var res = (tran?.Qty ?? 0) * (conv);  
            if (tran?.AcctNumber == "E000") hrs -= res;
            else hrs += res;
            
        }
        
        return hrs;
    }

    public static Model605 Compute_NetPay(List<TmptbltranemplistModel>? els, List<TbltranModel?>? tbltrans, TmptbltranemplistModel?  footerTotal)
    {
        Model605 m605 = new();
        if (els == null) return m605;
        if (tbltrans == null) return m605;
        
        //---  Compute for the Amount ------------------------------------------------------------------------
        foreach (var tran in tbltrans ?? [])
        {
            tran!.Amount = 0;
            if ((tran.Qty   != 0 || tran?.Qty  != null) && 
                (tran?.Rate != 0 || tran?.Rate != null))
            {
                tran!.Amount = tran.Qty * tran.Rate;
            } 
            
            
        }
        //=================================================================================================

        //---  Compute NetPay [ NP = Earnings - Deductions ] for every Employee ---------------------------
        foreach (var el in els ?? [])
        {
            var empmasId = el?.EmpmasId;
            var trans = tbltrans?.Where(t=>t?.EmpmasId==empmasId).ToList(); 
            var earnings = trans?.Where(t=> (t?.AcctNumber)?[..1] =="E")
                                        .Sum(t=> t?.Amount??0);
            var deductions = trans?.Where(t=> (t?.AcctNumber)?[..1] =="D")
                .Sum(t=> t?.Amount??0);
            
            el!.TotEarnings      = earnings??0;
            el!.TotDeductions    = deductions??0;
            el!.NetPay           = el.TotEarnings - el.TotDeductions;
            
            
        }
        
        //=================================================================================================

        //---  Footer Total  ------------------------------------------------------------------------------
        var totEarnings = tbltrans?.Where(t=> (t?.AcctNumber)?[..1] =="E")
            .Sum(t=> t?.Amount??0);
        var totDeductions = tbltrans?.Where(t=> (t?.AcctNumber)?[..1] =="D")
            .Sum(t=> t?.Amount??0);
        
        footerTotal!.TotEarnings     = totEarnings??0;
        footerTotal!.TotDeductions   = totDeductions??0;
        footerTotal!.NetPay          = totEarnings??0 + totDeductions??0; 
        //=================================================================================================
        
        m605.TmptbltranEmpLists = els!; 
        m605.FooterTotal     = footerTotal;
        return m605; 
    }

    public static Task<Model605?> Compute_NoOfHrs(List<TmptbltranemplistModel>? els, 
                                       List<TbltranModel?>? tbltrans, 
                                       TmptbltranemplistModel?  footerTotal, 
                                       List<DutyrenderedModel?>? rdlst, 
                                       SettingsModel s )
    {
        Model605? m605 = new(); 
        
        //--- Individual duty ------------------------------------------------------------------------------------------
        foreach (var el in els ?? [])
        {
            var hrs = 0.00; 
            
            foreach (var tran in tbltrans??[])
            {
                var acctNumber = tran?.AcctNumber??"---";
                var rds = rdlst?.Where(x => x?.AcctNumber == acctNumber).ToList();
                if (rds == null) continue;
                if (rds.Count < 1) continue;
                if (tran?.EmpmasId != el.EmpmasId) continue;
                var conv = tran.RateTypeId switch
                {
                    2 => s.Daytohours,
                    3 => s.SemiMonthtodays  * s.Daytohours,
                    4 => s.Monthtodays      * s.Daytohours,
                    5 => s.Semiannualtodays * s.Daytohours,
                    6 => s.Yeartodays       * s.Daytohours,
                    _ => 1
                };

                var res = (tran?.Qty ?? 0) * (conv);  
                if (tran?.AcctNumber == "E000") hrs -= res;
                else hrs += res;
            }
            
            
            el!.TotHours = hrs; 
        }
        //==============================================================================================================
        var totHrs = els?.Sum(t=> t!.TotHours);
        footerTotal!.TotHours = totHrs??0;

        m605.TmptbltranEmpLists = []; 
        if(m605.TmptbltranEmpLists!=null)  m605.TmptbltranEmpLists = els!; 
        m605.FooterTotal = footerTotal;
        
        return Task.FromResult(m605)!;
    }
}

