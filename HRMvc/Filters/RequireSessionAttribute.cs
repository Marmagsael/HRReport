using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMvc.Filters
{
    public class RequireSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var route = context.RouteData.Values;

            var controller  = route["controller"]?.ToString();
            var action      = route["action"]?.ToString();

            // ✅ Allow Authentication (login)
            if (controller == "Authentication") return;

            var session = context.HttpContext.Session;

            var oldPis = session.GetString("OldPis");
            var oldPay = session.GetString("OldPay");

            // ✅ Block if session missing
            if (string.IsNullOrEmpty(oldPis) || string.IsNullOrEmpty(oldPay))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Authentication",
                    null
                );
            }
        }
    }
}