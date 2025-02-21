using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products;
using Products.Models;
using WebApi.DTO;

namespace WebApi.Controllers;

[Route("api/Products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductContext _context;

    public ProductsController(ProductContext context)
    {
        _context = context;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        return await _context.Products
            .Select(x => ItemToDTO(x))
            .ToListAsync();
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(long id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return ItemToDTO(product);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, ProductDTO productDTO)
    {
        if (id != productDTO.ID)
        {
            return BadRequest();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = productDTO.Name;
        product.Price = productDTO.Price;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!ProductExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDTO)
    {
        var product = new Product
        {
            Name = productDTO.Name,
            Price = productDTO.Price,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.ID }, ItemToDTO(product));
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(long id)
    {
        return _context.Products.Any(e => e.ID == id);
    }

    private static ProductDTO ItemToDTO(Product product) =>
        new ProductDTO()
        {
            ID = product.ID,
            Name = product.Name,
            Price = product.Price,
        };
}