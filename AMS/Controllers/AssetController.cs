using AMS.Data;
using AMS.Helpers;
using AMS.Models;
using AMS.Models.AssetHistoryViewModel;
using AMS.Models.AssetViewModel;
using AMS.Models.CommonViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public AssetController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.MainMenu.Asset.RoleName)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllIndexAsset()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetDataTabelData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnAscDesc = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int resultTotal = 0;

                var _GetGridItem = _iCommon.GetGridAssetList();
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnAscDesc)))
                {
                    _GetGridItem = _GetGridItem.OrderBy(sortColumn + " " + sortColumnAscDesc);
                }

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    _GetGridItem = _GetGridItem.Where(obj => obj.Id.ToString().Contains(searchValue)
                    || obj.AssetId.ToLower().Contains(searchValue)
                    || obj.AssetModelNo.ToLower().Contains(searchValue)
                    || obj.Name.ToLower().Contains(searchValue)
                    || obj.AssignEmployeeDisplay.ToLower().Contains(searchValue)
                    || obj.UnitPrice.ToString().Contains(searchValue)

                    || obj.DateOfPurchase.ToString().Contains(searchValue));
                }

                resultTotal = _GetGridItem.Count();

                var result = _GetGridItem.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = resultTotal, recordsTotal = resultTotal, data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<IActionResult> Details(Int64 id)
        {
            var _GetAssetInfo = await GetAssetInfo(id);
            return PartialView("_AllInfo", _GetAssetInfo);
        }
        [HttpGet]
        public async Task<IActionResult> PrintAsset(Int64 id)
        {
            var _GetAssetInfo = await GetAssetInfo(id);
            _GetAssetInfo.CompanyInfoCRUDViewModel = _iCommon.GetCompanyInfo();
            return View(_GetAssetInfo);
        }
        private async Task<AssetCRUDViewModel> GetAssetInfo(Int64 id)
        {
            try
            {
                AssetCRUDViewModel vm = new AssetCRUDViewModel();
                vm = await _iCommon.GetAssetList().Where(x => x.Id == id).SingleOrDefaultAsync();
                vm.EmployeeCRUDViewModel = await _iCommon.GetEmployeeList().Where(x => x.Id == vm.AssignEmployeeId).SingleOrDefaultAsync();
                vm.listAssetHistoryCRUDViewModel = await _iCommon.GetAssetHistoryList().Where(x => x.AssetId == vm.Id).ToListAsync();
                vm.listCommentCRUDViewModel = await _iCommon.GetCommentList(id).ToListAsync();

                return vm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            try
            {
                AssetCRUDViewModel vm = new AssetCRUDViewModel();
                ViewBag._LoadddlAssetCategorie = new SelectList(_iCommon.LoadddlAssetCategorie(), "Id", "Name");
                ViewBag._LoadddlAssetSubCategorie = new SelectList(_iCommon.LoadddlAssetSubCategorie(), "Id", "Name");
                ViewBag._LoadddlDepartment = new SelectList(_iCommon.LoadddlDepartment(), "Id", "Name");
                ViewBag._LoadddlSubDepartment = new SelectList(_iCommon.LoadddlSubDepartment(), "Id", "Name");

                ViewBag._LoadddlEmployee = new SelectList(_iCommon.LoadddlEmployee(), "Id", "Name");
                ViewBag._LoadddlSupplier = new SelectList(_iCommon.LoadddlSupplier(), "Id", "Name");
                ViewBag._LoadddlAssetStatus = new SelectList(_iCommon.LoadddlAssetStatus(), "Id", "Name");

                if (id > 0)
                {
                    vm = await _context.Asset.Where(x => x.Id == id).SingleOrDefaultAsync();
                    vm.listCommentCRUDViewModel = await _iCommon.GetCommentList(id).ToListAsync();
                    return PartialView("_Edit", vm);
                }
                else
                {
                    vm.AssetId = "AST-" + StaticData.RandomDigits(6);
                    return PartialView("_Add", vm);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(AssetCRUDViewModel vm)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
             {
                Asset _Asset = new();
                if (vm.Id > 0)
                {
                    _Asset = await _context.Asset.FindAsync(vm.Id);
                    if (vm.ImageURLDetails == null)
                    {
                        vm.ImageURL = _Asset.ImageURL;
                    }
                    else
                    {
                        vm.ImageURL = "/upload/" + _iCommon.UploadedFile(vm.ImageURLDetails);
                    }

                    var _AssetAllocationUpdate = await AssetAllocationUpdate(vm, _Asset);

                    vm.AssetStatus = _AssetAllocationUpdate;
                    vm.CreatedDate = _Asset.CreatedDate;
                    vm.CreatedBy = _Asset.CreatedBy;
                    vm.ModifiedDate = DateTime.Now;
                    vm.ModifiedBy = HttpContext.User.Identity.Name;
                    _context.Entry(_Asset).CurrentValues.SetValues(vm);
                    await _context.SaveChangesAsync();

                    _JsonResultViewModel.AlertMessage = "Asset Updated Successfully. ID: " + _Asset.Id;
                    _JsonResultViewModel.IsSuccess = true;
                    return new JsonResult(_JsonResultViewModel);
                }
                else
                {
                    _Asset = vm;
                    var _ImageURL = _iCommon.UploadedFile(vm.ImageURLDetails);
                    _ImageURL = _ImageURL != null ? _ImageURL : "blank-asset.png";

                    _Asset.ImageURL = "/upload/" + _ImageURL;
                    _Asset.CreatedDate = DateTime.Now;
                    _Asset.ModifiedDate = DateTime.Now;
                    _Asset.CreatedBy = HttpContext.User.Identity.Name;
                    _Asset.ModifiedBy = HttpContext.User.Identity.Name;
                    _context.Add(_Asset);
                    await _context.SaveChangesAsync();

                    await AddAssetHistory(_Asset.Id, _Asset.AssignEmployeeId, "Asset Created.");
                    if (vm.AssignEmployeeId != 0)
                    {
                        await AddAssetHistory(_Asset.Id, vm.AssignEmployeeId, "Unassigned Asset Assigned to Employee.");
                    }

                    _JsonResultViewModel.AlertMessage = "Asset Created Successfully. ID: " + _Asset.Id;
                    _JsonResultViewModel.CurrentURL = vm.CurrentURL;
                    _JsonResultViewModel.IsSuccess = true;
                    return new JsonResult(_JsonResultViewModel);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                _JsonResultViewModel.AlertMessage = ex.Message;
                return new JsonResult(_JsonResultViewModel);
                throw;
            }
        }

        private async Task<int> AssetAllocationUpdate(AssetCRUDViewModel vm, Asset _Asset)
        {
            int _AssetStatusValue = vm.AssetStatus;
            if (_Asset.AssignEmployeeId != vm.AssignEmployeeId)
            {
                if (_Asset.AssignEmployeeId == 0)
                {
                    await AddAssetHistory(_Asset.Id, vm.AssignEmployeeId, "Unassigned Asset Assigned to Employee.");
                    _AssetStatusValue = AssetStatusValue.InUse;
                }
                else
                {
                    if (vm.AssignEmployeeId == 0)
                    {
                        await AddAssetHistory(_Asset.Id, _Asset.AssignEmployeeId, "Asset Unassigned from Employee.");
                        _AssetStatusValue = AssetStatusValue.Available;
                    }
                    else
                    {
                        await AddAssetHistory(_Asset.Id, _Asset.AssignEmployeeId, "Asset Unassigned from Employee.");
                        await AddAssetHistory(_Asset.Id, vm.AssignEmployeeId, "Asset Assigned to Employee.");
                        _AssetStatusValue = AssetStatusValue.InUse;
                    }
                }
            }
            else
            {
                await AddAssetHistory(_Asset.Id, vm.AssignEmployeeId, "Asset Updated.");
                _AssetStatusValue = vm.AssetStatus;
            }

            return _AssetStatusValue;
        }

        private async Task AddAssetHistory(Int64 _AssetId, Int64 _AssignEmployeeId, string _Action)
        {
            AssetHistoryCRUDViewModel _AssetHistoryCRUDViewModel = new AssetHistoryCRUDViewModel
            {
                AssetId = _AssetId,
                AssignEmployeeId = _AssignEmployeeId,
                Action = _Action,
                UserName = HttpContext.User.Identity.Name
            };
            var result = await _iCommon.AddAssetHistory(_AssetHistoryCRUDViewModel);
        }        

        [HttpDelete]
        public async Task<IActionResult> Delete(Int64 id)
        {
            try
            {
                var _Asset = await _context.Asset.FindAsync(id);
                _Asset.ModifiedDate = DateTime.Now;
                _Asset.ModifiedBy = HttpContext.User.Identity.Name;
                _Asset.Cancelled = true;
                _context.Update(_Asset);
                await _context.SaveChangesAsync();

                await AddAssetHistory(_Asset.Id, _Asset.AssignEmployeeId, "Asset Deleted.");
                return new JsonResult(_Asset);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
