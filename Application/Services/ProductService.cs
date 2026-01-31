using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public  class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(entities);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var entity = _mapper.Map<Product>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = "system";
            await _repo.AddAsync(entity);
            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<ProductDto?> GetByProductIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ProductDto>(entity);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                return null;

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = "system";

            await _repo.UpdateAsync(entity);

            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<bool> DeleteProductAsync(int id, string deletedBy)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null || product.DeletedAt != null)
                return false;

            product.DeletedBy = deletedBy;
            product.DeletedAt = DateTime.UtcNow;
            product.IsDeleted = true;

            await _repo.UpdateAsync(product);
            return true;
        }
    }
}
