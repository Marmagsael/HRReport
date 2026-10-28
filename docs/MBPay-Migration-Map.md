# MBPay (FoxPro) → HRSys (.NET) Migration Map

Source: `MBPay 2026.10.27.zip` (FoxPro 9, not committed — see `.gitignore`).
Target: `HRApiLibrary` (data access / logic) + `HRMvc/Applications/_03Payroll` and `PayrollReport` (UI).

## Key finding: same database

MBPay does **not** keep payroll data in DBF files. `PROGS/conn.prg` connects via ODBC to MySQL and uses the
same schemas as HRSys (`pay`, `pis`/`secpis`, `emp`, `ams`, plus `archpay`/`archpis` for archives).
The local DBFs only hold config (`link.dbf` = connection info, `fileloc.dbf`, BIR field mappings in `DataBIR/`).

So integration is **feature porting, not data migration**. Tables used by MBPay that HRSys does not reference yet
are listed at the end — some may have been renamed in HRSys (e.g. `ssstbl` → `matrixSSS`), to be verified per feature.

## Status legend

- ✅ **Has page** — an HRSys page with a matching name exists. Logic still needs to be checked against FoxPro.
- 🟡 **Partial** — related page/table exists but the FoxPro feature does more.
- ❌ **Missing** — nothing equivalent found.

## System

| FoxPro menu | FoxPro form | HRSys | Status |
|---|---|---|---|
| User Administration | FRMUSERMGR | `_0506_UserManagement` | ✅ |
| Change Password | FRMCHANGEPASS | login module | 🟡 verify |
| Log-out / Exit | LOGIN | — | ✅ (web login) |
| Menu/access per user | `pay.sysmenu`, `pay.useraccess`, `pay.acctaccess`, `pay.reportaccess` | — | ❌ |

## Setup

| FoxPro menu | FoxPro form | HRSys | Status |
|---|---|---|---|
| Employee Rate | FRMEMPRATES | `_0505_EmployeeRates` | ✅ |
| Surety Bond Quota | FRMSBQUOTA | — | ❌ |
| SSS Table | FRMSSSTABLE | `_0502_05_SSS` | ✅ |
| Phil-Health Table | FRMPHICTABLE | `_0502_06_PHIC` | ✅ |
| Rate Over Basic Pay | FRMOTSETUP | `_0504_00_PayrollgrpRates` | 🟡 |
| Salary Grade | salaryGrade | — | ❌ |
| Accounts Entry | FRMCHAROFACCT | `_0502_ChartOfAccounts` | ✅ |
| Account Mapping | AcctMapping | — | ❌ |
| Payroll Content | PAYCONTENT | — | ❌ |
| Deduction Order Setup | DEDUCTIONORDER | — | ❌ |
| Annual Tax Matrix | TAX_MATRIX | `_0502_04_Tax` | ✅ |
| Semi-Monthly Tax Matrix | FRMSEMIMONTHLYTAX | `_0502_04_Tax` | 🟡 verify |
| Account Code Entry | FRMACCTCODE | Chart of Accounts | 🟡 |
| Minimum Rate Setup | FRMMINIMUMSETUP | — | ❌ |
| Status visible in payroll | emp_status_visible_to_pay | — | ❌ |

## Transaction

| FoxPro menu | FoxPro form | HRSys | Status |
|---|---|---|---|
| Earnings (per employee) | FRMEARNFINAL | `_0602_EarningsEntry` | ✅ |
| Earnings (group) | FRMEARNINGS | `_0602_12_GroupEarnings` | ✅ |
| Fixed Earnings / Grp Fixed Earnings | fixed_earnings, fixed_earnings_grp | `_0602_11_FixedEarnings` | ✅ |
| 13th Mo Salary deduction setter | frm13thMonth_ded_Setter | — | ❌ |
| Deductions (per employee) | FRMCANCELDEDN | `_0604_DeductionEntry` | ✅ |
| Deductions (group) | FRMDEDUCTIONS | `_0604_DeductionEntry` | 🟡 |
| Partial Deductions | FRMPARTIALDED | — | ❌ |
| Mandatory Deduction | ded_mandatory | `DedmandatoryDataAccess` | 🟡 |
| Hospitalization | ded_hospitalization | — | ❌ |
| **Payroll** | **PAYROLL4** (largest form, ~400KB) | `_0605/_0606_PayrollProcess`, `DA605DataAccess` | 🟡 **core — see Phase 1** |
| Payroll variants | payroll4_manual, _negativepay, _below500, _40_prcnt_salary, _calculator, _13mogenerator, retro_pay | — | ❌ |
| Payroll (custom) | PAYROLLX | — | ❌ |
| BIR Fields Mapping / Data Migration / Excel Upload | bir_fields_mapping, bir_empmas_migration, bir_excelUploading | — | ❌ |

## Reports

| FoxPro menu | FoxPro form | HRSys | Status |
|---|---|---|---|
| Deductions Summary | FRMDEDSUM | `_1152_DeductionSummary` | ✅ |
| Earnings Summary | FRMEARNSUM | `_1102_EarningsSummary` | ✅ |
| Deductions History | FRMDEDSUMEMP | `_1154_DeductionHistory` | ✅ |
| Earnings History | FRMEARNSUMEMP | `_1103_EarningsHistory` | ✅ |
| Monthly Deductions | FRMDEDMONTHLY | `_1155_MonthlyDeductionSummary` | ✅ |
| Monthly Earnings | FRMEARNMONTHLY | `_1104_MonthlyEarningsReport` | ✅ |
| Loan Summary | loans_management | `_0603_LoansManagement` | 🟡 |
| Deduction Summary | rptdedsummary | `_1153_ConsolidatedDeductionSummary` | 🟡 verify |
| Retirement / Grp Retirement | rptretirement, rptretirement_grp | `_1307`, `_1308` | ✅ |
| Bank Advice | FRMBANKADVISE | `_1205_BankAdvise`, `_0703` | ✅ |
| Bank Charge | FRMBANKCHARGE | `_1204_BankCharge`, `_0704` | ✅ |
| Bank Text File | FRMBANKTEXTFILE | — | ❌ |
| Payroll Register | FRMPAYROLLREGISTER | `_1203_PayrollRegister`, `_0701` | ✅ |
| Payroll Register Gov | FRMPAYROLLREGISTER_GOV | — | ❌ |
| Payroll Register (Per Account) [+Gov] | FRMPAYROLLREGISTERPERACCT[_GOV] | — | ❌ |
| Payroll Register (Masterlist) | FRMREGISTERMASTER | — | ❌ |
| Payslip | FRMPS | `_1202_Payslip`, `_0702` | ✅ |
| Payslip Gov / Per Employee [+Gov] | FRMPS_GOV, FRMPSPEREMPLOYEE[_GOV] | — | ❌ |
| Revised PS: Payroll Summary | payroll_register4 | `_1206_PayrollSummaryPerGroup` | 🟡 |
| Revised PS: Register (2 Liner) | payroll_register4V2 | — | ❌ |
| Customized: Dressed Payslip / Register / Bank Advice / Text File | frmps_deduction_dressing, dressed_* | — | ❌ |
| 13th Month Detailed / Summary / Register / Bank Advice | FRM13THMONTHDETAILED, FRM13THMONTHSUMMARY, FRM132REGISTER, FRMBA13 | `_1209`, `_1207`, `_1212`, `_1208` | ✅ |
| 13th Month per Client / per Employee variants (5) | FRM13DTLPERCLIENT, FRM13DTLPEREMP, FRM13SUMMARYCLIENT, FRM13SUMMARYEMP, FRM13REGISTER | — | ❌ |
| 13th Month Payslip | — | `_1213_13thMonthPayslip` | ✅ (HRSys only) |
| Pag-IBIG Contribution Summary / Remittance | FRMPAGIBIGCONTRIBUTION, FRMPAGIBIGCONTREMIT | `_1254_PagibigContribution` | ✅ |
| Pag-IBIG Remittance (per client) | FRMPAGIBIGPERCLIENT | — | ❌ |
| Pag-IBIG / HDMF Loan Remittance | FRMPAGIBIGLOANREMIT, FRMHDMFLOANREMIT | `_1255`, `_1256_CalamityLoanRemittance` | 🟡 |
| PhilHealth Contribution | FRMPHILHEALTHCONTRIBUTION | `_1257_PHICContribution` | ✅ |
| PhilHealth Remittance (Text File) | FRMPHICQUARTERLY | — | ❌ |
| PhilHealth ER2 | PHIC_ER2 | — | ❌ |
| PhilHealth Monthly [per client] | FRMPHMO, FRMPHMOPERCLIENT | — | ❌ |
| SSS R1-A | SSS_R1A | — | ❌ |
| SSS Loan Remittance | FRMSSSLOANREMITTANCE | `_1253_SSSLoanRemittance` | ✅ |
| SSS Contribution Remittance | FRMSSSCONTREMIT | `_1252_SSSContribution` | ✅ |
| SSS Contribution (Adjusted) / Per Client | FRMSSSCONTREMIT2, FRMSSSCONTREMITPERCLIENT | — | ❌ |
| SSS Payment Module | paymentsss | — | ❌ |
| Alphalist / Gov / Per Employee | FRMALPHALIST2, FRMALPHALIST_GOV, FRMALPHALISTSINGLE2 | `_1302_TaxAnnualization` | 🟡 |
| Account Total | FRMACCOUNTTOTALS | `_1309`, `_1313_AccountTotal` | ✅ |
| BIR Form 1700 | FRMFORM1700 | — | ❌ |
| BIR Form 2316 | FRMFORM2316 | — | ❌ |
| Grand Total | FRMTOTALDED | `_1312_GrandTotal` | ✅ |
| Summary Per Client | FRMSUMPERCLIENT | — | ❌ |
| Withholding Tax Compliance Report | FRMWTCR | `_1305_Compliance`, `_0705` | 🟡 verify |
| Balance Report (current / by date) | FRMBLANCEREP, FRMBLANCEREP2 | — | ❌ |
| COE (with compensation) | coe | — | ❌ |
| Quit Claims | FRMAR | `_1304_QuitClaims` | ✅ |

## Account Configuration

| FoxPro menu | FoxPro form | HRSys | Status |
|---|---|---|---|
| Emp. Status Included in Payroll | payroll_employee_status_inclusion | — | ❌ |
| Meal Allowance | MEALALLOWANCECONFIG (+ `fmealallowance.prg`) | — | ❌ |
| Uniform | UNIFORMCONFIG (+ `funiform.prg`) | — | ❌ |
| Provision for Retirement | RETIREMENTCONFIG (+ `fprovisionforretirement.prg`) | — | ❌ |
| OT Account Setup | ACCTOTCONFIG (+ `fot.prg`) | `_0501_DutyRenderAcctMgt` | 🟡 |
| Minimum Client Rate | MINRATESETUP | — | ❌ |
| Deduction Dressing Mgt | deduction_dressing_mgt | — | ❌ |
| ECOLA / CTPA / SEA Management | ecola_mgt, ctpamgt, seamgt (+ `fecola`, `fctpa`, `fsea.prg`) | — | ❌ |
| Record Archiver | RecordArchiever, archieve_transaction | — | ❌ |
| Tax Setter | tax_mapper (+ `fclassifytax.prg`) | — | ❌ |
| Employee Account Exemption | deduction_exemption_management | — | ❌ |
| 13th Month Settings | 13mosettings | — | ❌ |

## Computation logic (`PROGS/*.prg`) vs HRSys

| FoxPro | Purpose | HRSys | Status |
|---|---|---|---|
| fcomputebasicpay | Basic pay | `DA605DataAccess._01EmpByPayrollgrpRate` etc. | 🟡 compare |
| fsssprem | SSS premium | `DA605._01_PremSSS` | 🟡 compare |
| fphicprem1 | PhilHealth premium | `DA605._01_PremPHIC` | 🟡 compare |
| fpagibigprem | Pag-IBIG premium | `DA605._01_PremPagibig` | 🟡 compare |
| ftax, fclassifytax | Withholding tax | `DA605._01_Tax` | 🟡 compare |
| calcloans | Loan amortization | `DA605._01_Loans` | 🟡 compare |
| minimumnet | Minimum net pay rule | — | ❌ |
| fot | Overtime | — | ❌ |
| fallowance, fmealallowance, funiform, fecola, fctpa, fsea, fincentiveleave, fcashbond, fprovisionforretirement | Allowances / special accounts | — | ❌ |
| loadgrpdedn, loadschedearn | Load scheduled earnings/deductions | `_01_FE_to_Tmptbltran`, `_01_FEG_to_Tmptbltran` | 🟡 |
| fpayrollfooter, fzeroingflds | Payroll totals / reset | — | verify |

Note: a lot of FoxPro logic lives inside form methods (`.SCT`), especially `payroll4.SCT`, not only in `PROGS`.

## Tables used by MBPay but not referenced in HRSys code

`pay`: 13thmogrp, 13thmosched, 13thmosettings, acctaccess, acctconfig, allowanceperday, audit_payroll_lock,
coaconfig, dedmandatory_trans, deductiondressing, empdedexemption, empstatpisval, hospitalization* (acct, dtl, grp,
hdr, savings, status), loansacctaccess, loansref, loansref_rep_access, mealallowopt, medtbl,
pisval_empdeploymenthist, ratesamt, reportaccess, reportlist, retirementopt, semimonthlytax, ssstbl, sysmenu,
tblatexemption, tblbatch, tblotherdedref, tblotherdeductions, tblotherearnings, tbltranarchieved,
tbltrandeployment, templock, undeddeduction, uniformopt, useraccess, plus `tmp*` work tables.

`pis`: clbranch, client_rate_settings, client_rateatrail, client_rateatrail_empmas, empmasarchieved,
empstatforarchive, userclientaccess.

## Proposed phases

1. **Payroll computation parity**: compare `payroll4.SCT` + `PROGS` with `DA605DataAccess`, run both on the same
   period, and fix differences. Add tests in `HRApiLibrary.Tests`.
2. **Missing account configs that affect computation**: OT, minimum net, ECOLA/CTPA/SEA, meal/uniform/retirement,
   deduction order, employee status inclusion, tax setter, 13th month settings.
3. **Missing transactions**: partial deductions, hospitalization, retro pay, payroll variants, BIR upload.
4. **Government reports and files**: SSS R1-A, PhilHealth ER2/text file, bank text file, Alphalist, BIR 1700/2316.
5. **Remaining report variants**: Gov / per client / per employee / dressed / 2-liner, COE, balance report.
6. **Access control and archiving**: sysmenu/useraccess equivalents, record archiver.
