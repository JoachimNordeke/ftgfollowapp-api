using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Factories;
using API.Models.Companies;
using DocumentDb;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : BaseController
    {
        private readonly IHttpContextAccessor _accessor;

        private readonly IReadRepository<Company> _readRepository;
        private readonly IWriteRepository<Company> _writeRepository;

        public CompaniesController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            _readRepository = DocumentDatabase.GetReadRepository<Company>();
            _writeRepository = DocumentDatabase.GetWriteRepository<Company>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var storeId = GetStoreId();

            var companies = await _readRepository.FindAsync(x => x.StoreId == storeId);

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var company = await _readRepository.GetAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpPost]
        [Authorize(Roles = "seller,manager,admin")]
        public async Task<IActionResult> Create([FromBody] NewUpdateCompany newCompany)
        {
            newCompany.StoreId = GetStoreId();

            var company = CompanyFactory.CreateCompany(newCompany);

            await _writeRepository.CreateAsync(company);

            return Ok(company);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "seller,manager,admin")]
        public async Task<IActionResult> Update(string id, [FromBody] NewUpdateCompany updateCompany)
        {
            var company = await _readRepository.GetAsync(id);

            company = CompanyFactory.UpdateCompany(company, updateCompany);

            var result = await _writeRepository.UpdateAsync(company);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "seller,manager,admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _writeRepository.DeleteAsync(id);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }

        private string GetStoreId()
        {
            return _accessor.HttpContext.User.Claims.Where(x => x.Type == "storeId").SingleOrDefault().Value;
        }
    }
}