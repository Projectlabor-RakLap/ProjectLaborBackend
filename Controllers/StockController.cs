using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Dtos.Stock;
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

        // GET api/<StockController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StockController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<StockController>/5
        [HttpPatch("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<StockController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
