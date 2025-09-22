using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Dtos.Product;
using ProjectLaborBackend.Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectLaborBackend.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<List<ProductGetDTO>> GetAllProducts()
        {
            return await _productService.GetAllProductsAsync(); 
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetDTO>> GetProductById(int id)
        {
            try
            {
                return await _productService.GetProductByIdAsync(id);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }

        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult<ProductCreateDTO>> CreateProduct([FromBody] ProductCreateDTO product)
        {
            // TODO!!! Check if EAN already exists

            try
            {
                await _productService.CreateProductAsync(product);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok("Product created");
        }

        // PATCH api/<ProductController>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDTO product)
        {
            try
            {
                await _productService.UpdateProductAsync(id, product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Created();
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                await _productService.DeleteProductAsync(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }
    }
}
