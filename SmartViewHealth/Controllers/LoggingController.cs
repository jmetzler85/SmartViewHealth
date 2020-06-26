using Microsoft.AspNet.Identity;
using SmartViewHealth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace SmartViewHealth.Controllers
{
    public class LoggingController : Controller
    {
        // GET: Logging
        public ActionResult Index(int? iHealthSystemId, int? iFacilityId, int? iLocationId, int? iEquipmentId)
        {
            LoggingViewModel logViewModel = new LoggingViewModel();
            PreloadEquipmentLogInfo peli = new PreloadEquipmentLogInfo();
            List<LoggingHealthSystemViewModel> lstHealthSystems;

            //get health systems
            lstHealthSystems = getHealthSystemsAssociatedToUser();

            //add to view model
            logViewModel.lstHealthSystems = lstHealthSystems;

            //if reloading add equipment id
            if(iHealthSystemId != null)
            {
                peli.HealthSystemId = iHealthSystemId;
                peli.FacilityId = iFacilityId;
                peli.LocationId = iLocationId;
                peli.EquipmentId = iEquipmentId;
            }
            else
            {
                peli.HealthSystemId = -1;
                peli.FacilityId = -1;
                peli.LocationId = -1;
                peli.EquipmentId = -1;
            }
            logViewModel.peli = peli;

            return View("EquipmentLogging", logViewModel);
        }

        private List<LoggingHealthSystemViewModel> getHealthSystemsAssociatedToUser()
        {
            List<LoggingHealthSystemViewModel> hs = new List<LoggingHealthSystemViewModel>();
            string userid = HttpContext.User.Identity.GetUserId();

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                //return health systems associated to the logged in user
                var model = (from a in dbContext.HealthSystems.Where(h => h.row_sta_cd == "A")
                             join b in dbContext.HealthSystemUserAssociations.Where(h => h.row_sta_cd == "A") 
                                    .Where(h => h.user_id == userid)           
                                    on a.Id equals b.healthsystem_id 
                             select new LoggingHealthSystemViewModel
                             {
                                 Id = a.Id,
                                 HealthSystemName = a.HealthSystemName,
                             });

                hs = model.ToList<LoggingHealthSystemViewModel>();

                if(hs.Count == 0)
                {
                    hs.Add(new LoggingHealthSystemViewModel
                    {
                        Id = -1,
                        HealthSystemName = "You need to associate your user id to a health system"
                    });
                }
            }

            return hs;
        }

        public ActionResult getHealthSystemsAssociatedToUserJson()
        {
            List<LoggingHealthSystemViewModel> lstHealthSystems;

            //get health systems
            lstHealthSystems = getHealthSystemsAssociatedToUser();

            //turn object into JSON
            var json = new JavaScriptSerializer().Serialize(lstHealthSystems);

            //return json object of facilities
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFacilities(int iHealthSystemId)
        {
            AdminController ac = new AdminController();
            
            List<Facility> lstFacilities;

            //get facilities for passed in health system
            lstFacilities = ac.getFacilities(iHealthSystemId, false);

            //turn object into JSON
            var json = new JavaScriptSerializer().Serialize(lstFacilities);

            //return json object of facilities
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocations(int iFacilityId)
        {
            AdminController ac = new AdminController();

            List<Location> lstLocations;

            //get facilities for passed in health system
            lstLocations = ac.getLocations(iFacilityId, false);

            //turn object into JSON
            var json = new JavaScriptSerializer().Serialize(lstLocations);

            //return json object of facilities
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipments(int iLocationId)
        {
            AdminController ac = new AdminController();

            List<EquipmentWithType> lstEquipment;

            //get facilities for passed in health system
            lstEquipment = ac.getEquipments(iLocationId, false);

            //turn object into JSON
            var json = new JavaScriptSerializer().Serialize(lstEquipment);

            //return json object of facilities
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private Equipment GetEquipment(int iEquipmentId)
        {
            Equipment eq;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                //get equipment
                eq = dbContext.Equipments.Where(h => h.Id == iEquipmentId).FirstOrDefault();
            }

            return eq;
        }

        public ActionResult GetEquipmentLogs(int iEquipmentId)
        {
            

            if (iEquipmentId > 0)
            {
                try
                 {
                    using (SVHDbContext dbContext = new SVHDbContext())
                    {
                        //get equipment
                        Equipment eq = GetEquipment(iEquipmentId);

                        switch (eq.Equipment_Type_Id)
                        {
                            case (int)_EquipmentTypes.Temperature:
                                TempEquipmentViewModel tevm = new TempEquipmentViewModel();
                                List<Equipment_Temp_Log> lstTempLogs;

                                //get logs
                                lstTempLogs = dbContext.Equipment_Temp_Log.Where(h => h.Equipment_Id == eq.Id)
                                    .Where(h => h.row_sta_cd == "A")
                                    .OrderByDescending(h => h.lst_mod_ts)
                                    .Take(10)
                                    .ToList();

                                //add to view model
                                tevm._equipment = eq;
                                tevm.lstTempLogs = lstTempLogs;

                                return PartialView("TempLoggingGrid", tevm);

                            case (int)_EquipmentTypes.SterilizationEquipment:
                                SEEquipmentViewModel sevm = new SEEquipmentViewModel();
                                List<Equipment_SE_Log> lstSELogs;

                                //get logs
                                lstSELogs = dbContext.Equipment_SE_Log.Where(h => h.Equipment_Id == eq.Id)
                                    .Where(h => h.row_sta_cd == "A")
                                    .OrderByDescending(h => h.lst_mod_ts)
                                    .Take(10)
                                    .ToList();

                                //add to view model
                                sevm._equipment = eq;
                                sevm.lstSELogs = lstSELogs;

                                return PartialView("SELoggingGrid", sevm);
                        }
                    }

                }
                catch(Exception ex)
                {
                    throw ex;
                }
                
            }

            throw new Exception("Error retrieving Equipment logs.");
        }

        public ActionResult CreateTempLog(int iEquipmentId)
        {
            TempEquipmentViewModel tevm = new TempEquipmentViewModel();
            AdminController ac = new AdminController();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (iEquipmentId > 0)
            {
                tevm._equipment = GetEquipment(iEquipmentId);
                tevm._Location = ac.getLocation(tevm._equipment.LocationId);
                tevm._Facility = ac.getFacility(tevm._Location.FacilityId);
                tevm._HealthSystem = ac.getHealthSystem(tevm._Facility.HealthSystemID);

                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    tevm._equipment_temp = dbContext.Equipment_Temp.Where(h => h.Equipment_Parent_Id == iEquipmentId).FirstOrDefault();
                }
            }

            return View("CreateTempLog", tevm);
        }

        public ActionResult PostTempLog(TempEquipmentViewModel tevm)
        {
            try
            {
                using (var ctx = new SVHDbContext())
                {
                    ctx.Equipment_Temp_Log.Add(new Equipment_Temp_Log()
                    {
                        Equipment_Id = tevm._equipment.Id,
                        temp_value = tevm.TempValue,
                        lst_mod_id = tevm._equipment.entry_user,
                        entry_user = tevm._equipment.entry_user,
                        lst_mod_ts = tevm._equipment.entry_ts,
                        entry_ts = tevm._equipment.entry_ts,
                        row_sta_cd = tevm._equipment.row_sta_cd
                    });

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Index", "Logging", new { iHealthSystemId = tevm._HealthSystem.Id,
                                                              iFacilityId = tevm._Facility.Id,
                                                              iLocationId = tevm._Location.Id,
                                                              iEquipmentId = tevm._equipment.Id
            });

        }

        public ActionResult EditTempLog(int Id)
        {
            TempEquipmentViewModel tevm = new TempEquipmentViewModel();
            AdminController ac = new AdminController();
            Equipment_Temp_Log etlLog;

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (Id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    etlLog = dbContext.Equipment_Temp_Log.Where(h => h.Id == Id).FirstOrDefault();
                    tevm._equipment_temp = dbContext.Equipment_Temp.Where(h => h.Equipment_Parent_Id == etlLog.Equipment_Id).FirstOrDefault();
                    tevm.TempValue = etlLog.temp_value;
                }


                tevm._equipment = GetEquipment(etlLog.Equipment_Id);
                tevm._Location = ac.getLocation(tevm._equipment.LocationId);
                tevm._Facility = ac.getFacility(tevm._Location.FacilityId);
                tevm._HealthSystem = ac.getHealthSystem(tevm._Facility.HealthSystemID);
                tevm.TempLogId = Id;
            }

            return View("EditTempLog", tevm);
        }

        public ActionResult PutTempLog(TempEquipmentViewModel tevm)
        {
            Equipment_Temp_Log etlLog;

            try
            {
                using (var ctx = new SVHDbContext())
                {

                    etlLog = ctx.Equipment_Temp_Log.Where(h => h.Id == tevm.TempLogId).FirstOrDefault();
                    
                    etlLog.temp_value = tevm.TempValue;
                    etlLog.lst_mod_id = tevm._equipment.lst_mod_id;
                    etlLog.lst_mod_ts = tevm._equipment.lst_mod_ts;
                    etlLog.row_sta_cd = tevm._equipment.row_sta_cd;

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Index", "Logging", new
            {
                iHealthSystemId = tevm._HealthSystem.Id,
                iFacilityId = tevm._Facility.Id,
                iLocationId = tevm._Location.Id,
                iEquipmentId = tevm._equipment.Id
            });

        }

        public ActionResult CreateSELog(int iEquipmentId, string iTaskTypeId)
        {
            SEEquipmentViewModel sevm = new SEEquipmentViewModel();
            AdminController ac = new AdminController();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (iEquipmentId > 0)
            {
                sevm._equipment = GetEquipment(iEquipmentId);
                sevm._Location = ac.getLocation(sevm._equipment.LocationId);
                sevm._Facility = ac.getFacility(sevm._Location.FacilityId);
                sevm._HealthSystem = ac.getHealthSystem(sevm._Facility.HealthSystemID);

                sevm.SETaskType = iTaskTypeId;

                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    sevm._equipment_SE_Tasks = dbContext.Equipment_SE_Tasks.Where(h => h.Equipment_Parent_Id == iEquipmentId)
                                                .Where(h => h.Task_Type_Id == iTaskTypeId).ToList();
                }
            }

            return View("CreateSELog", sevm);
        }

        public ActionResult PostSELog(SEEquipmentViewModel sevm)
        {
            try
            {
                using (var ctx = new SVHDbContext())
                {
                    ctx.Equipment_SE_Log.Add(new Equipment_SE_Log()
                    {
                        Equipment_Id = sevm._equipment.Id,
                        Task_Type_Id = sevm.SETaskType,
                        Tasks_RU_fg = sevm.Tasks_RU_fg,
                        Tasks_QandA = sevm.Tasks_QandA,
                        lst_mod_id = sevm._equipment.entry_user,
                        entry_user = sevm._equipment.entry_user,
                        lst_mod_ts = sevm._equipment.entry_ts,
                        entry_ts = sevm._equipment.entry_ts,
                        row_sta_cd = sevm._equipment.row_sta_cd
                    });

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Index", "Logging", new
            {
                iHealthSystemId = sevm._HealthSystem.Id,
                iFacilityId = sevm._Facility.Id,
                iLocationId = sevm._Location.Id,
                iEquipmentId = sevm._equipment.Id
            });

        }

        public ActionResult EditSELog(int Id)
        {
            SEEquipmentViewModel sevm = new SEEquipmentViewModel();
            AdminController ac = new AdminController();
            Equipment_SE_Log seLog;

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (Id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    seLog = dbContext.Equipment_SE_Log.Where(h => h.Id == Id).FirstOrDefault();
                }


                sevm._equipment = GetEquipment(seLog.Equipment_Id);
                sevm._Location = ac.getLocation(sevm._equipment.LocationId);
                sevm._Facility = ac.getFacility(sevm._Location.FacilityId);
                sevm._HealthSystem = ac.getHealthSystem(sevm._Facility.HealthSystemID);
                sevm._SE_Log = seLog;
            }

            return View("EditSELog", sevm);
        }

        public ActionResult PutSELog(SEEquipmentViewModel sevm)
        {
            Equipment_SE_Log seLog;

            try
            {
                using (var ctx = new SVHDbContext())
                {

                    seLog = ctx.Equipment_SE_Log.Where(h => h.Id == sevm._SE_Log.Id).FirstOrDefault();

                    seLog.lst_mod_id = sevm._equipment.lst_mod_id;
                    seLog.lst_mod_ts = sevm._equipment.lst_mod_ts;
                    seLog.row_sta_cd = sevm._equipment.row_sta_cd;

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Index", "Logging", new
            {
                iHealthSystemId = sevm._HealthSystem.Id,
                iFacilityId = sevm._Facility.Id,
                iLocationId = sevm._Location.Id,
                iEquipmentId = sevm._equipment.Id
            });

        }

    }
}