using Blazorise;
using Radzen;

namespace HRMvc.Applications.Vars
{
    public static class AppNotification
    {
        public static void ShowSuccess(this NotificationService service, string summary, string detail, int duration = 5000)
        {
            service.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        public static void ShowError(this NotificationService service, string summary, string detail, int duration = 5000)
        {
            service.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        public static void ShowInfo(this NotificationService service, string summary, string detail, int duration = 5000)
        {
            service.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        public static void ShowWarning(this NotificationService service, string summary, string detail, int duration = 5000)
        {
            service.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }
    }
}