using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryTracker2021.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InventoryTracker2021.Context
{
    public partial class InventoryContext : DbContext
    {

        public InventoryContext() : base("name=StorageInventory")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LookUp_State> LookUp_States { get; set; }
        public virtual DbSet<UnitType> UnitTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // https://stackoverflow.com/questions/12130059/how-turn-off-pluralize-table-creation-for-entity-framework-5
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Category>().HasKey(c => new { c.intCategoryID });
            modelBuilder.Entity<Inventory>().HasKey(c => new { c.intInventoryID });
            modelBuilder.Entity<Location>().HasKey(c => new { c.intLocationID });
            modelBuilder.Entity<LookUp_State>().HasKey(c => new { c.chrState });
            modelBuilder.Entity<UnitType>().HasKey(c => new { c.intUnitTypeID });
            modelBuilder.Entity<User>().HasKey(c => new { c.intUserID });
        }
    }
}
