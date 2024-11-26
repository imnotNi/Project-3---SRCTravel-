using BusTicketSRC.Shared;
using System;
using System.Web;
using System.Web.Mvc;

namespace BusTicketSRC.Controllers
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra session ở đây
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Session[Const.USERIDSESSION]?.ToString()))
            {
                // làm gì khi chưa đăng nhập ở đây
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
