using System.ComponentModel.DataAnnotations;

namespace AMS.Models.AssetViewModel
{
    public class AssetConsultViewModel
    {
        [Display(Name = "SL")]
        [Required]
        public Int64 Id { get; set; }
        [Display(Name = "Serial Number")]
        [Required]
        public string AssetId { get; set; }
        [Display(Name = "Model")]
        public string AssetModelNo { get; set; }
        [Required]
        [Display(Name = "Name SMI")]
        public string Name { get; set; }

        public string Description { get; set; }
        public int Category { get; set; }
        public string CategoryDisplay { get; set; }
        [Display(Name = "Sub Category")]
        public int SubCategory { get; set; }
        public string SubCategoryDisplay { get; set; }
        public int? Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public double? UnitPrice { get; set; }
        public int Supplier { get; set; }
        public string SupplierDisplay { get; set; }
        [Display(Name = "US or MX Purchase")]
        public string Location { get; set; }
        public int Department { get; set; }
        public string DepartmentDisplay { get; set; }
        [Display(Name = "Sub Department")]
        public int SubDepartment { get; set; }
        public string SubDepartmentDisplay { get; set; }
        [Display(Name = "Warranety In Month")]
        public int? WarranetyInMonth { get; set; }
        [Display(Name = "Depreciation In Month")]
        public int? DepreciationInMonth { get; set; }
        [Display(Name = "Image")]
        public string ImageURL { get; set; } = "/upload/blank-asset.png";
        public IFormFile ImageURLDetails { get; set; }
        [Display(Name = "Purchase Receipt")]
        public string PurchaseReceipt { get; set; } = "/upload/blank_purchasereceipt.pdf";
        public IFormFile PurchaseReceiptDetails { get; set; }
        [Display(Name = "Date Of Purchase")]
        public DateTime DateOfPurchase { get; set; } = DateTime.Now;
        [Display(Name = "Receipt Date")]
        public DateTime DateOfManufacture { get; set; } = DateTime.Now;
        [Display(Name = "Year Of Valuation")]
        public DateTime YearOfValuation { get; set; } = DateTime.Now;
        [Display(Name = "Assign Employee")]
        public Int64 AssignEmployeeId { get; set; }
        public string AssignEmployeeDisplay { get; set; }
     
     
        public string AssetStatusDisplay { get; set; }
        [Display(Name = "Is Avilable")]
        public bool IsAvilable { get; set; }
        [Display(Name = "Invoice No.")]
        public string Note { get; set; }
        public string Barcode { get; set; }
        public string QRCode { get; set; }
        public string QRCodeImage { get; set; }
        [Display(Name = "Comment Message")]
        public string CommentMessage { get; set; }
        public string CurrentURL { get; set; }
        public bool IsAdmin { get; set; }
    }
}
