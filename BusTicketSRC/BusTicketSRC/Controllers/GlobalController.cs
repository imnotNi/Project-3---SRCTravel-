using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusTicketSRC;
using BusTicketSRC.Models;
using BusTicketSRC.Shared;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

namespace BusTicketSRC.Controllers
{
    [SessionExpire]
    public class GlobalController : Controller
    {
        public GlobalController()
        {  
        }
    }
}
