using API.Responses;
using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService
            )
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var response = new ApiResponse<IEnumerable<ProductDto>>(products);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByProductId(int id)
        {
            var product = await _productService.GetByProductIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse<object>(null, false, "Product not found"));

            return Ok(new ApiResponse<ProductDto>(product));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null)
                return validationResult;

            var created = await _productService.CreateProductAsync(dto);
            var response = new ApiResponse<ProductDto>(created, true, "Product created successfully");
            return CreatedAtAction(nameof(GetByProductId), new { id = created.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            var validationResult = ValidateModelState();
            if(validationResult!=null)
                return validationResult;

            var updated = await _productService.UpdateProductAsync(id, dto);

            if (updated == null)
                return NotFound(new ApiResponse<object>(null, false, "Product not found"));

            return Ok(new ApiResponse<ProductDto>(updated, true, "Product updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id, "system");

            if (!deleted)
                return NotFound(new ApiResponse<object>(null, false, "Product not found"));

            return Ok(new ApiResponse<object>(null, true, "Product deleted successfully"));
        }

        private IActionResult ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new ApiResponse<object>(errors, false, "Validation failed"));
            }

            return null!;
        }
    }
}

