namespace HRApiLibrary.Models._90_Utils
{
    public class StatusModel
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public static class StatusList
    {
        public static List<StatusModel> Statuses { get; } = new()
        {
            new StatusModel { Name = "Active", Code = "A"  },
            new StatusModel { Name = "Inactive", Code = "I" }
        };
    }

    public static class UserStatusList
    {
        public static List<StatusModel> Statuses { get; } = new()
        {
            new StatusModel { Name = "Active", Code = "A" },
            new StatusModel { Name = "Inactive", Code = "D" }
        };
    }
}
