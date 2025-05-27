using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        return Ok(await repo.GetProductsAsync(brand, type, sort));
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        return product != null ? product : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);

        // if (await repo.SaveChangesAsync())
        // {
        //     return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        // }

        // return BadRequest("Un problème est survenu lors de la création du produit.");

        return await repo.SaveChangesAsync()
            ? CreatedAtAction("GetProduct", new { id = product.Id }, product)
            : BadRequest("Un problème est survenu lors de la création du produit.");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
            return BadRequest("Impossible de modifier ce produit");

        repo.UpdateProduct(product);

        // if (await repo.SaveChangesAsync()) 
        // {
        //     return NoContent();
        // }

        // return BadRequest("Un problème est survenu lors de la modification du produit.");

        return await repo.SaveChangesAsync() ? NoContent() : BadRequest("Un problème est survenu lors de la modification du produit.");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if (product == null) return NotFound();
        
        repo.DeleteProduct(product);

        // if (await repo.SaveChangesAsync()) 
        // {
        //     return NoContent();
        // }

        // return BadRequest("Un problème est survenu lors de la suppréssion du produit.");

        return await repo.SaveChangesAsync() ? NoContent() : BadRequest("Un problème est survenu lors de la suppréssion du produit.");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypesAsync());
    }

    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }
}
