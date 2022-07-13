using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryTracker2021.Context;
using InventoryTracker2021.Models;

namespace InventoryTracker2021.Controllers
{
    public class InventoryController : Controller
    {
        private InventoryContext _inventory = new InventoryContext();

        // GET: Inventory
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult Details(int? Loc_ID, int? Cat_ID)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.LocationID = Loc_ID;
                ViewBag.CategoryID = Cat_ID;
                Location locationRecord = _inventory.Locations.Find(Loc_ID);
                ViewBag.Location = locationRecord.chrNickName;
                Category categoryRecord = _inventory.Categories.Find(Cat_ID);
                ViewBag.Category = categoryRecord.chrName;

                var query = from categories in _inventory.Categories
                            join inventories in _inventory.Inventories
                                on categories.intCategoryID equals inventories.intCategoryID into catinvgroup
                            from inventory in catinvgroup.DefaultIfEmpty()
                            join unittype in _inventory.UnitTypes
                                on inventory.intUnitTypeID equals unittype.intUnitTypeID into unittypegroup
                            from units in unittypegroup.DefaultIfEmpty()
                            join user in _inventory.Users
                                on inventory.intChangedByID equals user.intUserID into usersgroup
                            from users in usersgroup.DefaultIfEmpty()
                            select new
                            {
                                inventory.intInventoryID,
                                inventory.intLocationID,
                                inventory.intCategoryID,
                                Category = categories.chrName,
                                inventory.intUnitTypeID,
                                units.chrUnitType,
                                intQuantity = inventory.intQuantity ?? 0,
                                inventory.dtmDateChanged,
                                inventory.intChangedByID,
                                inventory.bolBroken,
                                users.chrUser
                            };

                var detailsList = query.Where(d => d.intLocationID == Loc_ID && d.intCategoryID == Cat_ID)                             
                             .Select(s => new InventoryDetailModel
                             {
                                 InventoryID = s.intInventoryID,
                                 LocationID = s.intLocationID,
                                 CategoryID = s.intCategoryID,
                                 Category = s.Category,
                                 UnitTypeID = s.intUnitTypeID,
                                 UnitName = s.chrUnitType,
                                 Broken = s.bolBroken??false,
                                 DateChanged = s.dtmDateChanged,
                                 ChangedByID = s.intChangedByID,
                                 ChangedByName = s.chrUser,
                                 Quantity = s.intQuantity
                             })
                             .OrderBy(o => o.UnitName)
                             .ToList();


                ViewBag.UnitList = new SelectList(_inventory.UnitTypes.Where(u => u.intCategoryID == Cat_ID), "intUnitTypeID", "chrUnitType");

                return View(detailsList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult UpdateItem(int? ID, int? Quantity, bool Broken)
        {
            // Get current record
            var item = _inventory.Inventories.Find(ID);

            // Update values
            item.intQuantity = Quantity;
            item.dtmDateChanged = DateTime.Now;
            item.bolBroken = Broken;

            // Convert user Id to int
            if (Session["UserID"] != null && Int32.TryParse(Session["UserID"].ToString(), out int userId))
            {
                item.intChangedByID = userId;
            }
            else
            {
                return null;
            }

            // Save changes
            try
            {
                _inventory.SaveChanges();
            }
            catch
            {
                return null;
            }

            // Get updated record
            var query = from categories in _inventory.Categories
                        join inventories in _inventory.Inventories
                            on categories.intCategoryID equals inventories.intCategoryID into catinvgroup
                        from inventory in catinvgroup.DefaultIfEmpty()
                        join unittype in _inventory.UnitTypes
                            on inventory.intUnitTypeID equals unittype.intUnitTypeID into unittypegroup
                        from units in unittypegroup.DefaultIfEmpty()
                        join user in _inventory.Users
                            on inventory.intChangedByID equals user.intUserID into usersgroup
                        from users in usersgroup.DefaultIfEmpty()
                        select new
                        {
                            inventory.intInventoryID,
                            inventory.intLocationID,
                            inventory.intCategoryID,
                            Category = categories.chrName,
                            inventory.intUnitTypeID,
                            units.chrUnitType,
                            intQuantity = inventory.intQuantity ?? 0,
                            inventory.dtmDateChanged,
                            inventory.intChangedByID,
                            users.chrUser
                        };

            var inventoryRecord = query.Where(i => i.intInventoryID == ID)
                             .Select(s => new InventoryDetailModel
                             {
                                 InventoryID = s.intInventoryID,
                                 LocationID = s.intLocationID,
                                 CategoryID = s.intCategoryID,
                                 Category = s.Category,
                                 UnitTypeID = s.intUnitTypeID,
                                 UnitName = s.chrUnitType,
                                 DateChanged = s.dtmDateChanged,
                                 ChangedByID = s.intChangedByID,
                                 ChangedByName = s.chrUser,
                                 Quantity = s.intQuantity
                             })
                             .FirstOrDefault();

            // Convert to JSON and return
            return Json(inventoryRecord, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(int? Loc_ID)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.Location = Loc_ID;
                ViewBag.CategoryList = new SelectList(_inventory.Categories.ToList(), "intCategoryID", "chrName");
                ViewBag.UnitList = new SelectList(_inventory.UnitTypes.ToList(), "intUnitTypeID", "chrUnitType");

                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult AddItem(int? Loc_ID, int? Cat_ID, int? Unit_ID, int? Quantity, bool Broken)
        {
            try
            {
                var newUnit = new Inventory();
                newUnit.intLocationID = Loc_ID ?? 0;
                newUnit.intCategoryID = Cat_ID ?? 0;
                newUnit.intUnitTypeID = Unit_ID ?? 0;
                newUnit.intQuantity = Quantity;
                newUnit.bolBroken = Broken;
                newUnit.dtmDateChanged = DateTime.Now;

                if (Session["UserID"] != null && Int32.TryParse(Session["UserID"].ToString(), out int userId))
                {
                    newUnit.intChangedByID = userId;
                }
                else
                {
                    return null;
                }

                _inventory.Inventories.Add(newUnit);
                _inventory.SaveChanges();
            }
            catch
            {

            }

            //return RedirectToAction("Details", "Inventory", new { Loc_ID = Loc_ID, Cat_ID = Cat_ID });
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Details", "Inventory", new { Loc_ID = Loc_ID, Cat_ID = Cat_ID });
            return Json(new { Url = redirectUrl });
        }

        public ActionResult Delete(int id)
        {
            int? Loc_ID = null;
            int? Cat_ID = null;
            try
            {
                Inventory inventoryRecord = _inventory.Inventories.Find(id);
                Loc_ID = inventoryRecord.intLocationID;
                Cat_ID = inventoryRecord.intCategoryID;
                _inventory.Inventories.Remove(inventoryRecord);
                _inventory.SaveChanges();
            }
            catch (Exception e)
            {
                var test = e.Message;
            }

            return RedirectToAction("Details", "Inventory", new { Loc_ID = Loc_ID, Cat_ID = Cat_ID });
        }

        public ActionResult Search()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.CategoryList = new SelectList(_inventory.Categories.ToList(), "intCategoryID", "chrName");

                ViewBag.UnitList = new SelectList(_inventory.UnitTypes.ToList(), "intUnitTypeID", "chrUnitType");

                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult GetUnitList(int? Cat_ID)
        {
            var unitList = _inventory.UnitTypes.Where(u => u.intCategoryID == Cat_ID)
                .Select(u => new
                {
                    ID = u.intUnitTypeID,
                    Text = u.chrUnitType
                }
                ).ToList();

            // Convert to JSON and return
            return Json(unitList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnitLocations(int? Type_ID, int? Cat_ID)
        {
            var query = from inventories in _inventory.Inventories
                        join locations in _inventory.Locations
                            on inventories.intLocationID equals locations.intLocationID into invlocgroup
                        from location in invlocgroup.DefaultIfEmpty()
                        join unittype in _inventory.UnitTypes
                            on inventories.intUnitTypeID equals unittype.intUnitTypeID into unittypegroup
                        from units in unittypegroup.DefaultIfEmpty()
                        select new
                        {
                            inventories.intInventoryID,
                            inventories.intLocationID,
                            location.chrCity,
                            location.chrNickName,
                            location.chrStorageName,
                            inventories.intCategoryID,
                            inventories.intUnitTypeID,
                            units.chrUnitType,
                            intQuantity = inventories.intQuantity ?? 0,
                            inventories.bolBroken,
                            inventories.dtmDateChanged,
                            inventories.intChangedByID
                        };

            var unitList = query.Where(u => u.intUnitTypeID == Type_ID && u.bolBroken == false)
                .GroupBy(g => new { g.intLocationID, g.chrNickName, g.chrStorageName, g.intCategoryID, g.intUnitTypeID, g.chrUnitType })
                .Select(u => new InventorySearchModel
                {
                    LocationID = u.Key.intLocationID,
                    City = u.Key.chrNickName,
                    StorageName = u.Key.chrStorageName,
                    CategoryID = u.Key.intCategoryID,
                    UnitTypeID = u.Key.intUnitTypeID,
                    UnitName = u.Key.chrUnitType,
                    Quantity = u.Sum(i => i.intQuantity)
                })
                .ToList();

            // Get category name
            if (Cat_ID != null)
            {
                Category categoryRecord = _inventory.Categories.Find(Cat_ID);
                ViewBag.Category = categoryRecord.chrName;
            }
            else
            {
                // Use category from selected item
                Cat_ID = _inventory.Inventories.Where(i => i.intUnitTypeID == Type_ID).First().intCategoryID;
                Category categoryRecord = _inventory.Categories.Find(Cat_ID);
                ViewBag.Category = categoryRecord.chrName;
            }

            UnitType unitRecord = _inventory.UnitTypes.Find(Type_ID);
            ViewBag.UnitType = unitRecord.chrUnitType;

            ViewBag.EmptyList = unitList.Count();

            return PartialView("_SearchResults", unitList);
        }

        public virtual JsonResult FilteredUnitList(int? id)
        {
            var methodsList = _inventory.UnitTypes
                .Where(m => m.intCategoryID == id)
                .Select(x => new SelectListItem { Value = x.intUnitTypeID.ToString(), Text = x.chrUnitType })
                .ToList();

            // Convert list object to Json object
            //string jsonList = new JavaScriptSerializer().Serialize(DayList);
            return Json(methodsList, JsonRequestBehavior.AllowGet);
        }
    }
}