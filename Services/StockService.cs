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

            Stock? stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found!");
            }

            _mapper.Map(dto, stock);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
    }
}
