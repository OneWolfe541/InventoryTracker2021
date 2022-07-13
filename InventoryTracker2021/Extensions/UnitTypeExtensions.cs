using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using InventoryTracker2021.Models;

namespace InventoryTracker2021.Extensions
{
    public static class UnitTypeExtensions
    {
        public static IQueryable<T> CategoryEquals<T>(this IQueryable<T> queryable, int? intID) where T : UnitType
        {
            if (intID == null || queryable == null) return queryable;
            else
                return queryable.Where(arg => arg.intCategoryID == intID);
        }

        //public static IQueryable<UnitType> CategoryEquals<T>(this DbSet<UnitType> queryable, int? intID) where T : UnitType
        //{
        //    if (intID == null || queryable == null) return queryable;
        //    else
        //        return queryable.Where(q => q.intCategoryID == intID);
        //}
    }
}