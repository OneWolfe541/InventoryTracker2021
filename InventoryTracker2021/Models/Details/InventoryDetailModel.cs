namespace InventoryTracker2021.Models
{
    using System;

    public class InventoryDetailModel
    {
        public int InventoryID { get; set; }
        public int LocationID { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public int UnitTypeID { get; set; }
        public string UnitName { get; set; }
        public bool Broken { get; set; }
        public Nullable<System.DateTime> DateChanged { get; set; }
        public Nullable<int> ChangedByID { get; set; }
        public string ChangedByName { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}