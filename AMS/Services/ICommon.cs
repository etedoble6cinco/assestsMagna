using UAParser;
using AMS.Models;
using AMS.Models.CommonViewModel;
using AMS.Models.AssetViewModel;
using AMS.Models.EmployeeViewModel;
using AMS.Models.AssetHistoryViewModel;
using AMS.Models.UserAccountViewModel;
using AMS.Models.CommentViewModel;
using AMS.Models.CompanyInfoViewModel;
using Microsoft.AspNetCore.Identity;

namespace AMS.Services
{
    public interface ICommon
    {
        string UploadedFile(IFormFile ProfilePicture);
        Task<SMTPEmailSetting> GetSMTPEmailSetting();
        Task<SendGridSetting> GetSendGridEmailSetting();
        UserProfile GetByUserProfile(Int64 id);
        UserProfileCRUDViewModel GetByUserProfileInfo(Int64 id);
        Task<bool> InsertLoginHistory(LoginHistory _LoginHistory, ClientInfo _ClientInfo);
        IQueryable<ItemDropdownListViewModel> GetCommonddlData(string strTableName);
        IQueryable<ItemDropdownListViewModel> LoadddlDepartment();
        IQueryable<ItemDropdownListViewModel> LoadddlSubDepartment();
        IQueryable<ItemDropdownListViewModel> LoadddlAssetCategorie();
        IQueryable<ItemDropdownListViewModel> LoadddlAssetSubCategorie();
        IQueryable<ItemDropdownListViewModel> LoadddlSupplier();
        IQueryable<ItemDropdownListViewModel> LoadddlAssetStatus();
        IQueryable<ItemDropdownListViewModel> LoadddlEmployee();
        IQueryable<ItemDropdownListViewModel> LoadddlDesignation();
        IQueryable<AssetCRUDViewModel> GetAssetList();
        IQueryable<AssetCRUDViewModel> GetGridAssetList();
        IQueryable<EmployeeCRUDViewModel> GetEmployeeList();
        IQueryable<EmployeeCRUDViewModel> GetEmployeeGridList();
        Task<AssetHistory> AddAssetHistory(AssetHistoryCRUDViewModel vm);
        IQueryable<AssetHistoryCRUDViewModel> GetAssetHistoryList();
        IQueryable<CommentCRUDViewModel> GetCommentList();
        IQueryable<CommentCRUDViewModel> GetCommentList(Int64 AssetId);
        IQueryable<BarcodeViewModel> GetBarcodeList();
        CompanyInfoCRUDViewModel GetCompanyInfo();
        Task<UpdateRoleViewModel> GetRoleByUser(string _ApplicationUserId, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager);
    }
}
