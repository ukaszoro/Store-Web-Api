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
public class ProductsController(IProductsManager productsManager, IEmployeeSessionManager employeeSessionManager) : ControllerBase
{
    private readonly ProductValidator _productValidator = new ProductValidator(productsManager);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        return await productsManager
            .GetAll()
            .Select(x => new ProductDTO
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(long id)
    {
        var product = await productsManager.GetById(id);

        if (product is null)
        {
            return NotFound();
        }

        return new ProductDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, ProductDTO productDto)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (id != productDto.Id)
        {
            return BadRequest();
        }

        var product = await productsManager.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        if (!_productValidator.Validate(productDto))
        {
            return BadRequest();
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

    [HttpPost]
    public async Task<ActionResult> PostProduct(ProductDTO productDto)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (!_productValidator.Validate(productDto))
        {
            return BadRequest();
        }

        await productsManager.Add(new Product { Name = productDto.Name, Price = productDto.Price });

        return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
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