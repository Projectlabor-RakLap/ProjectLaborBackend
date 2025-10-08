using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Dtos.StockChange;
using ProjectLaborBackend.Entities;
using System.Linq;
namespace ProjectLaborBackend.Services
{
    public interface IStockChangeService
    {
        Task<List<StockChangeGetDTO>> GetAllStockChangeAsync();
        Task<StockChangeGetDTO> GetStockChangeByIdAsync(int id);
        Task CreateStockChangeAsync(StockChangeCreateDTO stockChangeDto);
        Task PatchStockChangesAsync(int id, StockChangeUpdateDTO stockChangeDto);
        Task DeleteStockChangeAsync(int id);
        void InsertOrUpdate(List<List<string>> data);
        Task<double> CalculateMovingAveragePriceAsync(int productId, int windowSize);
        Task<List<StockChangeGetDTO>> GetAllStockChangeByWarehouseAsync(string product, string warehouse);
    }
    public class StockChangeService : IStockChangeService
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public StockChangeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<StockChangeGetDTO> GetStockChangeByIdAsync(int id)
        {
            StockChange? stockChange = await _context.StockChanges.FirstOrDefaultAsync(s => s.Id == id);
            if (stockChange == null)
                throw new KeyNotFoundException($"StockChange with id: {id} is not found");

            return _mapper.Map<StockChangeGetDTO>(stockChange);
        }

        public async Task<List<StockChangeGetDTO>> GetAllStockChangeAsync()
        {
            var stockChanges = await _context.StockChanges.ToListAsync();
            return _mapper.Map<List<StockChangeGetDTO>>(stockChanges);
        }
        
        public async Task CreateStockChangeAsync(StockChangeCreateDTO stockChangeDto)
        {
            if (stockChangeDto == null)
                throw new ArgumentNullException("Quantity, ChangeDate or ProductId needed");
            if (!_context.Products.Any(p => p.Id == stockChangeDto.ProductId))
                throw new ArgumentException($"There is no product with id: {stockChangeDto.ProductId}");

            if (stockChangeDto.Quantity == 0) 
            {
                throw new ArgumentException("Quantity cannot be zero!");
            }

                StockChange stockChange = _mapper.Map<StockChange>(stockChangeDto);
            await _context.StockChanges.AddAsync(stockChange);
            await _context.SaveChangesAsync();
        }
        
        public async Task PatchStockChangesAsync(int id, StockChangeUpdateDTO stockChangeDto)
        {
            if (stockChangeDto == null)
                throw new ArgumentNullException("Quantity, ChangeDate or ProductId needed");
            if (stockChangeDto.ProductId != null && !_context.Products.Any(p => p.Id == stockChangeDto.ProductId))
                throw new ArgumentException($"There is no product with id: {stockChangeDto.ProductId}");
            if (stockChangeDto.Quantity == 0)
                throw new ArgumentException("Quantity cannot be zero");

            if (stockChangeDto.Quantity != null && stockChangeDto.Quantity == 0)
            {
                throw new ArgumentException("Quantity cannot be zero!");
            }

            StockChange? stockChange = await _context.StockChanges.FirstOrDefaultAsync(s => s.Id == id);
            if (stockChange == null)
                throw new KeyNotFoundException($"StockChange with id: {id} is not found");

            _mapper.Map(stockChangeDto, stockChange);
            _context.StockChanges.Update(stockChange);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StockChanges.Any(e => e.Id == id))
                {
                    throw new KeyNotFoundException($"StockChange with id: {id} is not found");
                }
                else
                {
                    throw;
                }
            }
        }
        
        public async Task DeleteStockChangeAsync(int id)
        {
            StockChange? stockChange = await _context.StockChanges.FirstOrDefaultAsync(s => s.Id == id);
            if (stockChange == null)
                throw new KeyNotFoundException($"StockChange with id: {id} is not found");
            _context.StockChanges.Remove(stockChange);
            await _context.SaveChangesAsync();
        }

        public void InsertOrUpdate(List<List<string>> data)
        {
            List<StockChange> currentStockChange = _context.StockChanges.ToList();
            List<StockChange> StockChangeFromExcel = new List<StockChange>();
            List<StockChange> StockChangeToAdd = new List<StockChange>();
            List<StockChange> StockChangeToUpdate = new List<StockChange>();
            foreach (List<string> item in data)
            {
                StockChangeFromExcel.Add(new StockChange
                {
                    Quantity = Convert.ToInt32(item[0]),
                    ChangeDate = Convert.ToDateTime(item[1]),
                    ProductId = Convert.ToInt32(item[2]),
                });
            }

            if (StockChangeFromExcel.Count == 0)
                throw new ArgumentNullException("No data was read");
            if (StockChangeFromExcel.Any(p => p.ProductId == 0))
                throw new ArgumentException("ProductId cannot be zero!");
            if (StockChangeFromExcel.Any(p => !_context.Products.Any(prod => prod.Id == p.ProductId)))
                throw new ArgumentException("There is no product with one of the given ProductIds!");
            if (StockChangeFromExcel.Any(p => p.Quantity == 0))
                throw new ArgumentException("Quantity cannot be zero!");

            foreach (StockChange stockChange in StockChangeFromExcel)
            {
                if (!currentStockChange.Any(p => p.Id == stockChange.Id))
                {
                    StockChangeToAdd.Add(stockChange);
                }
                else
                {
                    StockChange existingStockChange = currentStockChange.First(p => p.Id == stockChange.Id);
                    existingStockChange.Quantity = stockChange.Quantity;
                    existingStockChange.ProductId = stockChange.ProductId;
                    StockChangeToUpdate.Add(existingStockChange);
                }
            }

            if (StockChangeToAdd.Count > 0)
            {
                _context.StockChanges.AddRange(StockChangeToAdd);
            }
            if (StockChangeToUpdate.Count > 0)
            {
                _context.StockChanges.UpdateRange(StockChangeToUpdate);
            }
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public async Task<double> CalculateMovingAveragePriceAsync(int productId, int windowSize)
        {
            if (!_context.Products.Any(p => p.Id == productId))
            {
                throw new ArgumentException($"There is no product with id: {productId}");
            }

            var changes = await _context.StockChanges
                .Where(sc => sc.ProductId == productId && sc.Quantity < 0)
                .OrderByDescending(sc => sc.ChangeDate)
                .Take(windowSize)
                .ToListAsync();

            if (windowSize <= 0)
            {
                throw new ArgumentException($"Window must be positive and not 0!");
            }

            if (changes.Count < windowSize)
            {
                throw new Exception($"Not enough stock changes to calculate moving average for product with id: {productId}");
            }

            double average = Math.Abs(await _context.StockChanges
                .Where(sc => sc.ProductId == productId && sc.Quantity < 0)
                .OrderByDescending(sc => sc.ChangeDate)
                .Take(windowSize)
                .AverageAsync(sc => sc.Quantity));


            return average;
        }

        public async Task<List<StockChangeGetDTO>> GetAllStockChangeByWarehouseAsync(string product, string warehouse)
        {
            var stockChanges = await _context.StockChanges
                .Include(sc => sc.Product)
                .Where(sc => sc.Product.Name == product)
                .Where(sc => sc.Product.Stocks.Any(s => s.Warehouse.Name == warehouse))
                .ToListAsync();

            return _mapper.Map<List<StockChangeGetDTO>>(stockChanges);
        }
    }
}
