using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class ShareViewAdminController : Controller
    {
        // GET: Admin/ShareViewAdmin
        public PartialViewResult PanelAdmin()
        {
            return PartialView();
        }
        public PartialViewResult MenuAdmin()
        {
            return PartialView();
        }
    }
}