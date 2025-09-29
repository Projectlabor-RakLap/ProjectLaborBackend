using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectLaborBackend.Dtos.Product;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Services
{
    public interface IProductService
    {
        Task<List<ProductGetDTO>> GetAllProductsAsync();
        Task<ProductGetDTO?> GetProductByIdAsync(int id);
        Task CreateProductAsync(ProductCreateDTO product);
        Task UpdateProductAsync(int id, ProductUpdateDTO dto);
        Task DeleteProductAsync(int id);
        void InsertOrUpdate(List<List<string>> data);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateProductAsync(ProductCreateDTO product)
        {
            if (product.EAN.Length > 20)
            {
                throw new ArgumentException("EAN must be 20 characters or less!");
            }

            if (await _context.Products.AnyAsync(p => p.EAN == product.EAN))
            {
                throw new ArgumentException("Product with this EAN already exists!");
            }

            if (product.Name.Length > 100)
            {
                throw new ArgumentException("Product name must be 100 characters or less!");
            }

            if (product.Description.Length > 500)
            {
                throw new ArgumentException("Description must be 500 characters or less!");
            }

            await _context.Products.AddAsync(_mapper.Map<Product>(product));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductGetDTO>> GetAllProductsAsync()
        {
            return _mapper.Map<List<ProductGetDTO>>(await _context.Products.ToListAsync());
        }

        public async Task<ProductGetDTO?> GetProductByIdAsync(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found!");
            }

            return _mapper.Map<ProductGetDTO>(product);
        }

        public async Task UpdateProductAsync(int id, ProductUpdateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("No data to be changed!");
            }

            Product? product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found!");
            }

            if (dto.EAN.Length > 20)
            {
                throw new ArgumentException("EAN must be 20 characters or less!");
            }

            if (await _context.Products.AnyAsync(p => p.EAN == dto.EAN))
            {
                throw new ArgumentException("Product with this EAN already exists!");
            }

            if (dto.Name.Length > 100)
            {
                throw new ArgumentException("Product name must be 100 characters or less!");
            }

            if (dto.Description.Length > 500)
            {
                throw new ArgumentException("Description must be 500 characters or less!");
            }

            _mapper.Map(dto, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public void InsertOrUpdate(List<List<string>> data)
        {
            List<Product> currentProducts = _context.Products.ToList();
            List<Product> productsFromExcel = new List<Product>();
            List<Product> productsToAdd = new List<Product>();
            List<Product> productsToUpdate = new List<Product>();
            foreach (List<string> item in data)
            {
                productsFromExcel.Add(new Product
                {
                    EAN = item[0],
                    Name = item[1],
                    Description = item[2],
                    Image = "temp"
                });
            }

            if (productsFromExcel.Count == 0)
                throw new ArgumentNullException("No data was read");
            if (productsFromExcel.Any(p => p.EAN.Length > 20))
                throw new ArgumentException("Ean cant be longer than 20 cahrs");
            if(productsFromExcel.Any(p => p.Name.Length > 100))
                throw new ArgumentException("Name cant be longer than 100 cahrs");
            if(productsFromExcel.Any(p => p.Description.Length > 500))
                throw new ArgumentException("Description cant be longer than 500 cahrs");

            foreach (Product product in productsFromExcel)
            {
                if (!currentProducts.Any(p => p.EAN == product.EAN))
                {
                    productsToAdd.Add(product);
                }
                else
                {
                    Product existingProduct = currentProducts.First(p => p.EAN == product.EAN);
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    productsToUpdate.Add(existingProduct);
                }
            }

            if (productsToAdd.Count > 0)
            {
                _context.Products.AddRange(productsToAdd);
            }
            if (productsToUpdate.Count > 0)
            {
                _context.Products.UpdateRange(productsToUpdate);
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
    }
}
