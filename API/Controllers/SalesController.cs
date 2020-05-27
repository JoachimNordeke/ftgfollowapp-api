using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Factories;
using API.Models.Companies;
using API.Models.Products;
using API.Models.Sales;
using API.Models.Users;
using DocumentDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : BaseController
    {
        private readonly IHttpContextAccessor _accessor;

        private readonly IReadRepository<User> _userRepository;
        private readonly IReadRepository<Company> _companyRepository;
        private readonly IReadRepository<Product> _productRepository;
        private readonly IWriteRepository<Sale> _saleWriteRepository;

        public SalesController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            _userRepository = DocumentDatabase.GetReadRepository<User>();
            _companyRepository = DocumentDatabase.GetReadRepository<Company>();
            _productRepository = DocumentDatabase.GetReadRepository<Product>();
            _saleWriteRepository = DocumentDatabase.GetWriteRepository<Sale>();
        }

        [HttpPost]
        [Authorize(Roles = "seller,storemanager,admin")]
        public async Task<IActionResult> NewSale([FromBody] NewUpdateSale newSale)
        {
            var storeId = GetStoreId();
            var totalCommission = await CalculateCommission(newSale);

            var sale = SaleFactory.CreateSale(storeId, totalCommission, newSale);

            try
            {
            await _saleWriteRepository.CreateAsync(sale);

            }
            catch (Exception e)
            {
                var h = e.Message;
            }

            return Ok();
        }

        [HttpGet("get-new-sale-data")]
        public async Task<IActionResult> GetNewSaleData()
        {
            var storeId = GetStoreId();

            var users = (await _userRepository.FindAsync(x => (x.MainStoreId == storeId || x.StoreIds.Contains(storeId)) && (x.Role == "seller" || x.Role == "storemanager"))).Select(x => new { x.Id, x.Firstname });

            var companies = (await _companyRepository.FindAsync(x => x.StoreId == storeId)).Select(x => new { x.Id, x.Name });

            var products = await _productRepository.GetAllAsync();

            var hardwares = products.Where(x => x.Type == "Hårdvara").Select(x => new { x.Id, x.Title });
            var subscriptions = products.Where(x => x.Type == "Abonnemang").Select(x => new { x.Id, x.Title });
            var extras = products.Where(x => x.Type == "Tilläggstjänst").Select(x => new { x.Id, x.Title });

            var data = new { sellers = users, companies, hardwares, subscriptions, extras };

            return Ok(data);
        }

        private string GetStoreId()
        {
            return _accessor.HttpContext.User.Claims.Where(x => x.Type == "storeId").SingleOrDefault().Value;
        }

        private async Task<int> CalculateCommission(NewUpdateSale sale)
        {
            int commission = 0;

            var hwIds = sale.ChosenHardwares.Select(x => x.Id);
            var exIds = sale.ChosenExtras.Select(x => x.Id);
            var subIds = sale.ChosenSubscriptions.Select(x => x.Id);

            var hardwares = await _productRepository.FindAsync(x => hwIds.Contains(x.Id));
            var extras = await _productRepository.FindAsync(x => exIds.Contains(x.Id));
            var subscriptions = await _productRepository.FindAsync(x => subIds.Contains(x.Id));

            foreach(var hardware in hardwares)
            {
                commission += hardware.Commission * sale.ChosenHardwares.SingleOrDefault(x => x.Id == hardware.Id).Amount;
            }

            foreach(var extra in extras)
            {
                commission += extra.Commission * sale.ChosenExtras.SingleOrDefault(x => x.Id == extra.Id).Amount;
            }

            foreach (var subscription in subscriptions)
            {
                commission += subscription.Commission;
            }

            return commission;
        }
    }
}