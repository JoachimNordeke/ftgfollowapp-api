using System.Threading.Tasks;
using API.Factories;
using API.Models.Products;
using DocumentDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BaseController
    {
        private readonly IWriteRepository<Product> _writeRepository;
        private readonly IReadRepository<Product> _readRepository;

        public ProductsController()
        {
            _writeRepository = DocumentDatabase.GetWriteRepository<Product>();
            _readRepository = DocumentDatabase.GetReadRepository<Product>();
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _readRepository.GetAllAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var product = await _readRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] NewUpdateProduct newProduct)
        {
            var product = ProductFactory.CreateProduct(newProduct);

            await _writeRepository.CreateAsync(product);

            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(string id, [FromBody] NewUpdateProduct updateProduct)
        {
            var product = await _readRepository.GetAsync(id);

            product = ProductFactory.UpdateProduct(product, updateProduct);

            var result = await _writeRepository.UpdateAsync(product);

            if (!result.IsAcknowledged)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
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