using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Moq;
using Application;

namespace Tests.Application.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_repoMock.Object, _mapperMock.Object);
        }

        #region GetAllProductsAsync

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnMappedList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1" },
                new Product { Id = 2, Name = "Product2" }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
                       .Returns(new List<ProductDto>
                       {
                           new ProductDto { Id = 1, Name = "Product1" },
                           new ProductDto { Id = 2, Name = "Product2" }
                       });

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(p => p.Id == 1 && p.Name == "Product1");
        }

        #endregion

        #region GetByProductIdAsync

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await _service.GetByProductIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnProductDto_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "TestProduct" };
            var dto = new ProductDto { Id = 1, Name = "TestProduct" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

            var result = await _service.GetByProductIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("TestProduct");
        }

        #endregion

        #region CreateProductAsync

        [Fact]
        public async Task CreateProductAsync_ShouldMapAndAddProduct()
        {
            var dto = new CreateProductDto { Name = "NewProduct" };
            var product = new Product { Name = "NewProduct" };
            var productDto = new ProductDto { Name = "NewProduct" };

            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
            _repoMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);

            var result = await _service.CreateProductAsync(dto);

            result.Should().BeEquivalentTo(productDto);
            _repoMock.Verify(r => r.AddAsync(product), Times.Once);
        }

        #endregion

        #region UpdateProductAsync

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnNull_WhenProductNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await _service.UpdateProductAsync(1, new UpdateProductDto { Name = "Updated" });

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateAndReturnProductDto_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "OldName" };
            var dto = new UpdateProductDto { Name = "NewName" };
            var productDto = new ProductDto { Id = 1, Name = "NewName" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map(dto, product)).Callback<UpdateProductDto, Product>((d, p) => p.Name = d.Name);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
            _repoMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

            var result = await _service.UpdateProductAsync(1, dto);

            result.Should().BeEquivalentTo(productDto);
            product.Name.Should().Be("NewName");
            product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _repoMock.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        #endregion

        #region DeleteProductAsync

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var result = await _service.DeleteProductAsync(1, "tester");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductAlreadyDeleted()
        {
            var product = new Product { DeletedAt = DateTime.UtcNow };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _service.DeleteProductAsync(1, "tester");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldMarkDeletedAndReturnTrue_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "Test" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _repoMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

            var result = await _service.DeleteProductAsync(1, "tester");

            result.Should().BeTrue();
            product.IsDeleted.Should().BeTrue();
            product.DeletedBy.Should().Be("tester");
            product.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _repoMock.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        #endregion
    }
}
