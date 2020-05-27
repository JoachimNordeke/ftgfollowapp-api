using System.Threading.Tasks;
using API.Factories;
using API.Models.Stores;
using DocumentDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : BaseController
    {
        private readonly IReadRepository<Store> _readRepository;
        private readonly IWriteRepository<Store> _writeRepository;

        public StoresController()
        {
            _readRepository = DocumentDatabase.GetReadRepository<Store>();
            _writeRepository = DocumentDatabase.GetWriteRepository<Store>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stores = await _readRepository.GetAllAsync();

            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var store = await _readRepository.GetAsync(id);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewUpdateStore newStore)
        {
            var store = StoreFactory.CreateStore(newStore);

            await _writeRepository.CreateAsync(store);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] NewUpdateStore updateStore)
        {
            var store = await _readRepository.GetAsync(id);

            store = StoreFactory.UpdateStore(store, updateStore);

            var result = await _writeRepository.UpdateAsync(store);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _writeRepository.DeleteAsync(id);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}