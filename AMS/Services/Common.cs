using Microsoft.EntityFrameworkCore;
using System.Net;
using UAParser;
using AMS.Data;
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
    public class Common : ICommon
    {
        private readonly IWebHostEnvironment _iHostingEnvironment;
        private readonly ApplicationDbContext _context;
        public Common(IWebHostEnvironment iHostingEnvironment, ApplicationDbContext context)
        {
            _iHostingEnvironment = iHostingEnvironment;
            _context = context;
        }
        public string UploadedFile(IFormFile ProfilePicture)
        {
            string ProfilePictureFileName = null;

            if (ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_iHostingEnvironment.ContentRootPath, "wwwroot/upload");

                if (ProfilePicture.FileName == null)
                    ProfilePictureFileName = Guid.NewGuid().ToString() + "_" + "blank-person.png";
                else
                    ProfilePictureFileName = Guid.NewGuid().ToString() + "_" + ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, ProfilePictureFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(fileStream);
                }
            }
            return ProfilePictureFileName;
        }

        public async Task<SMTPEmailSetting> GetSMTPEmailSetting()
        {
            return await _context.Set<SMTPEmailSetting>().Where(x => x.Id == 1).SingleOrDefaultAsync();
        }
        public async Task<SendGridSetting> GetSendGridEmailSetting()
        {
            return await _context.Set<SendGridSetting>().Where(x => x.Id == 1).SingleOrDefaultAsync();
        }

        public UserProfile GetByUserProfile(Int64 id)
        {
            return _context.UserProfile.Where(x => x.UserProfileId == id).SingleOrDefault();
        }
        public UserProfileCRUDViewModel GetByUserProfileInfo(Int64 id)
        {
            UserProfileCRUDViewModel _UserProfileCRUDViewModel = _context.UserProfile.Where(x => x.UserProfileId == id).SingleOrDefault();
            return _UserProfileCRUDViewModel;
        }
        public async Task<bool> InsertLoginHistory(LoginHistory _LoginHistory, ClientInfo _ClientInfo)
        {
            try
            {
                _LoginHistory.PublicIP = GetPublicIP();
                _LoginHistory.CreatedDate = DateTime.Now;
                _LoginHistory.ModifiedDate = DateTime.Now;

                _context.Add(_LoginHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetPublicIP()
        {
            try
            {
                string url = "http://checkip.dyndns.org/";
                WebRequest req = WebRequest.Create(url);
                WebResponse resp = req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }
        public IQueryable<ItemDropdownListViewModel> GetCommonddlData(string strTableName)
        {
            var sql = "select Id, Name from "+ strTableName +" where Cancelled = 0";
            var result = _context.ItemDropdownListViewModel.FromSqlRaw(sql);
            return result;
        }

        public IQueryable<ItemDropdownListViewModel> LoadddlDepartment()
        {
            return (from tblObj in _context.Department.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlSubDepartment()
        {
            return (from tblObj in _context.SubDepartment.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlAssetCategorie()
        {
            return (from tblObj in _context.AssetCategorie.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlAssetSubCategorie()
        {
            return (from tblObj in _context.AssetSubCategorie.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlSupplier()
        {
            return (from tblObj in _context.Supplier.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlAssetStatus()
        {
            return (from tblObj in _context.AssetStatus.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                    select new ItemDropdownListViewModel
                    {
                        Id = tblObj.Id,
                        Name = tblObj.Name,
                    });
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlEmployee()
        {
            var result = (from tblObj in _context.Employee.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                          select new ItemDropdownListViewModel
                          {
                              Id = tblObj.Id,
                              Name = tblObj.FirstName + " " + tblObj.LastName,
                          });
            return result;
        }
        public IQueryable<ItemDropdownListViewModel> LoadddlDesignation()
        {
            var result = (from tblObj in _context.Designation.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                          select new ItemDropdownListViewModel
                          {
                              Id = tblObj.Id,
                              Name = tblObj.Name,
                          });
            return result;
        }
        public IQueryable<AssetCRUDViewModel> GetGridAssetList()
        {
            try
            {
                return (from _Asset in _context.Asset
                        join _Employee in _context.Employee on _Asset.AssignEmployeeId equals _Employee.Id
                        into listEmployee
                        from _Employee in listEmployee.DefaultIfEmpty()
                        where _Asset.Cancelled == false
                        select new AssetCRUDViewModel
                        {
                            Id = _Asset.Id,
                            AssetId = _Asset.AssetId,
                            AssignEmployeeId = _Asset.AssignEmployeeId,
                            AssetModelNo = _Asset.AssetModelNo,
                            Name = _Asset.Name,
                            AssignEmployeeDisplay = _Asset.AssignEmployeeId == 0 ? "Unassigned" : _Employee.FirstName + " " + _Employee.LastName,
                            UnitPrice = _Asset.UnitPrice,
                            DateOfPurchase = _Asset.DateOfPurchase,
                            ImageURL = _Asset.ImageURL,
                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<AssetCRUDViewModel> GetAssetList()
        {
            try
            {
                return (from _Asset in _context.Asset
                        join _AssetCategorie in _context.AssetCategorie on _Asset.Category equals _AssetCategorie.Id
                        into listAssetCategorie
                        from _AssetCategorie in listAssetCategorie.DefaultIfEmpty()
                        join _AssetSubCategorie in _context.AssetSubCategorie on _Asset.SubCategory equals _AssetSubCategorie.Id
                        into listAssetSubCategorie
                        from _AssetSubCategorie in listAssetSubCategorie.DefaultIfEmpty()
                        join _Department in _context.Department on _Asset.Department equals _Department.Id
                        into listDepartment
                        from _Department in listDepartment.DefaultIfEmpty()
                        join _SubDepartment in _context.SubDepartment on _Asset.SubDepartment equals _SubDepartment.Id
                        into listSubDepartment
                        from _SubDepartment in listSubDepartment.DefaultIfEmpty()
                        join _Supplier in _context.Supplier on _Asset.Supplier equals _Supplier.Id
                        into listSupplier
                        from _Supplier in listSupplier.DefaultIfEmpty()
                        join _AssetStatus in _context.AssetStatus on _Asset.AssetStatus equals _AssetStatus.Id
                        into listAssetStatus
                        from _AssetStatus in listAssetStatus.DefaultIfEmpty()
                            //where _Asset.Cancelled == false
                        select new AssetCRUDViewModel
                        {
                            Id = _Asset.Id,
                            AssetId = _Asset.AssetId,
                            AssetModelNo = _Asset.AssetModelNo,
                            Name = _Asset.Name,
                            Description = _Asset.Description,
                            Category = _Asset.Category,
                            CategoryDisplay = _AssetCategorie.Name,
                            SubCategory = _Asset.SubCategory,
                            SubCategoryDisplay = _AssetSubCategorie.Name,
                            Quantity = _Asset.Quantity,
                            UnitPrice = _Asset.UnitPrice,
                            Supplier = _Asset.Supplier,
                            SupplierDisplay = _Supplier.Name,
                            Location = _Asset.Location,
                            Department = _Asset.Department,
                            DepartmentDisplay = _Department.Name,
                            SubDepartment = _Asset.SubDepartment,
                            SubDepartmentDisplay = _SubDepartment.Name,
                            WarranetyInMonth = _Asset.WarranetyInMonth,
                            DepreciationInMonth = _Asset.DepreciationInMonth,
                            ImageURL = _Asset.ImageURL,
                            DateOfPurchase = _Asset.DateOfPurchase,
                            DateOfManufacture = _Asset.DateOfManufacture,
                            YearOfValuation = _Asset.YearOfValuation,
                            AssignEmployeeId = _Asset.AssignEmployeeId,
                            AssetStatus = _Asset.AssetStatus,
                            AssetStatusDisplay = _AssetStatus.Name,
                            IsAvilable = _Asset.IsAvilable,
                            Note = _Asset.Note,
                            Barcode = _Asset.Barcode,
                            CreatedDate = _Asset.CreatedDate,
                            ModifiedDate = _Asset.ModifiedDate,
                            CreatedBy = _Asset.CreatedBy,
                            ModifiedBy = _Asset.ModifiedBy,
                            Cancelled = _Asset.Cancelled,
                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<EmployeeCRUDViewModel> GetEmployeeList()
        {
            try
            {
                return (from _Employee in _context.Employee
                        join _Department in _context.Department on _Employee.Department equals _Department.Id
                        //join _SubDepartment in _context.SubDepartment on _Employee.SubDepartment equals _SubDepartment.Id
                        join _Designation in _context.Designation on _Employee.Designation equals _Designation.Id
                        where _Employee.Cancelled == false
                        select new EmployeeCRUDViewModel
                        {
                            Id = _Employee.Id,
                            EmployeeId = _Employee.EmployeeId,
                            FirstName = _Employee.FirstName,
                            LastName = _Employee.LastName,
                            DateOfBirth = _Employee.DateOfBirth,
                            Designation = _Employee.Designation,
                            DesignationDisplay = _Designation.Name,
                            Department = _Employee.Department,
                            DepartmentDisplay = _Department.Name,

                            SubDepartment = _Employee.SubDepartment,
                            //SubDepartmentDisplay = _SubDepartment.Name,
                            JoiningDate = _Employee.JoiningDate,
                            LeavingDate = _Employee.LeavingDate,
                            Phone = _Employee.Phone,
                            Email = _Employee.Email,
                            Address = _Employee.Address,
                            CreatedDate = _Employee.CreatedDate,
                            ModifiedDate = _Employee.ModifiedDate,
                            CreatedBy = _Employee.CreatedBy,
                            ModifiedBy = _Employee.ModifiedBy,
                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<EmployeeCRUDViewModel> GetEmployeeGridList()
        {
            try
            {
                return (from _Employee in _context.Employee
                        join _Department in _context.Department on _Employee.Department equals _Department.Id
                        join _Designation in _context.Designation on _Employee.Designation equals _Designation.Id
                        where _Employee.Cancelled == false
                        select new EmployeeCRUDViewModel
                        {
                            Id = _Employee.Id,
                            EmployeeId = _Employee.EmployeeId,
                            FirstName = _Employee.FirstName,
                            LastName = _Employee.LastName,
                            DateOfBirth = _Employee.DateOfBirth,
                            Designation = _Employee.Designation,
                            DesignationDisplay = _Designation.Name,
                            Department = _Employee.Department,
                            DepartmentDisplay = _Department.Name,
                        }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AssetHistory> AddAssetHistory(AssetHistoryCRUDViewModel vm)
        {
            try
            {
                AssetHistory _AssetHistory = new AssetHistory();
                _AssetHistory = vm;
                _AssetHistory.CreatedDate = DateTime.Now;
                _AssetHistory.ModifiedDate = DateTime.Now;
                _AssetHistory.CreatedBy = vm.UserName;
                _AssetHistory.ModifiedBy = vm.UserName;
                _context.Add(_AssetHistory);
                await _context.SaveChangesAsync();

                return _AssetHistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<AssetHistoryCRUDViewModel> GetAssetHistoryList()
        {
            try
            {
                var result = (from _AssetHistory in _context.AssetHistory
                              join _Employee in _context.Employee on _AssetHistory.AssignEmployeeId equals _Employee.Id
                              into listEmployee
                              from _Employee in listEmployee.DefaultIfEmpty()
                              where _AssetHistory.Cancelled == false
                              select new AssetHistoryCRUDViewModel
                              {
                                  Id = _AssetHistory.Id,
                                  AssetId = _AssetHistory.AssetId,
                                  Action = _AssetHistory.Action,
                                  AssignEmployeeId = _AssetHistory.AssignEmployeeId,
                                  AssignEmployeeDisplay = _AssetHistory.AssignEmployeeId == 0 ? "Unassigned" : _Employee.FirstName + " " + _Employee.LastName,
                                  Note = _AssetHistory.Note,
                                  CreatedDate = _AssetHistory.CreatedDate,
                                  CreatedDateDisplay = String.Format("{0:f}", _AssetHistory.CreatedDate),
                              }).OrderByDescending(x => x.Id);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<CommentCRUDViewModel> GetCommentList()
        {
            try
            {
                return (from _Comment in _context.Comment
                        join _Asset in _context.Asset
                        on _Comment.AssetId equals _Asset.Id
                        select new CommentCRUDViewModel
                        {
                            Id = _Comment.Id,
                            AssetId = _Comment.AssetId,
                            AssetName = _Asset.Name,
                            Message = _Comment.Message,
                            IsDeleted = _Comment.IsDeleted,
                            IsDeletedDisplay = _Comment.IsDeleted == true ? "Yes" : "No",
                            CreatedDate = _Comment.CreatedDate,
                            ModifiedDate = _Comment.ModifiedDate,
                            CreatedBy = _Comment.CreatedBy,
                        }).OrderBy(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<CommentCRUDViewModel> GetCommentList(Int64 AssetId)
        {
            try
            {
                return (from _Comment in _context.Comment
                        join _Asset in _context.Asset
                        on _Comment.AssetId equals _Asset.Id
                        where _Comment.Cancelled == false && _Asset.Id == AssetId
                        select new CommentCRUDViewModel
                        {
                            Id = _Comment.Id,
                            AssetId = _Comment.AssetId,
                            AssetName = _Asset.Name,
                            Message = _Comment.Message,
                            IsDeleted = _Comment.IsDeleted,
                            CreatedDate = _Comment.CreatedDate,
                            ModifiedDate = _Comment.ModifiedDate,
                            CreatedBy = _Comment.CreatedBy,
                        }).OrderBy(x => x.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<BarcodeViewModel> GetBarcodeList()
        {
            var result = (from _Asset in _context.Asset
                          where _Asset.Cancelled == false
                          select new BarcodeViewModel
                          {
                              Id = _Asset.Id,
                              AssetName = _Asset.Name,
                              Barcode = _Asset.Barcode,
                          }).OrderBy(x => x.Id);
            return result;
        }

        public CompanyInfoCRUDViewModel GetCompanyInfo()
        {
            CompanyInfoCRUDViewModel vm = _context.CompanyInfo.FirstOrDefault(m => m.Id == 1);
            return vm;
        }
        public async Task<UpdateRoleViewModel> GetRoleByUser(string _ApplicationUserId, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            UpdateRoleViewModel _UpdateRoleViewModel = new UpdateRoleViewModel();
            List<ManageUserRolesViewModel> list = new List<ManageUserRolesViewModel>();

            _UpdateRoleViewModel.ApplicationUserId = _ApplicationUserId;
            var user = await _userManager.FindByIdAsync(_ApplicationUserId);
            if (user != null)
            {
                foreach (var role in _roleManager.Roles.ToList())
                {
                    var userRolesViewModel = new ManageUserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.Selected = true;
                    }
                    else
                    {
                        userRolesViewModel.Selected = false;
                    }
                    list.Add(userRolesViewModel);
                }
            }

            _UpdateRoleViewModel.listManageUserRolesViewModel = list.OrderBy(x => x.RoleName).ToList();
            return _UpdateRoleViewModel;
        }
    }
}