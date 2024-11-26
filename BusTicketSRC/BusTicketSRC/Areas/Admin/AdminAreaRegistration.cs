﻿using System.Web.Mvc;

namespace BusTicketSRC.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Admin_dashboard",
               "Admin",
               new { controller = "BookingsAdmin", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_login",
                "Admin/login",
                new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}