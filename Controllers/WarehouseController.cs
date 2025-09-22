using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Dtos.Warehouse;
using ProjectLaborBackend.Services;

namespace ProjectLaborBackend.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<IEnumerable<WarehouseGetDTO>>> GetWarehouses()
        {
            return await _warehouseService.GetAllWarehousesAsync();
        }

        // GET: api/WarehousesController2/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseGetDTO>> GetWarehouse(int id)
        {
            return await _warehouseService.GetWarehouseByIdAsync(id);
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Created");
        }

        // DELETE: api/WarehousesController/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }   
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

            return NoContent();
            }

        private bool WarehouseExists(int id)
        {
            return _warehouseService.GetWarehouseByIdAsync(id) != null;
        }
    }
}
