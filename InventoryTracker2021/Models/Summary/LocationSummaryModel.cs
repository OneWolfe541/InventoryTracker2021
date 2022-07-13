namespace InventoryTracker2021.Models
{
    using System;

    public class LocationSummaryModel
    {
        public int LocationID { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public int UnitTypeID { get; set; }
        public string UnitName { get; set; }
        public int Total { get; set; }
    }
}