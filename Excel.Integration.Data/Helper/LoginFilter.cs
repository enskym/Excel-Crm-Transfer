using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Excel.Integration.Data.Helper
{
    public class LoginFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //var wrapper = new HttpContextWrapper(HttpContext.Current);
            //var sessionControl = context.HttpContext.Session["UserLogin"];
            var user = SalesCustomPrincipal.FromCookie();

            if (user == null)
            {
                //SalesCustomPrincipal.FromCookie();
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } }
                );
            }
            else
            {
                SalesCustomPrincipal.FromCookie();
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}

