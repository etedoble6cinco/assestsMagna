using AMS.Models.AssetHistoryViewModel;
using AMS.Models.CommentViewModel;
using AMS.Models.CompanyInfoViewModel;
using AMS.Models.EmployeeViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models.AssetViewModel
{
    public class AssetCRUDViewModel : EntityBase
    {
        [Display(Name = "Identificador Unico")]
        [Required]
        public Int64 Id { get; set; }
        [Display(Name = "Numero Serie")]
        [Required]
        public string AssetId { get; set; }
        [Display(Name = "Modelo")]
        public string AssetModelNo { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Name { get; set; }
        [Display(Name="Nombre SMI")]
        public string Description { get; set; }
        [Display(Name = "Categoria")]
        public int Category { get; set; }
  
        public string CategoryDisplay { get; set; }
        [Display(Name = "Sub Categoria")]
        public int SubCategory { get; set; }
     
        public string SubCategoryDisplay { get; set; }
        [Display(Name = "Cantidad")]
        public int? Quantity { get; set; }
        [Display(Name = "Precio")]
        public double? UnitPrice { get; set; }
        [Display(Name = "Marca")]
        public int Supplier { get; set; }
     
        public string SupplierDisplay { get; set; }
        [Display(Name = "Ubicacion")]
        public string Location { get; set; }
        [Display(Name = "Departamento")]
        public int Department { get; set; }
        
        public string DepartmentDisplay { get; set; }
        [Display(Name = "Administrado por")]
        public int SubDepartment { get; set; }
        public string SubDepartmentDisplay { get; set; }
        [Display(Name = "Mantenimiento")]
        public int? WarranetyInMonth { get; set; }
        [Display(Name = "Ultima revision")]
        public int? DepreciationInMonth { get; set; }
        [Display(Name = "Imagen")]
        public string ImageURL { get; set; } = "/upload/blank-asset.png";
        public IFormFile ImageURLDetails { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime DateOfPurchase { get; set; } = DateTime.Now;
        [Display(Name = "Fecha de Llegada")]
        public DateTime DateOfManufacture { get; set; } = DateTime.Now;
        [Display(Name = "Año de valoracion")]
        public DateTime YearOfValuation { get; set; } = DateTime.Now;
        [Display(Name = "Persona Responsable")]
        public Int64 AssignEmployeeId { get; set; }
        public string AssignEmployeeDisplay { get; set; }
        [Display(Name = "Estatus")]
        public int AssetStatus { get; set; }
        public string AssetStatusDisplay { get; set; }
        [Display(Name = "Disponibilidad")]
        public bool IsAvilable { get; set; }
        [Display(Name = "Notas")]
        public string Note { get; set; }
        [Display(Name ="Codigo de Barras")]
        public string Barcode { get; set; }
        [Display(Name = "Observaciones")]
        public string CommentMessage { get; set; }
        
        public string CurrentURL { get; set; }
        public EmployeeCRUDViewModel EmployeeCRUDViewModel { get; set; }
        public List<AssetHistoryCRUDViewModel> listAssetHistoryCRUDViewModel { get; set; }
        public List<CommentCRUDViewModel> listCommentCRUDViewModel { get; set; }
        public CompanyInfoCRUDViewModel CompanyInfoCRUDViewModel { get; set; }

        public static implicit operator AssetCRUDViewModel(Asset _Asset)
        {
            return new AssetCRUDViewModel
            {
                Id = _Asset.Id,
                AssetId = _Asset.AssetId,
                AssetModelNo = _Asset.AssetModelNo,
                Name = _Asset.Name,
                Description = _Asset.Description,
                Category = _Asset.Category,
                SubCategory = _Asset.SubCategory,
                Quantity = _Asset.Quantity,
                UnitPrice = _Asset.UnitPrice,
                Supplier = _Asset.Supplier,
                Location = _Asset.Location,
                Department = _Asset.Department,
                SubDepartment = _Asset.SubDepartment,
                WarranetyInMonth = _Asset.WarranetyInMonth,
                DepreciationInMonth = _Asset.DepreciationInMonth,
                ImageURL = _Asset.ImageURL,
                DateOfPurchase = _Asset.DateOfPurchase,
                DateOfManufacture = _Asset.DateOfManufacture,
                YearOfValuation = _Asset.YearOfValuation,
                AssignEmployeeId = _Asset.AssignEmployeeId,
                AssetStatus = _Asset.AssetStatus,
                IsAvilable = _Asset.IsAvilable,
                Note = _Asset.Note,
                Barcode=_Asset.Barcode,
                CreatedDate = _Asset.CreatedDate,
                ModifiedDate = _Asset.ModifiedDate,
                CreatedBy = _Asset.CreatedBy,
                ModifiedBy = _Asset.ModifiedBy,
                Cancelled = _Asset.Cancelled,
            };
        }

        public static implicit operator Asset(AssetCRUDViewModel vm)
        {
            return new Asset
            {
                Id = vm.Id,
                AssetId = vm.AssetId,
                AssetModelNo = vm.AssetModelNo,
                Name = vm.Name,
                Description = vm.Description,
                Category = vm.Category,
                SubCategory = vm.SubCategory,
                Quantity = vm.Quantity,
                UnitPrice = vm.UnitPrice,
                Supplier = vm.Supplier,
                Location = vm.Location,
                Department = vm.Department,
                SubDepartment = vm.SubDepartment,
                WarranetyInMonth = vm.WarranetyInMonth,
                DepreciationInMonth = vm.DepreciationInMonth,
                ImageURL = vm.ImageURL,
                DateOfPurchase = vm.DateOfPurchase,
                DateOfManufacture = vm.DateOfManufacture,
                YearOfValuation = vm.YearOfValuation,
                AssignEmployeeId = vm.AssignEmployeeId,
                AssetStatus = vm.AssetStatus,
                IsAvilable = vm.IsAvilable,
                Note = vm.Note,
                Barcode = vm.Barcode,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
