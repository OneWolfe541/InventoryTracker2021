using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryTracker2021.Context;
using InventoryTracker2021.Models;

namespace InventoryTracker2021.Controllers
{
    public class LocationsController : Controller
    {
        private InventoryContext _inventory = new InventoryContext();

        // GET: Locations
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                var locations = from loc in _inventory.Locations
                                select loc;
                locations = locations.OrderBy(loc => loc.chrNickName);
                return View(locations.ToList());
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // GET: Locations/Counts/5
        public ActionResult Counts(int? ID)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.LocationID = ID;

                var locationItem = _inventory.Locations.Find(ID);

                return View(locationItem);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult LocationDetails(int? ID)
        {
            var locationItem = _inventory.Locations.Find(ID);

            return PartialView("_Details", locationItem);
        }

        public ActionResult LocationSummary(int? ID)
        {
            var query = from categories in _inventory.Categories
                        join inventories in _inventory.Inventories
                            on categories.intCategoryID equals inventories.intCategoryID into catinvgroup
                        from inventory in catinvgroup.DefaultIfEmpty()
                        join unittype in _inventory.UnitTypes
                            on inventory.intUnitTypeID equals unittype.intUnitTypeID into unittypegroup
                        from units in unittypegroup.DefaultIfEmpty()
                        select new
                        {
                            inventory.intInventoryID,
                            inventory.intLocationID,
                            inventory.intCategoryID,
                            Category = categories.chrName,
                            inventory.intUnitTypeID,
                            units.chrUnitType,
                            inventory.bolBroken,
                            intQuantity = inventory.intQuantity??0
                        };


            var summaryList = query.Where(i => i.intLocationID == ID && i.bolBroken == false)
                             .GroupBy(g => new { g.intLocationID, g.intCategoryID, g.Category, g.intUnitTypeID, g.chrUnitType })
                             .Select(s => new LocationSummaryModel
                             {
                                 LocationID = s.Key.intLocationID,
                                 CategoryID = s.Key.intCategoryID,
                                 Category = s.Key.Category,
                                 UnitTypeID = s.Key.intUnitTypeID,
                                 UnitName = s.Key.chrUnitType,
                                 Total = s.Sum(i => i.intQuantity)
                             })
                             .OrderBy(o => o.CategoryID)
                             .ToList();

            return PartialView("_Summary", summaryList);
        }

        // GET: Locations/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.StateList = new SelectList(_inventory.LookUp_States.OrderBy(x => x.chrState), "chrState", "chrState");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // POST: Locations/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([Bind(Exclude = "intLocationID")]Location locationToCreate)
        {
            try
            {
                // TODO: Add insert logic here
                _inventory.Locations.Add(locationToCreate);
                _inventory.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        public ActionResult Update(int id, string Name, string Storage, string Phone, string Code, string Notes)
        {
            try
            {
                var location = _inventory.Locations.Find(id);

                location.chrNickName = Name;
                location.chrStorageName = Storage;
                location.chrPhone = Phone;
                location.chrGateCode = Code;
                location.chrIntructions = Notes;

                _inventory.SaveChanges();

                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Counts", "Locations", new { ID = id });
                return Json(new { Url = redirectUrl });
            }
            catch
            {
                return null;
            }
        }
    }
}