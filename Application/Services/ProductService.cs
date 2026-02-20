using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public  class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProductService> _logger;

        public ProductService(IProductRepository repo, IMapper mapper, IAppLogger<ProductService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedBy = "system";
            await _repo.AddAsync(product);
            _logger.LogInformation($"Product {product.Id} created by {product.CreatedBy}");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetByProductIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id, true);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id, false);

            if (product == null)
                return null;

            _mapper.Map(dto, product);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = "system";

            await _repo.UpdateAsync(product);
            _logger.LogInformation($"Product {product.Id} updated by {product.UpdatedBy}");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id, string deletedBy)
        {
            var product = await _repo.GetByIdAsync(id, false);

            if (product == null || product.DeletedAt != null)
                return false;

            product.DeletedBy = deletedBy;
            product.DeletedAt = DateTime.UtcNow;
            product.IsDeleted = true;
            _logger.LogInformation($"Product {product.Id} deleted by {product.DeletedBy}");
            await _repo.UpdateAsync(product);
            return true;
        }
    }
}
