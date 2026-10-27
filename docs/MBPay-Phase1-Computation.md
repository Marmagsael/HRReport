# Phase 1: Payroll Computation, FoxPro vs HRSys

Compared: `MBPay/forms/payroll4.SCT` (methods `compute_basic`, `compute_d00tax`, `compute_d01sss`,
`compute_d04phic`, `compute_d03pagibig`, `compute_ot`, `compute_absent`, `calc_loans`, `minimumnet`,
`check_negative_pay`) and `MBPay/PROGS/*.prg` against `HRApiLibrary/DataAccess/_20_Pay/DA0605/DA605DataAccess.cs`.

TRN format (from `MsdsDataAccess.Extract_FieldPrd`): chars 1–4 = year+month, chars 5–6 = period. So `left(trn,4)` = same month.

## Findings

| # | Area | FoxPro (payroll4) | HRSys (DA605) | Gap | Severity |
|---|---|---|---|---|---|
| 1 | Payroll process UI | PAYROLL4 form runs the whole process | `/03/0605` page has no content, `/03/0606` is a placeholder; nothing in any branch calls `DA605DataAccess` | The .NET payroll process cannot be run by users | Critical |
| 2 | Basic pay | rateid 1 (monthly) = rate/2; 2 (daily) = rate × days; 3 (semi-monthly) = rate; employee rate 0 → client rate × days; monthly/semi-monthly paid even with 0 days | No rule in backend; `Amount` is whatever the UI sends to `_03TmpTbltran` | Basic pay rules missing | High |
| 3 | SSS basis | Bracket uses month-to-date earnings (already saved this month + current); EE = bracket EE − SSS already deducted this month | Bracket uses current TRN earnings only, then subtracts SSS already deducted this month | 2nd cutoff under-deducts SSS | High |
| 4 | SSS max | `isMaxSSS = 1` → highest bracket regardless of earnings | Not supported | Missing | Medium |
| 5 | SSS ER / ECC | Computes ER share and stores it (used by remittance reports) | EE only | ER share missing | Medium |
| 6 | SSS non-member | `iswithSSS != 1` → skip | Earnings forced to 0 but still joined to matrix; a bracket starting at 0 would deduct minimum EE | Verify `matrixSSS` lowest bracket | Medium |
| 7 | PhilHealth formula | EE = fixed + (earnings × percent / 100) / 2 | EE = ee + earnings × percent / 100 (no ÷ 2) | Double the percentage part, unless `matrixPHIC.percent` is already halved — verify | High |
| 8 | PhilHealth basis | Month-to-date earnings, minus PHIC already deducted this month; skip if earnings = 0 | Current TRN earnings only | Same as #3 | High |
| 9 | PhilHealth non-member | `iswithPHIC != 1` → skip | `if(wphic!=1, 0, …) - deducted` → negative amount if there were earlier deductions | Wrong sign | Low |
| 10 | Pag-IBIG SQL | n/a | `left join pagibig p on t.t.Amount between …` — MySQL error “Unknown column t.t.Amount” | D004 computation fails at runtime | Critical |
| 11 | Pag-IBIG rules | `iswithPagibig` check; month-to-date earnings; EE − already deducted | No wPagibig check; current TRN only; `if(deducted > earnings, 0, …)` compares premium to earnings | Logic differs | High |
| 12 | Tax basis | Taxable = current-period taxable earnings − SSS − PHIC − Pag-IBIG (current period) | `(trn = @Trn or Status = 'P') and AcctNumber in coaTax` → includes every posted record of the employee (all history); premiums not deducted | Tax heavily overstated | Critical |
| 13 | Tax details | `excess < 1` → 0; blank tax code → "S"; only `istaxable = 1` | No floor on excess; uses `deprecsettings.wTax` | Minor differences | Medium |
| 14 | Computation order | Premiums first, then tax (tax needs D002–D004) | Each account computed separately by caller; order not enforced | Tax can run before premiums | Medium |
| 15 | Loans re-run | Skips a loan already deducted this period (same amount saved) | Check uses `LEFT(trn,4) = LEFT(@Trn,6)` (never true) and the result is unused | No duplicate protection | Medium |
| 16 | Loan closing | n/a | After posting: `Status = 'I' where Amount < 1` — should be `Balance < 1` | Paid-off loans stay Active | Medium |
| 17 | Posting | Saves all rows, including negative ones (absent is a negative earning) | `insert into TblTran … where Amount > 0` | Negative entries (absences, adjustments) are dropped | High |
| 18 | `_01Acct_per_Trn` | n/a | SQL has 3 values for 2 columns and missing parameters | Would fail if called (currently unused) | Low |
| 19 | Overtime | Hourly = daily rate / 8 × OT rate × hours; non-daily uses salary-grade daily rate; ROT can use fixed amount from `ratesamt` | Not implemented | Missing | High |
| 20 | Absent | −(hours × daily rate / 8), daily rate from salary grade or client rate | Not implemented | Missing | High |
| 21 | Minimum net / lock checks | Net < 1000 → zero out unlocked deductions and log to `undeddeduction`; block lock on negative net; warn if net 0–499 | Not implemented | Missing | High |
| 22 | Other earnings | ECOLA, CTPA, SEA, meal allowance, free uniform, incentive leave, provision for retirement, hospitalization, retro pay, 13th month | Not implemented (fixed earnings and mandatory deductions exist) | Missing | High |

## Decisions needed (FoxPro quirks)

| # | Question | FoxPro actual behavior | Recommendation |
|---|---|---|---|
| D1 | Negative premiums | `IIF(mpremee<1, mpremee=0, …)` is a no-op in FoxPro, so negative premiums (refunds) are not clamped | Clamp to 0, which is what the code was meant to do |
| D2 | Minimum net: government deductions | The filter `acct != 'D001' OR acct != 'D002' …` is always true, so tax/SSS/PHIC/Pag-IBIG can also be zeroed (unless the account is locked) | Never zero D001–D004 |
| D3 | Minimum net threshold | Zeroing triggers below 1000; lock warning below 500 | Make both a setting |
| D4 | Tax method | Per-period lookup in semi-monthly table, no annualization | Keep for parity now; annualization is a separate feature (Alphalist/2316) |
| D5 | Minimum net: how many deductions to zero | `LOOP` in the SCAN continues instead of exiting, so every unlocked deduction is zeroed once net < 1000 | Zero from highest SortDed down only until net reaches 500 (implemented) |

## Proposed implementation

1. Add a pure C# `PayrollCalculator` in `HRApiLibrary` (no SQL): basic pay, OT, absent, SSS, PHIC, Pag-IBIG, tax, loans, minimum net.
   Inputs: employee rate info, current lines, month-to-date saved lines, matrices. Output: computed lines + ER shares.
2. Unit tests in `HRApiLibrary.Tests` using the FoxPro rules above as the spec.
3. Replace the SQL formulas in `DA605DataAccess` with: load data → `PayrollCalculator` → save. Fix #10, #12, #16, #17.
4. Build the payroll process UI on `/03/0605`.
5. Parity check: run one real period in both FoxPro and HRSys and compare net pay per employee.

## Status

| # | Status | Note |
|---|---|---|
| 1 | Open | Next: payroll process UI on `/03/0605` |
| 2 | In progress | `PayrollCalculator.BasicPay` done and tested; wiring waits for the UI |
| 3 | Fixed | Month-to-date basis in `_01_Premium` |
| 4 | In progress | Calculator supports `IsMaxSss`; no column for it in HRSys yet |
| 5 | In progress | Calculator returns ER; no table to store it yet (needed by remittance reports) |
| 6 | Fixed | Non-members get 0 |
| 7 | Fixed | ÷ 2 applied. Verify that `matrixPHIC.percent` holds the total rate (e.g. 5), not the EE share |
| 8 | Fixed | Month-to-date basis |
| 9 | Fixed | Never negative |
| 10 | Fixed | SQL replaced |
| 11 | Fixed | wPagibig check, month-to-date basis |
| 12 | Fixed | Current TRN only, minus SSS/PHIC/Pag-IBIG |
| 13 | Fixed | Excess < 1 ignored, never negative |
| 14 | Fixed | `_01_Tax` computes missing premiums first |
| 15 | Fixed | Period check uses `LEFT(trn,6)`; already-deducted loans skipped |
| 16 | Fixed | Closes on `Balance < 1` |
| 17 | Fixed | Posts `Amount != 0` |
| 18 | Open | Unused method, left as is |
| 19 | In progress | `PayrollCalculator.Overtime` done and tested |
| 20 | In progress | `PayrollCalculator.Absent` done and tested |
| 21 | In progress | `ApplyMinimumNet` and `CheckNet` done and tested |
| 22 | Open | Phase 2 |
