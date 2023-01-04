using AMS.Data;
using AMS.Models;
using AMS.Models.AssetRequestViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AMS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AssetRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommon _iCommon;

        public AssetRequestController(ApplicationDbContext context, ICommon iCommon)
        {
            _context = context;
            _iCommon = iCommon;
        }

        [Authorize(Roles = Pages.MainMenu.AssetRequest.RoleName)]
        [HttpGet]
        public IActionResult Index()
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

                IQueryable<AssetRequestCRUDViewModel> _GetGridItem = GetGridItem(); ;
                //var _IsInRole = User.IsInRole("Admin");
                //var _UserProfile = _context.UserProfile.Where(x => x.Email == HttpContext.User.Identity.Name).SingleOrDefault();

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
                    || obj.AssetId.ToString().ToLower().Contains(searchValue)
                    || obj.RequestedEmployeeId.ToString().ToLower().Contains(searchValue)
                    || obj.ApprovedByEmployeeId.ToString().ToLower().Contains(searchValue)
                    || obj.RequestDetails.ToLower().Contains(searchValue)
                    || obj.Status.ToLower().Contains(searchValue)
                    || obj.RequestDate.ToString().ToLower().Contains(searchValue)

                    || obj.RequestDate.ToString().Contains(searchValue)
                    || obj.ReceiveDate.ToString().Contains(searchValue));
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
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            AssetRequestCRUDViewModel vm = await GetGridItem().Where(m => m.Id == id).SingleOrDefaultAsync();
            if (vm == null) return NotFound();
            return PartialView("_Details", vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            AssetRequestCRUDViewModel vm = new AssetRequestCRUDViewModel();
            ViewBag.LoadddlAssetName = new SelectList(_iCommon.GetCommonddlData("Asset"), "Id", "Name");
            ViewBag.LoadddlEmployee = new SelectList(_iCommon.LoadddlEmployee(), "Id", "Name");

            if (id > 0) vm = await _context.AssetRequest.Where(x => x.Id == id).SingleOrDefaultAsync();
            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(AssetRequestCRUDViewModel vm)
        {
            try
            {
                AssetRequest _AssetRequest = new AssetRequest();
                string _UserName = HttpContext.User.Identity.Name;
                if (vm.Id > 0)
                {
                    _AssetRequest = await _context.AssetRequest.FindAsync(vm.Id);

                    vm.CreatedDate = _AssetRequest.CreatedDate;
                    vm.CreatedBy = _AssetRequest.CreatedBy;
                    vm.ModifiedDate = DateTime.Now;
                    vm.ModifiedBy = _UserName;
                    _context.Entry(_AssetRequest).CurrentValues.SetValues(vm);
                    await _context.SaveChangesAsync();

                    var _AlertMessage = "Asset Request Updated Successfully. ID: " + _AssetRequest.Id;
                    return new JsonResult(_AlertMessage);
                }
                else
                {
                    _AssetRequest = vm;
                    _AssetRequest.CreatedDate = DateTime.Now;
                    _AssetRequest.ModifiedDate = DateTime.Now;
                    _AssetRequest.CreatedBy = _UserName;
                    _AssetRequest.ModifiedBy = _UserName;
                    _context.Add(_AssetRequest);
                    await _context.SaveChangesAsync();

                    var _AlertMessage = "Asset Request Created Successfully. ID: " + _AssetRequest.Id;
                    return new JsonResult(_AlertMessage);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new JsonResult(ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(Int64 id)
        {
            try
            {
                var _AssetRequest = await _context.AssetRequest.FindAsync(id);
                _AssetRequest.ModifiedDate = DateTime.Now;
                _AssetRequest.ModifiedBy = HttpContext.User.Identity.Name;
                _AssetRequest.Cancelled = true;

                _context.Update(_AssetRequest);
                await _context.SaveChangesAsync();
                return new JsonResult(_AssetRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<AssetRequestCRUDViewModel> GetGridItem()
        {
            try
            {
                var result = (from _AssetRequest in _context.AssetRequest
                              join _Asset in _context.Asset on _AssetRequest.AssetId equals _Asset.Id
                              join _RequestedEmployee in _context.Employee on _AssetRequest.RequestedEmployeeId equals _RequestedEmployee.Id
                              join _ApprovedByEmployee in _context.Employee on _AssetRequest.ApprovedByEmployeeId equals _ApprovedByEmployee.Id
                              where _AssetRequest.Cancelled == false
                              select new AssetRequestCRUDViewModel
                              {
                                  Id = _AssetRequest.Id,
                                  AssetId = _AssetRequest.AssetId,
                                  AssetDisplay = _Asset.Name,
                                  RequestedEmployeeId = _AssetRequest.RequestedEmployeeId,
                                  RequestedEmployeeDisplay = _RequestedEmployee.FirstName + " " + _RequestedEmployee.LastName,
                                  ApprovedByEmployeeId = _AssetRequest.ApprovedByEmployeeId,
                                  ApprovedByEmployeeDisplay = _ApprovedByEmployee.FirstName + " " + _ApprovedByEmployee.LastName,
                                  RequestDetails = _AssetRequest.RequestDetails,
                                  Status = _AssetRequest.Status,
                                  RequestDate = _AssetRequest.RequestDate,
                                  ReceiveDate = _AssetRequest.ReceiveDate,
                                  Comment = _AssetRequest.Comment,

                                  CreatedDate = _AssetRequest.CreatedDate,
                                  ModifiedDate = _AssetRequest.ModifiedDate,
                                  CreatedBy = _AssetRequest.CreatedBy,
                                  ModifiedBy = _AssetRequest.ModifiedBy,
                                  Cancelled = _AssetRequest.Cancelled,

                              }).OrderByDescending(x => x.Id);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
