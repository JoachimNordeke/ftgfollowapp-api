using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DocumentDb;
using API.Factories;
using API.Models.Users;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : BaseController
    {
        private readonly IHttpContextAccessor _accessor;

        private readonly IReadRepository<User> _readRepository;
        private readonly IWriteRepository<User> _writeRepository;

        public SellersController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            _readRepository = DocumentDatabase.GetReadRepository<User>();
            _writeRepository = DocumentDatabase.GetWriteRepository<User>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var storeId = GetStoreId();

            var sellers = (await _readRepository
                .FindAsync(x => (x.MainStoreId == storeId || x.StoreIds.Contains(storeId)) && x.Role == "seller" || x.Role == "storemanager"))
                .ToList()
                .Select(x => UserFactory.CreateUserDTOFromUser(x));

            return Ok(sellers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var seller = await _readRepository.GetAsync(id);

            if (seller == null)
            {
                return NotFound();
            }

            return Ok(UserFactory.CreateUserDTOFromUser(seller));
        }

        [HttpPost]
        [Authorize(Roles = "manager,regional,admin")]
        public async Task<IActionResult> Create([FromBody] NewUpdateUser newSeller)
        {
            var seller = UserFactory.CreateUser(newSeller);
            seller.MainStoreId = GetStoreId();
            seller.Role = "seller";

            await _writeRepository.CreateAsync(seller);

            return Ok(UserFactory.CreateUserDTOFromUser(seller));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "manager,regional,admin")]
        public async Task<IActionResult> Update(string id, [FromBody] NewUpdateUser updateSeller)
        {
            var seller = await _readRepository.GetAsync(id);

            seller = UserFactory.UpdateUser(seller, updateSeller);

            var result = await _writeRepository.UpdateAsync(seller);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "manager,regional,admin")]
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