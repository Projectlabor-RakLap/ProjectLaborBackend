using ProjectLaborBackend.Dtos.Warehouse;
using ProjectLaborBackend.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Threading.Tasks;

namespace ProjectLaborBackend.Services
{
    public interface IWarehouseService
    {
        Task<List<WarehouseGetDTO>> GetAllWarehousesAsync();
        Task<WarehouseGetDTO> GetWarehouseByIdAsync(int id);
        Task CreateWarehouseAsync(WarehousePostDTO warehouseDto);
        Task PatchWarehouseAsync(int id, WarehouseUpdateDTO warehouseDto);
        Task DeleteWarehouseAsync(int id);
        void InsertOrUpdate(List<List<string>> data);
    }

    public class WarehouseService : IWarehouseService
    {
        private AppDbContext _context;
        private IMapper _mapper; 
        public WarehouseService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<WarehouseGetDTO> GetWarehouseByIdAsync(int id)
        {
            Warehouse? wareHouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (wareHouse == null)
                throw new KeyNotFoundException($"Warehouse with id: {id} is not found");

            return _mapper.Map<WarehouseGetDTO>(wareHouse);
        }

        public async Task<List<WarehouseGetDTO>> GetAllWarehousesAsync()
        {
            var wareHouses = await _context.Warehouses.ToListAsync();
            return _mapper.Map<List<WarehouseGetDTO>>(wareHouses);
        }

        public async Task CreateWarehouseAsync(WarehousePostDTO warehouseDto)
        {
            if(warehouseDto == null)
                throw new ArgumentNullException("Name or location needed");
            if (_context.Warehouses.Any(x => x.Location == warehouseDto.Location))
                throw new ArgumentException($"There is already an existing warehouse with location: {warehouseDto.Location}");

            if (warehouseDto.Name.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Name cannot exceed 100 characters!");
            }

            if (warehouseDto.Location.Length > 200)
            {
                throw new ArgumentOutOfRangeException("Location cannot exceed 200 characters!");
            }

            Warehouse wareHouse = _mapper.Map<Warehouse>(warehouseDto);
            await _context.Warehouses.AddAsync(wareHouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            Warehouse? wareHouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (wareHouse == null)
                throw new KeyNotFoundException($"Warehouse with id: {id} is not found");
            
            _context.Warehouses.Remove(wareHouse);
            await _context.SaveChangesAsync();
        }

        public async Task PatchWarehouseAsync(int id, WarehouseUpdateDTO warehouseDto)
        {
            if(warehouseDto == null)
                throw new ArgumentNullException("Empty object passed");
            Warehouse? wareHouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (wareHouse == null)
                throw new KeyNotFoundException($"Warehouse with id: {id} is not found");
            if(warehouseDto.Location != null && _context.Warehouses.Any(x => x.Location == warehouseDto.Location && x.Id != id))
                throw new ArgumentException($"There is already an existing warehouse with location: {warehouseDto.Location}");

            if (warehouseDto.Name != null && warehouseDto.Name.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Name cannot exceed 100 characters!");
            }

            if (warehouseDto.Location != null && warehouseDto.Location.Length > 200)
            {
                throw new ArgumentOutOfRangeException("Location cannot exceed 200 characters!");
            }

            _mapper.Map(warehouseDto, wareHouse);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
                {
                    throw new KeyNotFoundException($"Warehouse with id: {id} is not found");
                }
                else
                {
                    throw;
                }
            }
        }

        public void InsertOrUpdate(List<List<string>> data)
        {
            List<Warehouse> currentWareHouse = _context.Warehouses.ToList();
            List<Warehouse> StockChangeFromExcel = new List<Warehouse>();
            List<Warehouse> StockChangeToAdd = new List<Warehouse>();
            List<Warehouse> WareHouseToUpdate = new List<Warehouse>();
            foreach (List<string> item in data)
            {
                StockChangeFromExcel.Add(new Warehouse
                {
                    Name = item[0],
                    Location = item[1],
                });
            }
            
            if (StockChangeFromExcel.Count == 0)
                throw new ArgumentException("No data found in the Excel file.");
            if (StockChangeFromExcel.Any(p => p.Name.Length > 100))
                throw new ArgumentException("Name cannot exceed 100 characters!");
            if (StockChangeFromExcel.Any(p => p.Location.Length > 200))
                throw new ArgumentException("Location cannot exceed 200 characters!");
            if (StockChangeFromExcel.GroupBy(p => p.Location).Any(g => g.Count() > 1))
                throw new ArgumentException("There are duplicate locations in the Excel file!");

            foreach (Warehouse stockChange in StockChangeFromExcel)
            {
                if (!currentWareHouse.Any(p => p.Id == stockChange.Id))
                {
                    StockChangeToAdd.Add(stockChange);
                }
                else
                {
                    Warehouse existingWareHouse = currentWareHouse.First(p => p.Id == stockChange.Id);
                    existingWareHouse.Name = stockChange.Name;
                    existingWareHouse.Location = stockChange.Location;
                    WareHouseToUpdate.Add(existingWareHouse);
                }
            }

            if (StockChangeToAdd.Count > 0)
            {
                _context.Warehouses.AddRange(StockChangeToAdd);
            }
            if (WareHouseToUpdate.Count > 0)
            {
                _context.Warehouses.UpdateRange(WareHouseToUpdate);
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
        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }
    }
}
