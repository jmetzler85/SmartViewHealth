using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SmartViewHealth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SmartViewHealth.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult HealthSystemUsers(int id, int? page, string searchString)
        {
            _HealthSystemUsersContext hsuContext = new _HealthSystemUsersContext();

            try
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {

                    hsuContext._HealthSystem = dbContext.HealthSystems.Where(x => x.Id == id).FirstOrDefault();

                    var model = (from a in dbContext.AspNetUsers
                                 join b in dbContext.HealthSystemUserAssociations.Where(h => h.healthsystem_id == id && h.row_sta_cd == "A") on a.Id equals b.user_id into abGroup
                                 from b in abGroup.DefaultIfEmpty()
                                 select new UserHealthSystemAssociationViewModel
                                 {
                                     UserName = a.UserName,
                                     UserId = a.Id,
                                     role_name = "",  //to be populated later
                                     association_id = b != null ? b.Id : -1
                                 });

                    if(searchString != null & searchString != "")
                    {
                        model = model.Where(s => s.UserName.Contains(searchString));
                    }

                    var results = model.ToList<UserHealthSystemAssociationViewModel>();

                    //append role to returned records
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    foreach (var s in results)
                    {
                        var list = userManager.GetRoles(s.UserId);
                        s.role_name = list.Count > 0 ? list[0] : "";
                    }

                    //add paging
                    var pager = new Pager(results.Count(), page);

                    hsuContext._UserHealthSystemAssociationViewModel = results.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
                    hsuContext.Pager = pager;

                    if (searchString != null && searchString != "")
                    {
                        ViewBag.ShowRefresh = true;
                    }
                    else
                    {
                        ViewBag.ShowRefresh = false;
                    }

                    return View(hsuContext);
                }
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }

        public List<HealthSystemUserAssociation> getHealthSystemUserAssociation()
        {
            List<HealthSystemUserAssociation> hsUserAssoc = new List<HealthSystemUserAssociation>();
            
            using (SVHDbContext dbContext = new SVHDbContext())
            {
                hsUserAssoc = dbContext.HealthSystemUserAssociations.ToList();
            }

            return hsUserAssoc;
        }


        [Route("users/AssociateUserToHealthSystem/{hSystemId}/{iUserId}/{bChecked}")]
        [HttpPost]
        public ActionResult AssociateUserToHealthSystem(int hSystemId, string iUserId, bool bChecked)
        {
            HealthSystemUserAssociation hsuaDB;

            try
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    //hSystem = dbContext.HealthSystems.Where(h => h.Id == id).FirstOrDefault();
                    hsuaDB = dbContext.HealthSystemUserAssociations.Where(h => h.healthsystem_id == hSystemId
                        && h.user_id == iUserId).FirstOrDefault();

                    if(hsuaDB != null)
                    {
                        ////association found need to change row_sta_cd

                        hsuaDB.user_id = iUserId;
                        hsuaDB.healthsystem_id = hSystemId;
                        hsuaDB.lst_mod_id = User.Identity.GetUserName();
                        hsuaDB.lst_mod_ts = DateTime.Now;
                        hsuaDB.row_sta_cd = bChecked ? "A" : "O";

                        dbContext.SaveChanges();
                    }
                    else {
                        //not found so we need to insert a new association
                        dbContext.HealthSystemUserAssociations.Add(new HealthSystemUserAssociation()
                        {
                            user_id = iUserId,
                            healthsystem_id = hSystemId,
                            entry_user = User.Identity.GetUserName(),
                            lst_mod_id = User.Identity.GetUserName(),
                            lst_mod_ts = DateTime.Now,
                            entry_ts = DateTime.Now,
                            row_sta_cd = "A"
                        });

                        dbContext.SaveChanges();
                    }
                }

                var result = new { Success = "True" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                var result = new { Success = "False", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

    }
}