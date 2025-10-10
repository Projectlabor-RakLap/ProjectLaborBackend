using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Services;
using ProjectLaborBackend.Dtos.StockChange;

namespace ProjectLaborBackend.Controllers
{
    [Route("api/stockchange")]
    [ApiController]
    public class StockChangeController : ControllerBase
    {
        private readonly IStockChangeService _service;

        public StockChangeController(IStockChangeService service)
        {
            _service = service;
        }

        // GET: api/StockChanges
        [HttpGet]
        public async Task<ActionResult<List<StockChangeGetDTO>>> GetStockAllChanges()
        {
           return await _service.GetAllStockChangeAsync();
        }

        // GET: api/StockChanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockChangeGetDTO>> GetStockChange(int id)
        {
            try
            {
                return await _service.GetStockChangeByIdAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("calculate-moving-average/{window}")]
        public async Task<ActionResult<double>> CalculateMovingAverage(int productId, int window)
        {
            try
            {
                double movingAverage = await _service.CalculateMovingAveragePriceAsync(productId, window);
                return Ok($"Moving average for a window size of {window}: " + movingAverage);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/StockChanges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchStockChange(int id, StockChangeUpdateDTO stockChange)
        {
            try
            {
                await _service.PatchStockChangesAsync(id, stockChange);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // POST: api/StockChanges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StockChange>> PostStockChange(StockChangeCreateDTO stockChange)
        {
            try
            {
                await _service.CreateStockChangeAsync(stockChange);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created();
        }

        // DELETE: api/StockChanges/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockChange(int id)
        {
            try
            {
                await _service.DeleteStockChangeAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpGet("warehouse-product/{productId}-{warehouseId}")]
        public async Task<List<StockChangeGetDTO>> GetAllStockchangeByWarehouseProduct(int productId, int warehouseId)
        {
            return await _service.GetStockChangesByProductAsync(productId, warehouseId);
        }

        [HttpGet("previous-week/{warehouse}")]
        public async Task<List<StockChangeGetDTO>> GetPreviousWeekStockChange(string warehouse)
        {
            return await _service.GetPreviousWeekSalesAsync(warehouse);
        }
    }
}
