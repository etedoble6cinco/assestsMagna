using AMS.Models.EmployeeViewModel;

namespace AMS.Models.AssetViewModel
{
    public class AssetDetailsViewModel : EntityBase
    {
        public AssetCRUDViewModel AssetCRUDViewModel { get; set; }
        public EmployeeCRUDViewModel EmployeeCRUDViewModel { get; set; }       
    }
}
