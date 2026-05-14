# Attendance Module — Business Rules Reference
---

## Overview

The attendance module builds a calendar view per employee per month.
Each day is evaluated and tagged with a status based on the following rules.

**Evaluation flow per day:**
```
Rest Day → Leave → Duty Type → Attendance Type → Date Scenario
```
---

## 1. Data Sources Per Day

| Source | What it provides |
|--------|-----------------|
| `LoadTemplate()` | Expected schedule: time in, duration, duty type, attendance type |
| `LoadPunches()` | Actual punches: first in, last out, total worked hours, all logs |
| `LoadLeave()` | Approved leave for the day: leave code, duration, start time |

**Priority rule for schedule info:**
- If employee **has punches** → use schedule from punches record
- If employee **has no punches** → use schedule from template

---

## 2. Duty Types

| Code | Name | Behavior |
|------|------|----------|
| `R` | Regular Day | Full computation — late, undertime, present/absent |
| `RN` | Regular Normal | Present check only — this day is NOT mandatory |
| `RD` | Rest Day | No computation, no tagging — show `RD` only |

---

## 3. Attendance Types

Applies to **Duty Type `R` only**.

| Code | Name | Late/Undertime Basis |
|------|------|----------------------|
| `FIXEDATTENDANCE` | Fixed | Based on expected schedule (time in / time out) |
| `FLEXIBLEATTENDANCE` | Flexible | Based on total expected working hours only |
| `EXEMPTATTENDANCE` | Exempt | No punch required — always PRESENT, no late/undertime |

---

## 4. Punch Rules

- Punches always come in **pairs**: IN → OUT → IN → OUT
- **At least 1 punch = PRESENT**
- **No punch at all = ABSENT**

### Computing Total Hours Worked
Sum all punch pairs 
```
Total Worked =  (Punch Out - Punch In) for each pair

```


---

## 5. Shift Boundaries

### Shift Start and End
```
shiftStart = date + expTimeIn
shiftEnd   = shiftStart + expWorkDuration
```

### Half Boundaries (used for half day leave computation)
```
breakTime       = 1 hr (fixed lunch break)
halfDuration    = (shiftEnd - shiftStart - breakTime) / 2  ← deduct lunch first!
firstHalfEnd    = shiftStart + halfDuration
secondHalfStart = firstHalfEnd + breakTime

First Half  : shiftStart  → firstHalfEnd
Lunch Break : firstHalfEnd → secondHalfStart
Second Half : secondHalfStart → shiftEnd
```

**Example (9 hr shift, 08:00 AM start):**
```
shiftStart   = 08:00 AM
shiftEnd     = 08:00 AM + 9 hrs = 05:00 PM
breakTime    = 1 hr
halfDuration = (9 hrs - 1 hr) / 2 = 4 hrs  ← 8hrs work / 2

firstHalfEnd    = 08:00 AM + 4 hrs = 12:00 PM  ✓
secondHalfStart = 12:00 PM + 1 hr  = 01:00 PM  ✓

First Half   : 08:00 AM → 12:00 PM
Lunch Break  : 12:00 PM → 01:00 PM
Second Half  : 01:00 PM → 05:00 PM
```

> Early time-in is IGNORED — computation always starts from scheduled start time.

---

## 6. Step-by-Step Evaluation Per Day

### STEP 1 — Rest Day
```
dType == "RD"?
  └─ Yes → Add "RD", STOP
           (ignore leave entirely — RD always wins)
  └─ No  → continue to Step 2
```

### STEP 2 — Leave
```
Has approved leave (LeaveCode is not empty)?
  └─ Yes → evaluate leave (see Step 7)
  └─ No  → continue to Step 3
```

### STEP 3 — Duty Type
```
dType == "RN"?
  └─ Has punch → Add "PRESENT", STOP
  └─ No punch  → blank, STOP

dType == "R"?
  └─ continue to Step 4
```

### STEP 4 — Attendance Type
```
aType == "EXEMPTATTENDANCE"    → Always PRESENT, STOP
aType == "FLEXIBLEATTENDANCE"  → see Step 9
aType == "FIXEDATTENDANCE"     → see Step 8
```

---

## 7. Leave Rules

### Whole Day Leave
```
leave.TimeDuration == expWorkDuration?
  └─ Yes → Add leave code tag (e.g. "VL", "SL"), STOP
```

### Half Day Leave
```
Determine which half the leave covers:
  leaveIsMorning   = leaveStart < firstHalfEnd AND leaveEnd > shiftStart
  leaveIsAfternoon = leaveStart < shiftEnd     AND leaveEnd > secondHalfStart

Always add: "1/2 [LEAVE CODE]" first

Then check the WORKING half:
```

#### Half Day Leave — Past Date
```
No punch?
  └─ Add "1/2 ABSENT", STOP

Has punch:
  Leave is MORNING (working half = AFTERNOON):
    └─ actualIn  > secondHalfStart → Add "LATE"      (actualIn - secondHalfStart)
    └─ actualOut < shiftEnd        → Add "UNDERTIME"  (shiftEnd - actualOut)
    └─ Add "1/2 PRESENT"

  Leave is AFTERNOON (working half = MORNING):
    └─ actualIn  > shiftStart   → Add "LATE"      (actualIn - shiftStart)
    └─ actualOut < firstHalfEnd → Add "UNDERTIME"  (firstHalfEnd - actualOut)
    └─ Add "1/2 PRESENT"
```

#### Half Day Leave — Today (no punch yet)
```
Leave is MORNING (should be in by secondHalfStart):
  └─ now > secondHalfStart               → Add "LATE" (now - secondHalfStart)
  └─ now ≤ secondHalfStart               → Add "1/2 ABSENT"

Leave is AFTERNOON (should be in by shiftStart):
  └─ now > shiftStart AND now ≤ firstHalfEnd → Add "LATE" (now - shiftStart)
  └─ otherwise                               → Add "1/2 ABSENT"
```

#### Half Day Leave — Future Date
```
Add "1/2 [LEAVE CODE]" only, no further computation
```

---

## 8. Fixed Attendance (aType = FIXEDATTENDANCE)

Based on expected schedule. No grace period — 1 min late = LATE.

### Past Date
```
No punch?
  └─ Add "ABSENT", STOP

Has punch:
  └─ actualIn  > shiftStart → Add "LATE"      (actualIn - shiftStart)
  └─ actualOut < shiftEnd   → Add "UNDERTIME"  (shiftEnd - actualOut)
  └─ Add "PRESENT"
```

### Today
```
Has punch → same logic as past date

No punch:
  └─ now < shiftStart → blank (not yet time)
  └─ now > shiftEnd   → Add "ABSENT" (missed the whole day)
  └─ otherwise        → Add "LATE" (now - shiftStart)  ← running late
```

### Future Date
```
→ blank, no computation
```

---

## 9. Flexible Attendance (aType = FLEXIBLEATTENDANCE)

Based on total hours worked vs expected. No schedule-based late.

```
Future date?
  └─ blank, STOP

No punch?
  └─ Add "ABSENT", STOP

Has punch:
  worked   = sum of all punch pairs (TotalWorkedHrs)
  expected = ConvertToTime(expWorkDuration)  ← HHMM format

  worked < expected?
    └─ Add "UNDERTIME" (expected - worked)

  Add "PRESENT"
```

---

## 10. Exempt Attendance (aType = EXEMPTATTENDANCE)

```
→ Always add "PRESENT"
→ No late or undertime computation
→ No punch required
```

---

## 11. Present / Absent Summary

| Condition | Tag |
|-----------|-----|
| No punch, past/today, Regular Fixed | `ABSENT` |
| Has punch, Regular Fixed | `PRESENT` |
| No punch, Regular Flexible, past/today | `ABSENT` |
| Has punch, Regular Flexible | `PRESENT` |
| Exempt (any) | `PRESENT` |
| Half day leave + no punch for working half | `1/2 ABSENT` |
| Half day leave + has punch for working half | `1/2 PRESENT` |
| Regular Normal + has punch | `PRESENT` |
| Regular Normal + no punch | blank |

> `1/2 ABSENT` and `1/2 PRESENT` are ONLY used in half day leave scenarios.
> Late duration does NOT determine half day — late is just late.

---

## 12. Date Scenarios Summary

| Date | Behavior |
|------|----------|
| Past date | Full computation — late, undertime, present, absent |
| Today + has punch | Same as past date |
| Today + no punch + before shift start | Blank |
| Today + no punch + during shift | LATE (running duration) |
| Today + no punch + after shift end | ABSENT |
| Future date | Blank |
| Future date + leave | Show leave tag only |
| Future date + rest day | Show `RD` only |

---

## 13. Summary Counter Rules (ComputeSummary)

Counts are computed after all days are loaded.

| Item | Rule |
|------|------|
| **Working days** | All days except `RD` |
| **Whole day leave** | Skip — does not count as working day |
| **Half day leave** | Count as 0.5 working day |
| **Full day** | Count as 1.0 working day |
| **Present** | `PRESENT` = +1.0, `1/2 PRESENT` = +0.5 |
| **Absent** | `ABSENT` = +1.0, `1/2 ABSENT` = +0.5 |
| **Late count** | Count of days with `LATE` tag |
| **Undertime count** | Count of days with `UNDERTIME` tag |
| **Rest day count** | Count of days with `RD` tag |

### Dynamic Leave Detection

**Never hardcode leave codes** (VL, SL, SIL, ML, etc.) in UI or summary logic.
Use `AttDayDetail.LeaveType` instead — it is populated from DB and works for any leave type.

```
// WRONG — breaks when new leave types are added
s.Contains("VL") || s.Contains("SL")

// CORRECT — dynamic, works for all leave types
!string.IsNullOrWhiteSpace(day.LeaveType)
```

**How to detect leave scenarios:**
```
Whole day leave = LeaveType is not empty
              AND status does NOT contain "PRESENT"
              AND status does NOT contain "ABSENT"

Half day leave  = LeaveType is not empty
              AND status contains "1/2"

IsEmptyDay      = Rest day (RD)
              OR  Whole day leave
              NOT Half day leave (working half must show)
```

**Badge styling** — leave type badge color is determined by `hasLeave` flag, not by code:
```
hasLeave = !string.IsNullOrWhiteSpace(AttDayDetail.LeaveType)
→ any leave type → "bg-primary text-white"
```

---

## 14. HHMM Format Reference

All time values from DB are stored in **HHMM integer format**.

```
ConvertToTime(800)  → 08:00 (8 hrs)
ConvertToTime(1330) → 13:30 (1:30 PM)
ConvertToTime(900)  → 09:00 (9 hrs)
```

Use `ConvertToTime()` whenever reading duration or time values from DB.

---

## 15. Full Decision Tree

```
Per day:
│
├─ dType == "RD"?
│    └─ YES → "RD", STOP
│
├─ hasApprovedLeave?
│    ├─ Whole day → "[LEAVE CODE]", STOP
│    └─ Half day  → "1/2 [LEAVE CODE]"
│         ├─ Past date
│         │    ├─ No punch        → "1/2 ABSENT", STOP
│         │    └─ Has punch
│         │         ├─ Leave morning   → check LATE/UNDERTIME on afternoon half
│         │         └─ Leave afternoon → check LATE/UNDERTIME on morning half
│         │              └─ "1/2 PRESENT"
│         ├─ Today (no punch yet)
│         │    ├─ Leave morning   → now > secondHalfStart? LATE : "1/2 ABSENT"
│         │    └─ Leave afternoon → now > shiftStart?      LATE : "1/2 ABSENT"
│         └─ Future → show tag only
│
├─ dType == "RN"?
│    ├─ Has punch → "PRESENT", STOP
│    └─ No punch  → blank, STOP
│
└─ dType == "R"
     ├─ EXEMPT      → "PRESENT", STOP
     ├─ FLEXIBLE
     │    ├─ Future     → blank
     │    ├─ No punch   → "ABSENT"
     │    └─ Has punch  → worked < expected? "UNDERTIME" then "PRESENT"
     └─ FIXED
          ├─ Past date
          │    ├─ No punch  → "ABSENT"
          │    └─ Has punch → LATE? + UNDERTIME? + "PRESENT"
          ├─ Today
          │    ├─ Has punch → same as past date
          │    └─ No punch
          │         ├─ now < shiftStart → blank
          │         ├─ now > shiftEnd   → "ABSENT"
          │         └─ otherwise        → "LATE" (running)
          └─ Future → blank
```

## 16. Known Leave Types (from DB)

Leave types are stored in the `leavetypes` table and loaded dynamically.
**Never hardcode these in UI logic** — always use `AttDayDetail.LeaveType`.

| Code | Name |
|------|------|
| `SIL` | Service Incentive Leave |
| `SL` | Sick Leave |
| `VL` | Vacation Leave |
| `ML` | Maternity Leave |

> New leave types added to DB will automatically work without code changes.

---

*Reference code: `AttendanceModule.cs`, `CalendarComponent.razor`*