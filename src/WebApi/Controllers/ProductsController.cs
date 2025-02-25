using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Models;
using Products.ProductManager;
using WebApi.Validator;
using WebApi.SessionManager;

namespace WebApi.Controllers;

[Route("api/Products")]
[ApiController]
public class ProductsController(IProductsManager productsManager, ISessionManager sessionManager) : ControllerBase
{
    private ProductValidator _productValidator = new ProductValidator(productsManager);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await productsManager.GetAll()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        var product = await productsManager.GetById(id);

        if (product is null)
        {
            return NotFound();
        }

        return product;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, Product productNew)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (id != productNew.Id)
        {
            return BadRequest();
        }

        var product = await productsManager.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        if (!_productValidator.Validate(productNew))
        {
            return BadRequest();
        }

        product.Name = productNew.Name;
        product.Price = productNew.Price;

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

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (!_productValidator.Validate(product))
        {
            return BadRequest();
        }

        await productsManager.Add(product);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        var product = await productsManager.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        await productsManager.Remove(product);

        return NoContent();
    }
}