using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Dtos.Stock;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectLaborBackend.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        // GET: api/<StockController>
        [HttpGet]
        public async Task<List<StockGetDTO>> GetAllStocks()
        {
            return await _stockService.GetAllStocksAsync();
        }

        [HttpGet("get-stock-by-warehouse/{warehouseId}")]
        public async Task<List<StockGetWithProductDTO>> GetStocksByWarehouse(int warehouseId)
        {
            return await _stockService.GetStocksByWarehouseAsync(warehouseId);
        }

        // GET api/<StockController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockGetDTO>> GetStockById(int id)
        {
            try
            {
                return await _stockService.GetStockByIdAsync(id);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        // POST api/<StockController>
        [HttpPost]
        public async Task<ActionResult<StockCreateDTO>> CreateStock([FromBody] StockCreateDTO stock)
        {
            try
            {
                await _stockService.CreateStockAsync(stock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok("Stock created!");
        }

        // PUT api/<StockController>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto stock)
        {
            try
            {
                await _stockService.UpdateStockAsync(id, stock);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }

        // DELETE api/<StockController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _stockService.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            try
            {
                await _stockService.DeleteStockAsync(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        [HttpGet("product/{product}")]
        public async Task<ActionResult<StockGetDTO?>> GetStockByProduct(int product)
        {
            try
            {
                return await _stockService.GetStockByProductAsync(product);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
