namespace InventoryTracker2021.Models
{
    using System;

    public class InventorySearchModel
    {
        public int LocationID { get; set; }
        public string City { get; set; }
        public string StorageName { get; set; }
        public int CategoryID { get; set; }
        public int UnitTypeID { get; set; }
        public string UnitName { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}