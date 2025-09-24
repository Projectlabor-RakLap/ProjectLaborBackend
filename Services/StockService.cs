using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Dtos.Stock;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Services
{
    public interface IStockService
    {
        Task<List<StockGetDTO>> GetAllStocksAsync();
        Task<StockGetDTO?> GetStockByIdAsync(int id);
        Task CreateStockAsync(StockCreateDTO stock);
        Task UpdateStockAsync(int id, StockUpdateDto dto);
        Task DeleteStockAsync(int id);
    }

    public class StockService : IStockService
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public StockService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateStockAsync(StockCreateDTO stock)
        {
            if (stock.Currency.Length > 50)
            {
                throw new ArgumentOutOfRangeException("Currency cannot exceed 50 characters!");
            }

            if (stock.StockInStore > stock.StoreCapacity && stock.StockInWarehouse > stock.WarehouseCapacity)
            {
                throw new Exception("Stock in store and warehouse cannot exceed their capacities!");
            }

            var warehouse = await _context.Warehouses.FindAsync(stock.WarehouseId);
            if (warehouse == null)
            {
                throw new KeyNotFoundException("Warehouse with that id does not exist!");
            }

            var product = await _context.Products.FindAsync(stock.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Warehouse with that id does not exist!");
            }

            await _context.Stocks.AddAsync(_mapper.Map<Stock>(stock));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockAsync(int id)
        {
            Stock stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found");
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StockGetDTO>> GetAllStocksAsync()
        {
            return _mapper.Map<List<StockGetDTO>>(await _context.Stocks.ToListAsync());
        }

        public async Task<StockGetDTO?> GetStockByIdAsync(int id)
        {
            Stock stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found!");
            }

            return _mapper.Map<StockGetDTO>(stock);
        }

        public async Task UpdateStockAsync(int id, StockUpdateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("No data to be changed!");
            }

            if (dto.Currency != null && dto.Currency.Length > 50)
            {
                throw new ArgumentOutOfRangeException("Currency cannot exceed 50 characters!");
            }

            Stock stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found!");
            }

            //Validate store capacity
            if (dto.StoreCapacity != null && dto.StockInStore != null)
            {
                if (dto.StockInStore > dto.StoreCapacity)
                {
                    throw new Exception("Stock in store cannot exceed its capacity!");
                }
            }
            else if (dto.StoreCapacity != null)
            {
                if (stock.StockInStore > dto.StoreCapacity)
                {
                    throw new Exception("Stock capacity cannot exceed the stock in store!");
                }
            }
            else if (dto.StockInStore != null)
            {
                if (dto.StockInStore > stock.StoreCapacity)
                {
                    throw new Exception("Stock in store cannot exceed its capacity!");
                }
            }

            //Validate warehouse capacity
            if (dto.WarehouseCapacity != null && dto.StockInWarehouse != null)
            {
                if (dto.StockInWarehouse > dto.WarehouseCapacity)
                {
                    throw new Exception("Stock in warehouse cannot exceed its capacity!");
                }
            }
            else if (dto.WarehouseCapacity != null)
            {
                if (stock.StockInWarehouse > dto.WarehouseCapacity)
                {
                    throw new Exception("Stock capacity cannot exceed the stock in warehouse!");
                }
            }
            else if (dto.StockInWarehouse != null)
            {
                if (dto.StockInWarehouse > stock.WarehouseCapacity)
                {
                    throw new Exception("Stock in warehouse cannot exceed its capacity!");
                }
            }


            if (dto.WarehouseId != null)
            {
                var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == dto.WarehouseId);
                if (warehouse == null)
                {
                    throw new KeyNotFoundException($"Warehouse with {dto.WarehouseId} id does not exist!");
                }
            }


            if (dto.ProductId != null)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with {dto.ProductId} id does not exist!");
                }
            }

            _mapper.Map(dto, stock);
            stock.WarehouseId = dto.WarehouseId ?? stock.WarehouseId;
            stock.ProductId = dto.ProductId ?? stock.ProductId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }
    }
}
