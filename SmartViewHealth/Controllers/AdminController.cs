using Microsoft.AspNet.Identity;
using SmartViewHealth.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SmartViewHealth.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        //protected override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (!User.IsInRole("Admin"))
        //    {
        //        base.OnAuthorization(filterContext);
        //        filterContext.RequestContext.HttpContext.Response.Redirect("/JEFF", true);
        //    }
        //}

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        #region HealthSystems
        public ActionResult HealthSystems(int? page, bool? bShowObsolete, string searchString)
        {
            _HealthSystemViewModel hsvm = new _HealthSystemViewModel();
            List<HealthSystem> hs = new List<HealthSystem>();
            hs = getHealthSystems(bShowObsolete, searchString);

            //add paging
            var pager = new Pager(hs.Count(), page);

            hsvm._HealthSystems = hs.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            hsvm.Pager = pager;

            if (bShowObsolete != null)
            {
                ViewBag.bShowObsolete = bShowObsolete;
            }
            else
            {
                ViewBag.bShowObsolete = false;
            }

            if(searchString != null && searchString != "")
            {
                ViewBag.ShowRefresh = true;
            }
            else
            {
                ViewBag.ShowRefresh = false;
            }

            return View(hsvm);
        }

        private List<HealthSystem> getHealthSystems(bool? bShowObsolete, string searchString = null)
        {
            List<HealthSystem> hs = new List<HealthSystem>();

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                var hSystems = dbContext.HealthSystems;


                if(searchString != null & searchString != "")
                {
                    hs = dbContext.HealthSystems.Where(s => s.HealthSystemName.Contains(searchString)).ToList();
                    return hs;
                }

                if (bShowObsolete != null && bShowObsolete == true)
                {
                    hs = dbContext.HealthSystems.ToList();
                }
                else
                {
                    hs = dbContext.HealthSystems.Where(d => d.row_sta_cd == "A").ToList();
                }
            }

            return hs;
        }

        public ActionResult CreateHealthSystem()
        {
            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            return View();
        }

        [HttpPost]
        public ActionResult PostHealthSystem(HealthSystem hSystem)
        {
            try
            {
                using (var ctx = new SVHDbContext())
                {
                    ctx.HealthSystems.Add(new HealthSystem()
                    {
                        HealthSystemName = hSystem.HealthSystemName,
                        lst_mod_id = hSystem.entry_user,
                        entry_user = hSystem.entry_user,
                        lst_mod_ts = hSystem.entry_ts,
                        entry_ts = hSystem.entry_ts,
                        row_sta_cd = hSystem.row_sta_cd
                    });

                    ctx.SaveChanges();

                    return RedirectToAction("HealthSystems");
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public ActionResult EditHealthSystem(int id)
        {
            HealthSystem hSystem = new HealthSystem();
            hSystem = getHealthSystem(id);

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            return View(hSystem);
        }

        public HealthSystem getHealthSystem(int id)
        {
            HealthSystem hSystem = new HealthSystem();

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    hSystem = dbContext.HealthSystems.Where(h => h.Id == id).FirstOrDefault();
                }
            }

            return hSystem;
        }

        [HttpPost]
        public ActionResult PutEditHealthSystem(HealthSystem hSystem)
        {
            try
            {
                using (var dbContext = new SVHDbContext())
                {

                    HealthSystem hsDB = new HealthSystem();
                    hsDB = dbContext.HealthSystems.Where(h => h.Id == hSystem.Id).FirstOrDefault();

                    hsDB.HealthSystemName = hSystem.HealthSystemName;
                    hsDB.lst_mod_id = hSystem.lst_mod_id;
                    hsDB.lst_mod_ts = hSystem.lst_mod_ts;
                    hsDB.row_sta_cd = hSystem.row_sta_cd;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("HealthSystems");
        }
        #endregion

        #region Facilities
        public ActionResult Facilities(int id, int? page, bool? bShowObsolete, string searchString)
        {
            _FacilityContext mModel = new _FacilityContext();
            HealthSystem hSystem = new HealthSystem();
            List<Facility> lstFacilities = new List<Facility>();

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    hSystem = getHealthSystem(id);
                    lstFacilities = getFacilities(hSystem.Id, bShowObsolete, searchString);

                    //add paging
                    var pager = new Pager(lstFacilities.Count(), page);

                    mModel._HealthSystem = hSystem;
                    mModel._Facilities = lstFacilities.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
                    mModel.Pager = pager;
                }
            }
            else
            {
                throw new Exception("Health System Not Passed In.");
            }

            if(bShowObsolete != null)
            {
                ViewBag.bShowObsolete = bShowObsolete;
            }
            else
            {
                ViewBag.bShowObsolete = false;
            }

            if (searchString != null && searchString != "")
            {
                ViewBag.ShowRefresh = true;
            }
            else
            {
                ViewBag.ShowRefresh = false;
            }

            return View(mModel);
        }

        [Route("admin/CreateFacility/{hSystemId}")]
        public ActionResult CreateFacility(int hSystemId)
        {
            _CreateFacilityContext cfcModel = new _CreateFacilityContext();
            HealthSystem hSystem = new HealthSystem();
            Facility mfacility = new Facility();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                hSystem = dbContext.HealthSystems.Where(h => h.Id == hSystemId).FirstOrDefault();

                cfcModel._HealthSystem = hSystem;
                cfcModel._Facility = mfacility;
            }

            return View(cfcModel);
        }

        [HttpPost]
        public ActionResult PostFacility(_CreateFacilityContext mFacility)
        {
            try
            {
                using (var ctx = new SVHDbContext())
                {
                    ctx.Facilities.Add(new Facility()
                    {
                        FacilityName = mFacility._Facility.FacilityName,
                        HealthSystemID = mFacility._HealthSystem.Id,
                        lst_mod_id = mFacility._Facility.entry_user,
                        entry_user = mFacility._Facility.entry_user,
                        lst_mod_ts = mFacility._Facility.entry_ts,
                        entry_ts = mFacility._Facility.entry_ts,
                        row_sta_cd = mFacility._Facility.row_sta_cd
                    });

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Facilities", new { id = mFacility._HealthSystem.Id });
        }

        public ActionResult EditFacility(int id)
        {
            _CreateFacilityContext cvcContext = new _CreateFacilityContext();
            HealthSystem hSystem = new HealthSystem();
            Facility oFacility = new Facility();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    oFacility = getFacility(id);

                    if (oFacility != null)
                    {
                        hSystem = getHealthSystem(oFacility.HealthSystemID);

                        cvcContext._HealthSystem = hSystem;
                        cvcContext._Facility = oFacility;
                    }
                }
            }

            return View(cvcContext);
        }

        public Facility getFacility(int id)
        {
            Facility oFacility = new Facility();

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    oFacility = dbContext.Facilities.Where(h => h.Id == id).FirstOrDefault();
                }
            }

            return oFacility;
        }

        [HttpPost]
        public ActionResult PutEditFacility(_CreateFacilityContext cvcContext)
        {
            try
            {
                using (var dbContext = new SVHDbContext())
                {
                    Facility oFacilityDB = new Facility();
                    oFacilityDB = dbContext.Facilities.Where(h => h.Id == cvcContext._Facility.Id).FirstOrDefault();

                    oFacilityDB.FacilityName = cvcContext._Facility.FacilityName;
                    oFacilityDB.HealthSystemID = cvcContext._HealthSystem.Id;
                    oFacilityDB.lst_mod_id = cvcContext._Facility.lst_mod_id;
                    oFacilityDB.lst_mod_ts = cvcContext._Facility.lst_mod_ts;
                    oFacilityDB.row_sta_cd = cvcContext._Facility.row_sta_cd;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Facilities", new { id = cvcContext._HealthSystem.Id });
        }

        public List<Facility> getFacilities(int hSystemId, bool? bShowObsolete, string searchString = null)
        {
            List<Facility> fac = new List<Facility>();

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                if (searchString != null && searchString != "")
                {
                    fac = dbContext.Facilities.Where(s => s.FacilityName.Contains(searchString)).ToList();
                    return fac;
                }


                if (bShowObsolete != null && bShowObsolete == true)
                {
                    fac = dbContext.Facilities.Where(d => d.HealthSystemID == hSystemId).ToList();
                }
                else
                {
                    fac = dbContext.Facilities
                        .Where(d => d.row_sta_cd == "A").ToList()
                        .Where(d => d.HealthSystemID == hSystemId).ToList();
                }
            }

            return fac;
        }
        #endregion

        #region Location
        public ActionResult Locations(int id, int? page, bool? bShowObsolete, string searchString)
        {
            _LocationsContext mModel = new _LocationsContext();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            List<Location> lstLocations = new List<Location>();

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    
                    oFacility = getFacility(id);
                    oHealthSystem = getHealthSystem(oFacility.HealthSystemID);
                    lstLocations = getLocations(oFacility.Id, bShowObsolete, searchString);

                    //add paging
                    var pager = new Pager(lstLocations.Count(), page);

                    mModel._HealthSystem = oHealthSystem;
                    mModel._Facility = oFacility;
                    mModel._Locations = lstLocations.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
                    mModel.Pager = pager;
                }
            }
            else
            {
                throw new Exception("Facility Not Passed In.");
            }

            if (bShowObsolete != null)
            {
                ViewBag.bShowObsolete = bShowObsolete;
            }
            else
            {
                ViewBag.bShowObsolete = false;
            }

            if (searchString != null && searchString != "")
            {
                ViewBag.ShowRefresh = true;
            }
            else
            {
                ViewBag.ShowRefresh = false;
            }

            return View(mModel);
        }

        [Route("admin/CreateLocation/{iFacilityId}")]
        public ActionResult CreateLocation(int iFacilityId)
        {
            _CreateLocationContext cfcModel = new _CreateLocationContext();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility mfacility = new Facility();
            Location oLocation = new Location();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                mfacility = getFacility(iFacilityId);
                oHealthSystem = getHealthSystem(mfacility.HealthSystemID);

                cfcModel._HealthSystem = oHealthSystem;
                cfcModel._Facility = mfacility;
                cfcModel._Location = oLocation;
            }

            return View(cfcModel);
        }

        [HttpPost]
        public ActionResult PostLocation(_CreateLocationContext mLocation)
        {
            try
            {
                using (var ctx = new SVHDbContext())
                {
                    ctx.Locations.Add(new Location()
                    {
                        LocationName = mLocation._Location.LocationName,
                        FacilityId = mLocation._Facility.Id,
                        lst_mod_id = mLocation._Location.entry_user,
                        entry_user = mLocation._Location.entry_user,
                        lst_mod_ts = mLocation._Location.entry_ts,
                        entry_ts = mLocation._Location.entry_ts,
                        row_sta_cd = mLocation._Location.row_sta_cd
                    });

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Locations", new { id = mLocation._Facility.Id });
        }

        public ActionResult EditLocation(int id)
        {
            _CreateLocationContext cvcContext = new _CreateLocationContext();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

            if (id > 0)
            {
                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    oLocation = dbContext.Locations.Where(h => h.Id == id).FirstOrDefault();
                    oFacility = dbContext.Facilities.Where(h => h.Id == oLocation.FacilityId).FirstOrDefault();
                    oHealthSystem = dbContext.HealthSystems.Where(h => h.Id == oFacility.HealthSystemID).FirstOrDefault();
                }
            }

            cvcContext._HealthSystem = oHealthSystem;
            cvcContext._Location = oLocation;
            cvcContext._Facility = oFacility;

            return View(cvcContext);
        }

        [HttpPost]
        public ActionResult PutEditLocation(_CreateLocationContext cvcContext)
        {
            try
            {
                using (var dbContext = new SVHDbContext())
                {
                    Location oLocationDB = new Location();
                    oLocationDB = dbContext.Locations.Where(h => h.Id == cvcContext._Location.Id).FirstOrDefault();

                    oLocationDB.LocationName = cvcContext._Location.LocationName;
                    oLocationDB.FacilityId = cvcContext._Facility.Id;
                    oLocationDB.lst_mod_id = cvcContext._Location.lst_mod_id;
                    oLocationDB.lst_mod_ts = cvcContext._Location.lst_mod_ts;
                    oLocationDB.row_sta_cd = cvcContext._Location.row_sta_cd;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return RedirectToAction("Locations", new { id = cvcContext._Facility.Id });
        }

        public List<Location> getLocations(int iFacilityID, bool? bShowObsolete, string searchString = null)
        {
            List<Location> locList = new List<Location>();

            using (SVHDbContext dbContext = new SVHDbContext())
            {

                if (searchString != null && searchString != "")
                {
                    locList = dbContext.Locations.Where(s => s.LocationName.Contains(searchString)).ToList();
                    return locList;
                }

                if (bShowObsolete != null && bShowObsolete == true)
                {
                    locList = dbContext.Locations.Where(d => d.FacilityId == iFacilityID).ToList();
                }
                else
                {
                    locList = dbContext.Locations
                            .Where(d => d.row_sta_cd == "A").ToList()
                            .Where(d => d.FacilityId == iFacilityID).ToList();
                }
            }

            return locList;
        }

        public Location getLocation(int iLocationId)
        {
            Location oLocation = new Location();

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oLocation = dbContext.Locations.Where(h => h.Id == iLocationId).FirstOrDefault();
            }

            return oLocation;
        }


        #endregion

        #region Equipment

        

        public ActionResult Equipment(int id, int? page, bool? bShowObsolete, string searchString)
        {
            _EquipmentsViewModel mModel = new _EquipmentsViewModel();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();
            List<EquipmentWithType> lstEquipment = new List<EquipmentWithType>();
            List<Equipment_Types> lstEquipmentTypes = new List<Equipment_Types>();

            if (id > 0)
            {
                oLocation = getLocation(id);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                using (SVHDbContext dbContext = new SVHDbContext())
                {
                    lstEquipment = getEquipments(oLocation.Id, bShowObsolete, searchString);
                    lstEquipmentTypes = dbContext.Equipment_Types.ToList();

                    //add paging
                    var pager = new Pager(lstEquipment.Count(), page);

                    mModel._HealthSystem = oHealthSystem;
                    mModel._Facility = oFacility;
                    mModel._Location = oLocation;
                    mModel._Equipments = lstEquipment.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
                    mModel._EquipmentTypes = lstEquipmentTypes;
                    mModel.Pager = pager;
                }
            }
            else
            {
                throw new Exception("Location Not Passed In.");
            }

            if (bShowObsolete != null)
            {
                ViewBag.bShowObsolete = bShowObsolete;
            }
            else
            {
                ViewBag.bShowObsolete = false;
            }

            if (searchString != null && searchString != "")
            {
                ViewBag.ShowRefresh = true;
            }
            else
            {
                ViewBag.ShowRefresh = false;
            }

            return View(mModel);
        }

        public List<EquipmentWithType> getEquipments(int iLocationId, bool? bShowObsolete, string searchString = null)
        {
            List<EquipmentWithType> lstEquipment = new List<EquipmentWithType>();

            using (SVHDbContext dbContext = new SVHDbContext())
            {

                var model = (from a in dbContext.Equipments
                             join b in dbContext.Equipment_Types on a.Equipment_Type_Id equals b.Id into abGroup
                             from b in abGroup.DefaultIfEmpty()
                             where a.LocationId == iLocationId
                             select new EquipmentWithType
                             {
                                 Id = a.Id,
                                 EquipmentName = a.EquipmentName,
                                 LocationId = a.LocationId,
                                 Equipment_Type_Id = b.Id,
                                 EquipmentTypeName = b.Equipment_Type_Name,
                                 lst_mod_id = a.lst_mod_id,
                                 lst_mod_ts = a.lst_mod_ts,
                                 entry_ts = a.entry_ts,
                                 entry_user = a.entry_user,
                                 row_sta_cd = a.row_sta_cd
                             });

                if (searchString != null && searchString != "")
                {
                    model = model.Where(s => s.EquipmentName.Contains(searchString));
                    lstEquipment = model.ToList<EquipmentWithType>();
                    return lstEquipment;
                }

                if (bShowObsolete != null && bShowObsolete == true)
                {
                    lstEquipment = model.ToList<EquipmentWithType>();
                    return lstEquipment;
                }
                else
                {
                    model = model.Where(s => s.row_sta_cd == "A");
                    lstEquipment = model.ToList<EquipmentWithType>();
                    return lstEquipment;
                }
            }
        }

        public ActionResult CreateEquipment(int Id, int EquipmentType)
        {
            //determine which type of equipment we are going to create
            switch(EquipmentType)
            {
                case (int)_EquipmentTypes.Temperature:
                    return CreateTemperatureEquipment(Id, EquipmentType);
                case (int)_EquipmentTypes.SterilizationEquipment:
                    return CreateSEEquipment(Id, EquipmentType);
                case (int)_EquipmentTypes.EmergencyCart:
                    return CreateECEquipment(Id, EquipmentType);
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            
            return View();
        }

        public ActionResult EditEquipment(int Id, int EquipmentType)
        {
            //determine which type of equipment we are going to create
            switch (EquipmentType)
            {
                case (int)_EquipmentTypes.Temperature:
                    return EditTemperatureEquipment(Id, EquipmentType);
                case (int)_EquipmentTypes.SterilizationEquipment:
                    return EditSEEquipment(Id, EquipmentType);
                case (int)_EquipmentTypes.EmergencyCart:
                    return EditECEquipment(Id, EquipmentType);
            }

            return View();
        }

        private ActionResult CreateTemperatureEquipment(int Id, int EquipmentType)
        {
            CreateTemperatureViewModel ctModel = new CreateTemperatureViewModel();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oLocation = getLocation(Id);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ctModel._HealthSystem = oHealthSystem;
                ctModel._Facility = oFacility;
                ctModel._Location = oLocation;
            }

            return View("CreateTemperatureEquipment", ctModel);
        }

        [HttpPost]
        public ActionResult PostCreateTemperature(CreateTemperatureViewModel ctViewModel)
        {
            Equipment _Equipment;
            Equipment_Temp _EquipmentTemp;

                using (var ctx = new SVHDbContext())
                {
                    using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            _Equipment = new Equipment()
                                {
                                    EquipmentName = ctViewModel._Equipment.EquipmentName,
                                    LocationId = ctViewModel._Location.Id,
                                    Equipment_Type_Id = ctViewModel._Equipment.Equipment_Type_Id,
                                    lst_mod_id = ctViewModel._Equipment.entry_user,
                                    entry_user = ctViewModel._Equipment.entry_user,
                                    lst_mod_ts = ctViewModel._Equipment.entry_ts,
                                    entry_ts = ctViewModel._Equipment.entry_ts,
                                    row_sta_cd = ctViewModel._Equipment.row_sta_cd
                                };

                            ctx.Equipments.Add(_Equipment);
                            ctx.SaveChanges();

                            _EquipmentTemp = new Equipment_Temp()
                            {
                                Equipment_Parent_Id = _Equipment.Id,
                                Temp_Range_Low = ctViewModel._Equipment_Temp.Temp_Range_Low,
                                Temp_Range_High = ctViewModel._Equipment_Temp.Temp_Range_High,
                                Temp_Hour_Chk = ctViewModel._Equipment_Temp.Temp_Hour_Chk,
                                lst_mod_id = ctViewModel._Equipment.entry_user,
                                entry_user = ctViewModel._Equipment.entry_user,
                                lst_mod_ts = ctViewModel._Equipment.entry_ts,
                                entry_ts = ctViewModel._Equipment.entry_ts
                            };

                            ctx.Equipment_Temp.Add(_EquipmentTemp);
                            ctx.SaveChanges();
                        
                            transaction.Commit();
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            throw (ex);
                        }
                    }
                }

            return RedirectToAction("Equipment", new { id = ctViewModel._Location.Id });
        }

        private ActionResult EditTemperatureEquipment(int Id, int EquipmentType)
        {
            CreateTemperatureViewModel ctModel = new CreateTemperatureViewModel();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();
            Equipment oEquipment = new Equipment();
            Equipment_Temp oEquipment_Temp = new Equipment_Temp();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oEquipment = dbContext.Equipments.Where(h => h.Id == Id).FirstOrDefault();
                oEquipment_Temp = dbContext.Equipment_Temp.Where(h => h.Equipment_Parent_Id == Id).FirstOrDefault();
                oLocation = getLocation(oEquipment.LocationId);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ctModel._HealthSystem = oHealthSystem;
                ctModel._Facility = oFacility;
                ctModel._Location = oLocation;
                ctModel._Equipment = oEquipment;
                ctModel._Equipment_Temp = oEquipment_Temp;
            }

            return View("EditTemperatureEquipment", ctModel);
        }

        public ActionResult PutEditTempEquipment(CreateTemperatureViewModel ctViewModel)
        {
            Equipment _Equipment;
            Equipment_Temp _EquipmentTemp;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        _Equipment = ctx.Equipments.Where(h => h.Id == ctViewModel._Equipment.Id).FirstOrDefault();

                        _Equipment.EquipmentName = ctViewModel._Equipment.EquipmentName;
                        _Equipment.LocationId = ctViewModel._Location.Id;
                        _Equipment.lst_mod_id = ctViewModel._Equipment.lst_mod_id;
                        _Equipment.lst_mod_ts = ctViewModel._Equipment.lst_mod_ts;
                        _Equipment.row_sta_cd = ctViewModel._Equipment.row_sta_cd;

                        ctx.SaveChanges();

                        _EquipmentTemp = ctx.Equipment_Temp.Where(h => h.Id == ctViewModel._Equipment_Temp.Id).FirstOrDefault();

                        _EquipmentTemp.Temp_Range_Low = ctViewModel._Equipment_Temp.Temp_Range_Low;
                        _EquipmentTemp.Temp_Range_High = ctViewModel._Equipment_Temp.Temp_Range_High;
                        _EquipmentTemp.Temp_Hour_Chk = ctViewModel._Equipment_Temp.Temp_Hour_Chk;
                        _EquipmentTemp.lst_mod_id = ctViewModel._Equipment.lst_mod_id;
                        _EquipmentTemp.lst_mod_ts = ctViewModel._Equipment.lst_mod_ts;

                        ctx.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return RedirectToAction("Equipment", new { id = ctViewModel._Location.Id });

        }

        private ActionResult CreateECEquipment(int Id, int EquipmentType)
        {
            CreateECViewModel ecModel = new CreateECViewModel();

            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oLocation = getLocation(Id);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ecModel._HealthSystem = oHealthSystem;
                ecModel._Facility = oFacility;
                ecModel._Location = oLocation;
            }

            return View("CreateECEquipment", ecModel);
        }

        [HttpPost]
        public ActionResult PostCreateEC(CreateTemperatureViewModel ecViewModel)
        {
            Equipment _Equipment;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        _Equipment = new Equipment()
                        {
                            EquipmentName = ecViewModel._Equipment.EquipmentName,
                            LocationId = ecViewModel._Location.Id,
                            Equipment_Type_Id = ecViewModel._Equipment.Equipment_Type_Id,
                            lst_mod_id = ecViewModel._Equipment.entry_user,
                            entry_user = ecViewModel._Equipment.entry_user,
                            lst_mod_ts = ecViewModel._Equipment.entry_ts,
                            entry_ts = ecViewModel._Equipment.entry_ts,
                            row_sta_cd = ecViewModel._Equipment.row_sta_cd
                        };

                        ctx.Equipments.Add(_Equipment);
                        ctx.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return RedirectToAction("Equipment", new { id = ecViewModel._Location.Id });
        }

        private ActionResult EditECEquipment(int Id, int EquipmentType)
        {
            CreateECViewModel ecModel = new CreateECViewModel();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();
            Equipment oEquipment = new Equipment();
            List<Equipment_EC_Supplies> oEC_Supplies = new List<Equipment_EC_Supplies>();
            List<Equipment_EC_Meds> oEC_Meds = new List<Equipment_EC_Meds>();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oEquipment = dbContext.Equipments.Where(h => h.Id == Id).FirstOrDefault();

                
                oEC_Supplies = dbContext.Equipment_EC_Supplies.Where(h => h.Equipment_Parent_Id == Id)
                                    .Where(h => h.row_sta_cd == "A").ToList();
                oEC_Meds = dbContext.Equipment_EC_Meds.Where(h => h.Equipment_Parent_Id == Id)
                                    .Where(h => h.row_sta_cd == "A").ToList();


                oLocation = getLocation(oEquipment.LocationId);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ecModel._HealthSystem = oHealthSystem;
                ecModel._Facility = oFacility;
                ecModel._Location = oLocation;
                ecModel._Equipment = oEquipment;
                ecModel._Equipment_EC_Supplies = oEC_Supplies;
                ecModel._Equipment_EC_Meds = oEC_Meds;

            }

            return View("EditECEquipment", ecModel);
        }

        public ActionResult PutEditECEquipment(CreateECViewModel ecViewModel)
        {
            Equipment _Equipment;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        _Equipment = ctx.Equipments.Where(h => h.Id == ecViewModel._Equipment.Id).FirstOrDefault();

                        _Equipment.EquipmentName = ecViewModel._Equipment.EquipmentName;
                        _Equipment.LocationId = ecViewModel._Location.Id;
                        _Equipment.lst_mod_id = ecViewModel._Equipment.lst_mod_id;
                        _Equipment.lst_mod_ts = ecViewModel._Equipment.lst_mod_ts;
                        _Equipment.row_sta_cd = ecViewModel._Equipment.row_sta_cd;

                        ctx.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return RedirectToAction("Equipment", new { id = ecViewModel._Location.Id });

        }

        [HttpPost]
        public ActionResult UpdateECEquipmentSupplies(Equipment_EC_Supplies ecSupply)
        {
            Equipment_EC_Supplies _Supply;
            List<Equipment_EC_Supplies> lstSupplies;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if(ecSupply.Id == -1)
                        {
                            //INSERT new supply

                            _Supply = new Equipment_EC_Supplies()
                            {
                                Equipment_Parent_Id = ecSupply.Equipment_Parent_Id,
                                Supply_Name = ecSupply.Supply_Name,
                                Supply_Qty = ecSupply.Supply_Qty,
                                entry_user = ecSupply.entry_user,
                                entry_ts = ecSupply.entry_ts,
                                lst_mod_id = ecSupply.lst_mod_id,
                                lst_mod_ts = ecSupply.lst_mod_ts,
                                row_sta_cd = ecSupply.row_sta_cd
                            };

                            ctx.Equipment_EC_Supplies.Add(_Supply);
                            ctx.SaveChanges();

                        }
                        else
                        {
                            //update supply
                            _Supply = ctx.Equipment_EC_Supplies.Where(h => h.Id == ecSupply.Id).FirstOrDefault();

                            _Supply.Supply_Name = ecSupply.Supply_Name;
                            _Supply.Supply_Qty = ecSupply.Supply_Qty;
                            _Supply.lst_mod_id = ecSupply.lst_mod_id;
                            _Supply.lst_mod_ts = ecSupply.lst_mod_ts;
                            _Supply.row_sta_cd = ecSupply.row_sta_cd;

                            ctx.SaveChanges();
                        }

                        transaction.Commit();

                        //now get all the supplies to return back
                        lstSupplies = ctx.Equipment_EC_Supplies.Where(h => h.Equipment_Parent_Id == ecSupply.Equipment_Parent_Id)
                                                        .Where(h => h.row_sta_cd == "A").ToList();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return PartialView("EC_Supplies_List", lstSupplies);
        }


        [HttpPost]
        public ActionResult UpdateECEquipmentMeds(Equipment_EC_Meds ecMed)
        {
            Equipment_EC_Meds _Med;
            List<Equipment_EC_Meds> lstMeds;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if (ecMed.Id == -1)
                        {
                            //INSERT new supply

                            _Med = new Equipment_EC_Meds()
                            {
                                Equipment_Parent_Id = ecMed.Equipment_Parent_Id,
                                Med_Name = ecMed.Med_Name,
                                Med_UOI = ecMed.Med_UOI,
                                Med_Qty = ecMed.Med_Qty,
                                Med_exp_dt = ecMed.Med_exp_dt,
                                entry_user = ecMed.entry_user,
                                entry_ts = ecMed.entry_ts,
                                lst_mod_id = ecMed.lst_mod_id,
                                lst_mod_ts = ecMed.lst_mod_ts,
                                row_sta_cd = ecMed.row_sta_cd
                            };

                            ctx.Equipment_EC_Meds.Add(_Med);
                            ctx.SaveChanges();

                        }
                        else
                        {
                            //update supply
                            _Med = ctx.Equipment_EC_Meds.Where(h => h.Id == ecMed.Id).FirstOrDefault();

                            _Med.Med_Name = ecMed.Med_Name;
                            _Med.Med_UOI = ecMed.Med_UOI;
                            _Med.Med_Qty = ecMed.Med_Qty;
                            _Med.Med_exp_dt = ecMed.Med_exp_dt;
                            _Med.lst_mod_id = ecMed.lst_mod_id;
                            _Med.lst_mod_ts = ecMed.lst_mod_ts;
                            _Med.row_sta_cd = ecMed.row_sta_cd;

                            ctx.SaveChanges();
                        }

                        transaction.Commit();

                        //now get all the supplies to return back
                        lstMeds = ctx.Equipment_EC_Meds.Where(h => h.Equipment_Parent_Id == ecMed.Equipment_Parent_Id)
                                                        .Where(h => h.row_sta_cd == "A").ToList();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return PartialView("EC_Meds_List", lstMeds);
        }

        private ActionResult CreateSEEquipment(int Id, int EquipmentType)
        {
            CreateSEViewModel ecModel = new CreateSEViewModel();

            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oLocation = getLocation(Id);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ecModel._HealthSystem = oHealthSystem;
                ecModel._Facility = oFacility;
                ecModel._Location = oLocation;
            }

            return View("CreateSEEquipment", ecModel);
        }

        [HttpPost]
        public ActionResult PostCreateSE(CreateSEViewModel ecViewModel)
        {
            Equipment _Equipment;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        _Equipment = new Equipment()
                        {
                            EquipmentName = ecViewModel._Equipment.EquipmentName,
                            LocationId = ecViewModel._Location.Id,
                            Equipment_Type_Id = ecViewModel._Equipment.Equipment_Type_Id,
                            lst_mod_id = ecViewModel._Equipment.entry_user,
                            entry_user = ecViewModel._Equipment.entry_user,
                            lst_mod_ts = ecViewModel._Equipment.entry_ts,
                            entry_ts = ecViewModel._Equipment.entry_ts,
                            row_sta_cd = ecViewModel._Equipment.row_sta_cd
                        };

                        ctx.Equipments.Add(_Equipment);
                        ctx.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return RedirectToAction("Equipment", new { id = ecViewModel._Location.Id });
        }

        private ActionResult EditSEEquipment(int Id, int EquipmentType)
        {
            CreateSEViewModel ecModel = new CreateSEViewModel();
            HealthSystem oHealthSystem = new HealthSystem();
            Facility oFacility = new Facility();
            Location oLocation = new Location();
            Equipment oEquipment = new Equipment();
            List<Equipment_SE_Tasks> oSE_Tasks = new List<Equipment_SE_Tasks>();

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            ViewBag.EquipmentTypeId = EquipmentType;

            using (SVHDbContext dbContext = new SVHDbContext())
            {
                oEquipment = dbContext.Equipments.Where(h => h.Id == Id).FirstOrDefault();


                oSE_Tasks = dbContext.Equipment_SE_Tasks.Where(h => h.Equipment_Parent_Id == Id)
                                    .Where(h => h.row_sta_cd == "A").ToList();

                oLocation = getLocation(oEquipment.LocationId);
                oFacility = getFacility(oLocation.FacilityId);
                oHealthSystem = getHealthSystem(oFacility.HealthSystemID);

                ecModel._HealthSystem = oHealthSystem;
                ecModel._Facility = oFacility;
                ecModel._Location = oLocation;
                ecModel._Equipment = oEquipment;
                ecModel._Equipment_SE_Tasks_Daily = oSE_Tasks.Where(h => h.Task_Type_Id == "D").ToList();
                ecModel._Equipment_SE_Tasks_Weekly = oSE_Tasks.Where(h => h.Task_Type_Id == "W").ToList();
                ecModel._Equipment_SE_Tasks_Monthly = oSE_Tasks.Where(h => h.Task_Type_Id == "M").ToList();
            }

            return View("EditSEEquipment", ecModel);
        }

        [HttpPost]
        public ActionResult UpdateSEEquipmentTasks(Equipment_SE_Tasks seTasks)
        {
            Equipment_SE_Tasks _Task;
            List<Equipment_SE_Tasks> lstTasks;

            using (var ctx = new SVHDbContext())
            {
                using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if (seTasks.Id == -1)
                        {
                            //INSERT new task

                            _Task = new Equipment_SE_Tasks()
                            {
                                Equipment_Parent_Id = seTasks.Equipment_Parent_Id,
                                Task_Type_Id = seTasks.Task_Type_Id,
                                Task_Text = seTasks.Task_Text,
                                entry_user = seTasks.entry_user,
                                entry_ts = seTasks.entry_ts,
                                lst_mod_id = seTasks.lst_mod_id,
                                lst_mod_ts = seTasks.lst_mod_ts,
                                row_sta_cd = seTasks.row_sta_cd
                            };

                            ctx.Equipment_SE_Tasks.Add(_Task);
                            ctx.SaveChanges();

                        }
                        else
                        {
                            //update supply
                            _Task = ctx.Equipment_SE_Tasks.Where(h => h.Id == seTasks.Id).FirstOrDefault();

                            _Task.Task_Text = seTasks.Task_Text;
                            _Task.lst_mod_id = seTasks.lst_mod_id;
                            _Task.lst_mod_ts = seTasks.lst_mod_ts;
                            _Task.row_sta_cd = seTasks.row_sta_cd;

                            ctx.SaveChanges();
                        }

                        transaction.Commit();

                        //now get all the supplies to return back
                        lstTasks = ctx.Equipment_SE_Tasks.Where(h => h.Equipment_Parent_Id == seTasks.Equipment_Parent_Id)
                                                        .Where(h => h.row_sta_cd == "A")
                                                        .Where(h => h.Task_Type_Id == seTasks.Task_Type_Id).ToList();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw (ex);
                    }
                }
            }

            return PartialView("SC_TaskList", lstTasks);
        }


        #endregion

        [Authorize(Roles = "Admin, Manager, Logger")]
        public ActionResult EquipmentLogging()
        {
            return RedirectToAction("Index", "Logging");
        }

        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Reports()
        {
            return View();
        }
    }
}
