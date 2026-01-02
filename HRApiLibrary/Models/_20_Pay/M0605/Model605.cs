using Microsoft.IdentityModel.Tokens;

namespace HRApiLibrary.Models._20_Pay.M0605;

public class Model605
{
    public string?                          Paydb                   { get; set; } = string.Empty;
    public string?                          Pisdb                   { get; set; } = string.Empty;
    public string?                          Conn                    { get; set; } = string.Empty;
    public int                              UserId                  { get; set; } = 0; 
    
    public PaymainhdrModel?                 Paymainhdr              { get; set; } = new();
    public List<PaymainhdrModel?>?          Paymainhdrs             { get; set; } = [];
    public List<TbltranModel?>?             Tmptbltrans             { get; set; } = [];
    public List<TmptbltranemplistModel?>?   TmptbltranEmpLists      { get; set; } = [];
    public PayrollgrpModel?                 Payrollgrp              { get; set; } = new();
    
    
    
    
    
    

    public List<TbltranModel?>?             Tbltrans                { get; set; } = []; 
    public TbltranModel?                    Tbltran                 { get; set; } = new(); 
    public List<PaymainvisacctModel?>?      Paymainvisaccts         { get; set; } = [];
    public List<CoaModel?>?                 PrdAccts                { get; set; } = [];
    public string?                          Msg                     { get; set; } = string.Empty;
    public string?                          ProcStatus              { get; set; } = string.Empty;
                                                                    
    public string?                          Trn                     { get; set; } = string.Empty;
    public int                              PayrollgrpId            { get; set; } = 0; 
    public PaymaindtlModel?                 Paymaindtl              { get; set; } = new();
    public List<PaymaindtlModel?>?          Paymaindtls             { get; set; } = [];
    public PaytranModel?                    Paytran                 { get; set; } = new();
    public List<PaytranModel?>?             Paytrans                { get; set; } = [];
    
    public TmptbltranemplistModel?         FooterTotal             { get; set; } = new();
    
    
    
}