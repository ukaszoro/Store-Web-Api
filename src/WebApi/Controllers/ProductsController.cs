using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Models;
using Products.ProductManager;
using WebApi.DTO;
using WebApi.Validator;
using WebApi.SessionManager;

namespace WebApi.Controllers;

[Route("api/Products")]
[ApiController]
public class ProductsController(IProductsManager productsManager, ISessionManager sessionManager) : ControllerBase
{
    private static ProductValidator _productValidator = new ProductValidator();

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        return await productsManager.GetProducts()
            .Select(x => ItemToDto(x)).ToListAsync();
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(long id)
    {
        var product = await productsManager.GetProductById(id);

        if (!_productValidator.Validate(product))
        {
            return NotFound();
        }

        return ItemToDto(product);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, ProductDTO productDto)
    {
        if (!sessionManager.IsLoggedIn(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (id != productDto.ID)
        {
            return BadRequest();
        }

        var product = await productsManager.GetProductById(id);
        if (!_productValidator.Validate(product))
        {
            return NotFound();
        }

        product.Name = productDto.Name;
        product.Price = productDto.Price;

        try
        {
            await productsManager.SaveChanges();
        }
        catch (DbUpdateConcurrencyException) when (!_productValidator.ProductExists(product))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDto)
    {
        if (!sessionManager.IsLoggedIn(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
        };
        if (!_productValidator.Validate(product))
        {
            return BadRequest();
        }

        await productsManager.AddProduct(product);

        return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, ItemToDto(product));
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        if (!sessionManager.IsLoggedIn(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        var product = await productsManager.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        await productsManager.RemoveProduct(product);

        return NoContent();
    }

    private static ProductDTO ItemToDto(Product product) =>
        new ProductDTO()
        {
            ID = product.ID,
            Name = product.Name,
            Price = product.Price,
        };
}