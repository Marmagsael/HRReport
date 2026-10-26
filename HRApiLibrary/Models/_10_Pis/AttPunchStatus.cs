namespace HRApiLibrary.Models._10_Pis;

// AttPunches1.Status :
//   N   - New                  L   - Lock
//   OB0 - OB Whole day         OB1 - OB First Half         OB2 - OB 2nd Half
//   LV0 - Leave Whole Day      LV1 - Leave 1st Half        LV2 - Leave Second Half
public static class AttPunchStatus
{
    public const string New             = "N";
    public const string Lock            = "L";

    public const string OB_WholeDay     = "OB0";
    public const string OB_FirstHalf    = "OB1";
    public const string OB_SecondHalf   = "OB2";

    public const string LV_WholeDay     = "LV0";
    public const string LV_FirstHalf    = "LV1";
    public const string LV_SecondHalf   = "LV2";

    // --- ObDuration.Id / LeavedayType.Id : 1 Whole day, 2 First-Half, 3 Second-Half
    public static string OB(int? obDurationId) => obDurationId switch {
        2 => OB_FirstHalf,
        3 => OB_SecondHalf,
        _ => OB_WholeDay
    };

    public static string LV(int? leaveDayTypeId) => leaveDayTypeId switch {
        2 => LV_FirstHalf,
        3 => LV_SecondHalf,
        _ => LV_WholeDay
    };
}
