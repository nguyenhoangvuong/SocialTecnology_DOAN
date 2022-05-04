using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Hubs;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Controllers.ControllerFramword
{
    public class TestMessageController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: TestMessage
        public ActionResult Chat()
        {
            return View();
        }
        
    }
}