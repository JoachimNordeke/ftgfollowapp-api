using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Factories;
using API.Models.Dashboard;
using API.Models.Sales;
using DocumentDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IReadRepository<Sale> _sales;
        public DashboardController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _sales = DocumentDatabase.GetReadRepository<Sale>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var factory = new DashboardFactory();

            return Ok(factory.Dashboard());
        }

        [HttpPost("dashboard-pinnar")]
        public async Task<IActionResult> GetPinnar([FromBody] DashboardFilter filter)
        {
            var storeId = GetStoreId();
            var fromDate = DateTime.Parse(filter.FromDate);
            var toDate = DateTime.Parse(filter.ToDate);

            var sales = await _sales.FindAsync(x => x.StoreId == storeId && x.SaleDate > fromDate && x.SaleDate < toDate);

            var list = new List<object>();

            foreach(var group in sales.GroupBy(x => x.SaleDate))
            {
                // DOES THIS WORK???
                list.Add(new { Date = group.Key, Count = group.SelectMany(x => x.Subscriptions).Count() });
            }

            return Ok(list);
        }

        [HttpPost("business-sales-data")]
        public async Task<IActionResult> GetBusinessSalesData([FromBody]DashboardFilter filter)
        {
            var fromDate = DateTime.Parse(filter.FromDate);
            var toDate = DateTime.Parse(filter.ToDate);

            ////// fake data ////////
            var random = new Random();

            var businessSalesData = new List<BusinessSalesData>();

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                businessSalesData.Add(new BusinessSalesData { Label = i.ToShortDateString(), Value = random.Next(5, 26) });
            }
            /////////////////////////

            return Ok(businessSalesData);
        }

        [HttpGet("business-sales-per-seller")]
        public async Task<IActionResult> GetBusinessSalesPerSellerData()
        {
            var businessPerSeller = new[]
            {
                new { Label = "Mimmi", Value = "23" },
                new { Label = "Fredrik", Value = "27" },
                new { Label = "Nathali", Value = "18" },
                new { Label = "Robin", Value = "12" }
            };

            return Ok(businessPerSeller);
        }

        private string GetStoreId()
        {
            return _accessor.HttpContext.User.Claims.Where(x => x.Type == "storeId").SingleOrDefault().Value;
        }
    }
}