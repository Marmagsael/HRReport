# frx2trdp

Tools for porting MBPay (FoxPro) reports to Telerik `.trdp` files for HRMvc `PayrollReport`.

```bash
dotnet build tools/frx2trdp/frx2trdp.csproj
X="dotnet tools/frx2trdp/bin/Debug/net8.0/frx2trdp.dll"

$X dump    "MBPay 2026.10.27.zip" MBPay/reports/rptpyr_04b.frx          # list labels, fields, lines, bands
$X convert "MBPay 2026.10.27.zip" MBPay/reports/rptpyr_04b.frx \
           tools/frx2trdp/out/K_PayrollRegister.trdp tools/frx2trdp/configs/rptpyr_04b.json
PNG_PAGES=1,2 $X register HRMvc/Reports/_03Payroll/K_PayrollRegister.trdp tools/frx2trdp/out/reg.pdf 260502
$X regcheck                                                            # DA vs plain SQL, every period
```

`render`/`regcheck` read `HRMvc/appsettings.json` (`MySqlConn`) and use the `pay`/`secpis` schemas. Point it only at the local test DB.

## Converter config (`configs/*.json`)

| Key | Meaning |
|---|---|
| `Name` | Telerik report name |
| `Detail` | FoxPro cursor of the detail rows. `Dx2.e1` becomes `Fields.E1` (first letter upper case) |
| `Header`, `HeaderPrefix` | One-row header cursor copied onto every data row. `rh.e1` becomes `Fields.HE1` |
| `Params` | Expressions that become report parameters, e.g. `"Fileloc.coname": "CoName"` |
| `Constants` | Expressions replaced by a literal, e.g. `"rh.l1": "1"` |
| `Replace` | `"<expr>@<hpos>": "<expr>"` to fix a field placed wrong in the FRX |
| `ForceSum` | Footer fields that the FRX forgot to total |
| `PageHeaderToGroup` | Put the page header in the group header (printed on every page) so it can use `Fields` |
| `Exprs` | Whole FoxPro expression (spaces and case ignored) -> Telerik expression, for logic done in the DA (e.g. `mdy(...)` -> `Fields.Period`) |
| `FontMap` | Printer font -> PDF font, e.g. `"Draft 20cpi": "Arial Narrow"` |
| `NoGrow` | Stretch fields stay one line. Telerik pushes every item below a grown text down; FoxPro only overlaps |
| `Paper`, `Landscape` | Override the FRX page (dot-matrix reports have no `PAPERSIZE`). The report width is cut to fit the page |
| `Rename` | `"alias.field": "Name"` gives a field another Telerik name, e.g. `"Curpyr.e001_1": "E001H"` |
| `Labels` | Label text prefix -> expression, e.g. a hard-coded company name -> `=Fields.HDtl` |
| `PrintWhen` | Field expression -> its PRINT WHEN, to fix a wrong one in the FRX |

PRINT WHEN is not mapped to `Visible`: Telerik pulls the items to the right of a hidden item to the left. A text gets `=IIf(cond, value, Null)` and a line or box a transparent colour instead.

Other reports: `payslip` / `payslipcheck`, `peracct` / `peracctcheck`, `advise` / `cash` / `charge` / `bankcheck` (same arguments as `register` / `regcheck`), and `textfile <out.xlsx> <yymmpp> <motherAcct>` for the bank upload sheet (compiles `HRMvc/.../Class/BankTextFileSheet.cs` in).

A FoxPro memory variable (`lcFinPeriod`) listed in `Params` becomes a report parameter. The summary band becomes the report footer, where a `0` counter field becomes `Count(1)`.

## FRX notes

- Units are 1/10000 inch. Each band is followed by a 0.19271in separator, so a band starts at the sum of the earlier bands' height + separator.
- `OBJTYPE` 5 label, 6 line, 7 box, 8 field, 9 band. Band `OBJCODE` 1 page header, 3 group header, 4 detail, 5 group footer, 7 page footer. A group band with `PAGEBREAK=T` starts each group on a new page.
- The report record's `EXPR` holds `ORIENTATION=1` (landscape) and `PAPERSIZE` (5 = Legal).
- MBPay amounts are `double(10,4)` but report cursors are `n(12,2)`. Round each amount as `decimal`, half away from zero, before totals.
