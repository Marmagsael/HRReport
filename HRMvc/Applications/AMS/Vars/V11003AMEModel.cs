using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._11_AMS;
using HRApiLibrary.Modules._11003AME;
using HRApiLibrary.Modules._11003O;

namespace HRMvc.Applications.AMS.Vars;

public class V11003AMEModel
{
    // --- Payroll Group ---
    public List<M11003_Payrollgrp> PayrollGrps { get; set; } = [];
    public int? SelectedPayrollGrpId { get; set; }

    // --- Employee List ---
    public List<M11003AME_Empmas> Employees { get; set; } = [];
    public M11003AME_Empmas? SelectedEmployee { get; set; }

    // --- Date Range ---
    public DateTime? DateStart { get; set; } = DateTime.Today.AddDays(-15);
    public DateTime? DateEnd { get; set; } = DateTime.Today;

    // --- Punches Grid ---
    public List<Attpunches1Model> Punches { get; set; } = [];

    // --- Modal ---
    public bool IsModalOpen { get; set; } = false;
    public bool IsEditMode { get; set; } = false;
    public Attpunches1Model ModalPunch { get; set; } = new();

    // --- Modal: Separate date/time fields ---
    public DateTime? ModalPunchInDate { get; set; }
    public int? ModalPunchInTime { get; set; } = -1;
    public DateTime? ModalPunchOutDate { get; set; }
    public int? ModalPunchOutTime { get; set; } = -1;

    // --- Delete Confirm Dialog ---
    public bool IsDeleteDialogOpen { get; set; } = false;
    public Attpunches1Model? PunchToDelete { get; set; }

    // --- Schedule/Time Dropdowns ---
    public List<AttinscheduleModel> Attinschedules { get; set; } =
    [
        new() { Id = -1,    Inschedule = "-" },
        new() { Id = 0,     Inschedule = "12:00 AM" },
        new() { Id = 30,    Inschedule = "12:30 AM" },
        new() { Id = 100,   Inschedule = "01:00 AM" },
        new() { Id = 130,   Inschedule = "01:30 AM" },
        new() { Id = 200,   Inschedule = "02:00 AM" },
        new() { Id = 230,   Inschedule = "02:30 AM" },
        new() { Id = 300,   Inschedule = "03:00 AM" },
        new() { Id = 330,   Inschedule = "03:30 AM" },
        new() { Id = 400,   Inschedule = "04:00 AM" },
        new() { Id = 430,   Inschedule = "04:30 AM" },
        new() { Id = 500,   Inschedule = "05:00 AM" },
        new() { Id = 530,   Inschedule = "05:30 AM" },
        new() { Id = 600,   Inschedule = "06:00 AM" },
        new() { Id = 630,   Inschedule = "06:30 AM" },
        new() { Id = 700,   Inschedule = "07:00 AM" },
        new() { Id = 730,   Inschedule = "07:30 AM" },
        new() { Id = 800,   Inschedule = "08:00 AM" },
        new() { Id = 830,   Inschedule = "08:30 AM" },
        new() { Id = 900,   Inschedule = "09:00 AM" },
        new() { Id = 930,   Inschedule = "09:30 AM" },
        new() { Id = 1000,  Inschedule = "10:00 AM" },
        new() { Id = 1030,  Inschedule = "10:30 AM" },
        new() { Id = 1100,  Inschedule = "11:00 AM" },
        new() { Id = 1130,  Inschedule = "11:30 AM" },
        new() { Id = 1200,  Inschedule = "12:00 PM" },
        new() { Id = 1230,  Inschedule = "12:30 PM" },
        new() { Id = 1300,  Inschedule = "01:00 PM" },
        new() { Id = 1330,  Inschedule = "01:30 PM" },
        new() { Id = 1400,  Inschedule = "02:00 PM" },
        new() { Id = 1430,  Inschedule = "02:30 PM" },
        new() { Id = 1500,  Inschedule = "03:00 PM" },
        new() { Id = 1530,  Inschedule = "03:30 PM" },
        new() { Id = 1600,  Inschedule = "04:00 PM" },
        new() { Id = 1630,  Inschedule = "04:30 PM" },
        new() { Id = 1700,  Inschedule = "05:00 PM" },
        new() { Id = 1730,  Inschedule = "05:30 PM" },
        new() { Id = 1800,  Inschedule = "06:00 PM" },
        new() { Id = 1830,  Inschedule = "06:30 PM" },
        new() { Id = 1900,  Inschedule = "07:00 PM" },
        new() { Id = 1930,  Inschedule = "07:30 PM" },
        new() { Id = 2000,  Inschedule = "08:00 PM" },
        new() { Id = 2030,  Inschedule = "08:30 PM" },
        new() { Id = 2100,  Inschedule = "09:00 PM" },
        new() { Id = 2130,  Inschedule = "09:30 PM" },
        new() { Id = 2200,  Inschedule = "10:00 PM" },
        new() { Id = 2230,  Inschedule = "10:30 PM" },
        new() { Id = 2300,  Inschedule = "11:00 PM" },
        new() { Id = 2330,  Inschedule = "11:30 PM" },
        new() { Id = 2400,  Inschedule = "12:00 AM (End)" },
    ];

    // SchedDuration list (exclude Id = -1)
    public List<AttinscheduleModel> SchedDurations =>
        Attinschedules.Where(x => x.Id >= 0).ToList();

    // Duty Duration list
    public List<AttworkinghourModel> Attworkinghours { get; set; } = [
        new() { Id = 0,     Hours = "-"        },
        new() { Id = 100,   Hours = "1 Hr"     },
        new() { Id = 150,   Hours = "1.5 Hrs"  },
        new() { Id = 200,   Hours = "2 Hrs"    },
        new() { Id = 250,   Hours = "2.5 Hrs"  },
        new() { Id = 300,   Hours = "3 Hrs"    },
        new() { Id = 350,   Hours = "3.5 Hrs"  },
        new() { Id = 400,   Hours = "4 Hrs"    },
        new() { Id = 450,   Hours = "4.5 Hrs"  },
        new() { Id = 500,   Hours = "5 Hrs"    },
        new() { Id = 550,   Hours = "5.5 Hrs"  },
        new() { Id = 600,   Hours = "6 Hrs"    },
        new() { Id = 650,   Hours = "6.5 Hrs"  },
        new() { Id = 700,   Hours = "7 Hrs"    },
        new() { Id = 750,   Hours = "7.5 Hrs"  },
        new() { Id = 800,   Hours = "8 Hrs"    },
        new() { Id = 850,   Hours = "8.5 Hrs"  },
        new() { Id = 900,   Hours = "9 Hrs"    },
        new() { Id = 950,   Hours = "9.5 Hrs"  },
        new() { Id = 1000,  Hours = "10 Hrs"   },
        new() { Id = 1050,  Hours = "10.5 Hrs" },
        new() { Id = 1100,  Hours = "11 Hrs"   },
        new() { Id = 1150,  Hours = "11.5 Hrs" },
        new() { Id = 1200,  Hours = "12 Hrs"   },
        new() { Id = 1250,  Hours = "12.5 Hrs" },
        new() { Id = 1300,  Hours = "13 Hrs"   },
    ];

    // --- Notification ---
    public (string Type, string Message) Notification { get; set; } = (string.Empty, string.Empty);
}
