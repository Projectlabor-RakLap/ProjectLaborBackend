using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        Task InsertOrUpdate(List<List<string>> data);
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

        public async Task InsertOrUpdate(List<List<string>> data)
        {
            List<Product> currentProducts = await _context.Products.ToListAsync();
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
                await _context.Products.AddRangeAsync(productsToAdd);
            }
            if (productsToUpdate.Count > 0)
            {
                 _context.Products.UpdateRange(productsToUpdate);
            }
            try
            {
                 await _context.SaveChangesAsync();
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
