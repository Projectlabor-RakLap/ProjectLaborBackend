using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Dtos.Stock;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Services
{
    public interface IStockService
    {
        Task<List<StockGetDTO>> GetAllStocksAsync();
        Task<List<StockGetWithProductDTO>> GetStocksByWarehouseAsync(int warehouseId);
        Task<StockGetDTO?> GetStockByIdAsync(int id);
        Task CreateStockAsync(StockCreateDTO stock);
        Task UpdateStockAsync(int id, StockUpdateDto dto);
        Task DeleteStockAsync(int id);
        void InsertOrUpdate(List<List<string>> data);
        Task<StockGetDTO?> GetStockByProductAsync(int productId);
        Task UpdateStockAfterStockChange(int stockId, int warehouseId, int quantity);
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

            if (stock.StockInWarehouse < 0 || stock.StockInStore < 0)
            {
                throw new ArgumentOutOfRangeException("Stock cannot be negative!");
            }

            if (stock.WarehouseCapacity <= 0 || stock.StoreCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException("Capacity cannot be equal or less than 0!");
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
            return _mapper.Map<List<StockGetDTO>>(await _context.Stocks.Include(p => p.Product).Include(w => w.Warehouse).ToListAsync());
        }

        public async Task<StockGetDTO?> GetStockByIdAsync(int id)
        {
            Stock? stock = await _context.Stocks.Include(p => p.Product).Include(w => w.Warehouse).FirstOrDefaultAsync(i => i.Id == id);
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
        public void InsertOrUpdate(List<List<string>> data)
        {
            List<Stock> currentStock = _context.Stocks.ToList();
            List<Stock> stocksFromExcel = new List<Stock>();
            List<Stock> stocksToAdd = new List<Stock>();
            List<Stock> stocksToUpdate = new List<Stock>();
            foreach (List<string> item in data)
            {
                stocksFromExcel.Add(new Stock
                {
                    StockInWarehouse = int.Parse(item[0]),
                    StockInStore = int.Parse(item[1]),
                    WarehouseCapacity = int.Parse(item[2]),
                    StoreCapacity = int.Parse(item[3]),
                    ProductId = int.Parse(item[4]),
                    Currency = item[5],
                    Price = double.Parse(item[6]),
                    WarehouseId = int.Parse(item[7])
                });
            }
            if(stocksFromExcel.Count == 0)
            {
                throw new ArgumentNullException("No data to be changed!");
            }
            if (stocksFromExcel.Any(s => s.Currency.Length > 50))
            {
                throw new ArgumentOutOfRangeException("Currency cannot exceed 50 characters!");
            }
            if (stocksFromExcel.Any(s => s.StockInWarehouse < 0 || s.StockInStore < 0))
            {
                throw new ArgumentOutOfRangeException("Stock cannot be negative!");
            }
            if (stocksFromExcel.Any(s => s.WarehouseCapacity <= 0 || s.StoreCapacity <= 0))
            {
                throw new ArgumentOutOfRangeException("Capacity cannot be equal or less than 0!");
            }
            if (stocksFromExcel.Any(s => s.StockInStore > s.StoreCapacity && s.StockInWarehouse > s.WarehouseCapacity))
            {
                throw new Exception("Stock in store and warehouse cannot exceed their capacities!");
            }
            if (stocksFromExcel.Any(s => !_context.Warehouses.Any(w => w.Id == s.WarehouseId)))
            {
                throw new KeyNotFoundException("One or more warehouses with given ids do not exist!");
            }
            if (stocksFromExcel.Any(s => !_context.Products.Any(p => p.Id == s.ProductId)))
            {
                throw new KeyNotFoundException("One or more products with given ids do not exist!");
            }
            if (stocksFromExcel.Any(s => currentStock.Count(cs => cs.ProductId == s.ProductId) > 1))
            {
                throw new Exception("There are duplicate products in the database!");
            }
            if (stocksFromExcel.Any(s => stocksFromExcel.Count(cs => cs.ProductId == s.ProductId) > 1))
            {
                throw new Exception("There are duplicate products in the imported data!");
            }
            if (stocksFromExcel.Any(s => currentStock.Any(cs => cs.ProductId == s.ProductId && (s.StockInStore < 0 || s.StockInWarehouse < 0))))
            {
                throw new Exception("Stock in store or warehouse cannot be negative!");
            }
            if (stocksFromExcel.Any(s => currentStock.Any(cs => cs.ProductId == s.ProductId && s.Price < 0)))
            {
                throw new ArgumentOutOfRangeException("Price cannot be negative!");
            }

            foreach (Stock stock in stocksFromExcel)
            {
                if (!currentStock.Any(p => p.ProductId == stock.ProductId))
                {
                    stocksToAdd.Add(stock);
                }
                else
                {
                    Stock existingStock = currentStock.First(p => p.ProductId == stock.ProductId);
                    existingStock.StockInStore = stock.StockInStore;
                    existingStock.StockInWarehouse = stock.StockInWarehouse;
                    existingStock.StoreCapacity = stock.StoreCapacity;
                    existingStock.WarehouseCapacity = stock.WarehouseCapacity;
                    existingStock.Price = stock.Price;
                    existingStock.Currency = stock.Currency;
                    stocksToUpdate.Add(existingStock);
                }
            }

            if (stocksToAdd.Count > 0)
            {
                _context.Stocks.AddRange(stocksToAdd);
            }
            if (stocksToUpdate.Count > 0)
            {
                _context.Stocks.UpdateRange(stocksToUpdate);
            }
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task UpdateStockAfterStockChange(int productId, int warehouseId, int quantity)
        {
            Stock stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found!");
            }

            if (stock.StockInWarehouse + quantity < 0)
            {
                throw new ArgumentOutOfRangeException("Stock in warehouse cannot be negative!");
            }

            stock.StockInWarehouse += quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }
        
        public async Task<StockGetDTO?> GetStockByProductAsync(int productId)
        {
            Stock? stock = await _context.Stocks.Include("Product").Where(x => x.Product.Id == productId).FirstOrDefaultAsync();
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found!");
            }

            return _mapper.Map<StockGetDTO>(stock);
        }

        public async Task<List<StockGetWithProductDTO>> GetStocksByWarehouseAsync(int warehouseId)
        {
            var stocks = await _context.Stocks
                .Include(p => p.Product)
                .Include(w => w.Warehouse)
                .Where(s => s.WarehouseId == warehouseId)
                .ToListAsync();
            return _mapper.Map<List<StockGetWithProductDTO>>(stocks);
        }
    }
}
