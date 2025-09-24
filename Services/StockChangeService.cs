using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectLaborBackend.Dtos.StockChange;
using ProjectLaborBackend.Entities;
namespace ProjectLaborBackend.Services
{
    public interface IStockChangeService
    {
        Task<List<StockChangeGetDTO>> GetAllStockChangeAsync();
        Task<StockChangeGetDTO> GetStockChangeByIdAsync(int id);
        Task CreateStockChangeAsync(StockChangeCreateDTO stockChangeDto);
        Task PatchStockChangesAsync(int id, StockChangeUpdateDTO stockChangeDto);
        Task DeleteStockChangeAsync(int id);
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
    }
}
