
using AMS.Data;
using AMS.Models;
using AMS.Models.AssetViewModel;
using AMS.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AMS.Controllers
{
    [AllowAnonymous]
    public class AssetConsultController : Controller
    {
        private readonly string connectionString = "Server=SMI-SS-WEBSERV1;Database=AMSDev;User ID=serversmi;Password=adminserver123!;MultipleActiveResultSets=true;Persist Security Info=False";


        public AssetConsultController()
        {
         
        }


        public async Task<AssetConsultViewModel> QueryAssetConsult(int Id)
        {
            var procedure = "[GetAssetAllInfo]";
            using (var connection = new SqlConnection(connectionString))
            {
                var asset = await connection.QuerySingleOrDefaultAsync<Asset>(procedure , new
                {
                    IdAsset = Id
                }, commandType: CommandType.StoredProcedure);

                AssetConsultViewModel assetConsultViewModel = new AssetConsultViewModel();  
                assetConsultViewModel.Id = asset.Id;    
                assetConsultViewModel.Name = asset.Name;    
               



                return assetConsultViewModel;
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAssetConsult(int Id)
        {
            var AssetConsulted = await QueryAssetConsult(Id);
            return View(AssetConsulted);

        }
       
    }
}
