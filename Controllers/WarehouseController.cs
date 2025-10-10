using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Dtos.Warehouse;
using ProjectLaborBackend.Services;
using System.Threading.Tasks;

namespace ProjectLaborBackend.Controllers
{
    [Route("api/warehouse")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private IWarehouseService _warehouseService;
        
        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET: api/WarehousesController
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseGetDTO>>> GetAllWarehouses()
        {
            return await _warehouseService.GetAllWarehousesAsync();
        }

        // GET: api/WarehousesController2/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseGetDTO>> GetWarehouseById(int id)
        {
            try
            {
                return await _warehouseService.GetWarehouseByIdAsync(id);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/WarehousesController/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, WarehouseUpdateDTO warehouse)
        {
            try
            {
                await _warehouseService.PatchWarehouseAsync(id, warehouse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        // POST: api/WarehousesController
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Warehouse>> PostWarehouse([FromBody] WarehousePostDTO warehouse)
        {
            try
            {
                await _warehouseService.CreateWarehouseAsync(warehouse);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created();
        }

        // DELETE: api/WarehousesController/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            try
            {
                await _warehouseService.DeleteWarehouseAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        private bool WarehouseExists(int id)
        {
            return _warehouseService.GetWarehouseByIdAsync(id) != null;
        }
    }
}
