﻿using BusTicketSRC.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BusTicketSRC.Areas.Admin.Controllers.AuthorizeAttributeConfig
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
			// Kiểm tra nếu người dùng có vai trò "Admin"
			return !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session[Const.ADMINIDSESSION]?.ToString());
		}

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Xử lý khi không được phép truy cập
            filterContext.Result = new RedirectResult("~/Home/Login");
        }
    }
}