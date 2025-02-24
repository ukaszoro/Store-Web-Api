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
    private ProductValidator _productValidator = new ProductValidator(productsManager);

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await productsManager.GetAll()
            .Select(x => ItemToDto(x)).ToListAsync();
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        var product = await productsManager.GetById(id);

        if (product is null)
        {
            return NotFound();
        }

        return ItemToDto(product);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, Product productDto)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (id != productDto.Id)
        {
            return BadRequest();
        }

        var product = await productsManager.GetById(id);
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
        catch (DbUpdateConcurrencyException) when (!_productValidator.Exists(product.Id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product productDto)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
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

        await productsManager.Add(product);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, ItemToDto(product));
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        var product = await productsManager.GetById(id);
        if (!_productValidator.Validate(product))
        {
            return NotFound();
        }

        await productsManager.Remove(product);

        return NoContent();
    }

    private static Product ItemToDto(Product product) =>
        new Product()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
        };
}