using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartViewHealth.Controllers
{
    public class UnAuthorizedUserController : Controller
    {
        // GET: UnAuthorizedUser
        public ActionResult UnAuthorizedUser()
        {
            return View();
        }
    }
}