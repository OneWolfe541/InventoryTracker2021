namespace InventoryTracker2021.Models
{
    using System;

    public class PasswordResetModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
    }
}