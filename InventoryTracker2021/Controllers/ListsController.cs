using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryTracker2021.Context;
using InventoryTracker2021.Extensions;
using InventoryTracker2021.Models;

namespace InventoryTracker2021.Controllers
{
    public class ListsController : Controller
    {
        private InventoryContext _inventory = new InventoryContext();

        // GET: Lists
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            if (Session["UserID"] != null)
            {
                var categoryList = _inventory.Categories.ToList();
                return View(categoryList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult UpdateCategory(int? ID, string Name)
        {
            // Get current record
            var category = _inventory.Categories.Find(ID);

            category.chrName = Name;

            // Save changes
            try
            {
                _inventory.SaveChanges();
            }
            catch
            {
                return null;
            }

            // Convert to JSON and return
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCategory(string Name)
        {
            try
            {
                Category newCategory = new Category();

                newCategory.chrName = Name;

                _inventory.Categories.Add(newCategory);

                _inventory.SaveChanges();

                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Categories", "Lists");
                return Json(new { Url = redirectUrl });
            }
            catch
            {
                return null;
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var removeCategory = _inventory.Categories.Find(id);

                _inventory.Categories.Remove(removeCategory);

                _inventory.SaveChanges();
            }
            catch
            {

            }

            return RedirectToAction("Categories", "Lists");
        }

        public ActionResult UnitTypes(int? Cat_ID)
        {
            if (Session["UserID"] != null)
            {
                var unitList = _inventory.UnitTypes
                    .CategoryEquals(Cat_ID)
                    .OrderBy(o => o.intCategoryID)
                    .ThenBy(o => o.chrUnitType)
                    .ToList();

                ViewBag.CategoryList = new SelectList(_inventory.Categories.ToList(), "intCategoryID", "chrName", Cat_ID);
                ViewBag.CategoryId = Cat_ID;

                return View(unitList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult UpdateUnit(int? ID, int Cat_ID, string Name, string Desc)
        {
            // Get current record
            var unit = _inventory.UnitTypes.Find(ID);

            unit.intCategoryID = Cat_ID;
            unit.chrUnitType = Name;
            unit.chrDescription = Desc;

            // Save changes
            try
            {
                _inventory.SaveChanges();
            }
            catch
            {
                return null;
            }

            // Convert to JSON and return
            return Json(unit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUnit(int? ID, int Cat_ID, string Name, string Desc)
        {
            try
            {
                UnitType newUnit = new UnitType();

                newUnit.intCategoryID = Cat_ID;
                newUnit.chrUnitType = Name;
                newUnit.chrDescription = Desc;

                _inventory.UnitTypes.Add(newUnit);

                _inventory.SaveChanges();

                if (ID == null)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("UnitTypes", "Lists");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("UnitTypes", "Lists", new { Cat_ID = ID });
                    return Json(new { Url = redirectUrl });
                }
            }
            catch
            {
                return null;
            }
        }
    }
}