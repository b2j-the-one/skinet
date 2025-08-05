using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        return product != null ? product : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.Add(product);

        // if (await repo.SaveAllAsync())
        // {
        //     return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        // }

        // return BadRequest("Un problème est survenu lors de la création du produit.");

        return await repo.SaveAllAsync()
            ? CreatedAtAction("GetProduct", new { id = product.Id }, product)
            : BadRequest("Un problème est survenu lors de la création du produit.");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
            return BadRequest("Impossible de modifier ce produit");

        repo.Update(product);

        // if (await repo.SaveAllAsync()) 
        // {
        //     return NoContent();
        // }

        // return BadRequest("Un problème est survenu lors de la modification du produit.");

        return await repo.SaveAllAsync() ? NoContent() : BadRequest("Un problème est survenu lors de la modification du produit.");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();
        
        repo.Remove(product);

        // if (await repo.SaveAllAsync()) 
        // {
        //     return NoContent();
        // }

        // return BadRequest("Un problème est survenu lors de la suppréssion du produit.");

        return await repo.SaveAllAsync() ? NoContent() : BadRequest("Un problème est survenu lors de la suppréssion du produit.");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id)
    {
        return repo.Exists(id);
    }
}
