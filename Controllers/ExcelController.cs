using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Controllers;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Services;

namespace ProjectLaborBackend.Controllers
{
    [Route("api/excel")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        CsvOperations csvOperations;
        private IProductService productService;
        private IStockChangeService stockChangeService;
        private IStockService stockService;
        private IWarehouseService warehouseService;

        public ExcelController(IProductService productService, IStockChangeService stockChangeService, IWarehouseService warehouseService, IStockService stockService)
        {
            this.productService = productService;
            this.stockChangeService = stockChangeService;
            this.warehouseService = warehouseService;
            this.stockService = stockService;

            csvOperations = new CsvOperations();
        }

        [HttpPost("export")]
        public IActionResult ExportDataToExcel(AppDbContext.Tables table)
        {
            try
            {
                csvOperations.ExportDataToExcel(table);
            }
            catch (IOException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest("Database error: " + e.Message);
            }
            return Ok("Exported");
        }

        [HttpPost("import")]
        public IActionResult ImportDataFromExcel(AppDbContext.Tables table)
        {
            List<List<string>> _dataFromExcel = new List<List<string>>();
            try 
            {
                _dataFromExcel = csvOperations.ImportDataFromExcel(table);
            }
            catch (FileNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (IOException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest("Database error: " + e.Message);
            }

            if (_dataFromExcel.Count == 0)
                return BadRequest("No data found in the Excel file.");

            try
            {
                switch (table)
                {
                    case AppDbContext.Tables.Products:
                        productService.InsertOrUpdate(_dataFromExcel);
                        break;

                    case AppDbContext.Tables.StockChanges:
                        stockChangeService.InsertOrUpdate(_dataFromExcel);
                        break;

                    case AppDbContext.Tables.Warehouses:
                        warehouseService.InsertOrUpdate(_dataFromExcel);
                        break;

                    case AppDbContext.Tables.Stocks:
                        stockService.InsertOrUpdate(_dataFromExcel);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(table), "Unsupported table for import");
                }
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest($"{e.Message}, {e.StackTrace}");// "Database error: " + e.Message);
            }
            return Ok("Imported");
        }

    }
}
