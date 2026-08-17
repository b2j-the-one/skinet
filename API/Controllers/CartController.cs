using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await cartService.GetCartAsync(id);

        return Ok(cart ?? new ShoppingCart {Id = id});
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updateCart = await cartService.SetCartAsync(cart);

        if (updateCart == null) return BadRequest("Un problème est survenu avec le panier");

        return updateCart;
    }

    [HttpDelete]
    //[HttpDelete("{id}")] // Ne jamais utiliser string comme contrainte
    //[HttpGet("{id:int}")] // Paramètre doit être un entier.
    //[HttpGet("{name:alpha}")] // Paramètre doit contenir uniquement des lettres (A–Z, a–z)
    //[HttpGet("{flag:bool}")] // Paramètre doit être true ou false
    //[HttpGet("{id:long}")]  // Paramètre doit être un entier 64 bits
    //[HttpGet("{id:guid}")] // Paramètre doit être un GUID valide
    //[HttpGet("{date:datetime}")] // Paramètre doit être une date valide
    //[HttpGet("{price:decimal}")] // Paramètre doit être un nombre décimal
    //[HttpGet("{value:double}")]
    //[HttpGet("{value:float}")]
    //[HttpGet("{age:min(18)}")] // Contraintes numériques
    //[HttpGet("{age:max(65)}")]
    //[HttpGet("{age:range(18,65)}")]
    //[HttpGet("{code:length(5)}")] // Contraintes sur la longueur de chaîne
    //[HttpGet("{code:minlength(3)}")]
    //[HttpGet("{code:maxlength(10)}")]
    //[HttpGet("{name:regex(^[a-zA-Z]+$)}")] // Contrôle total via expression régulière
    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);

        // if (!result) return BadRequest("Un problème est survenu lors de la suppréssion du panier");
        // return Ok();
        return !result ? BadRequest("Un problème est survenu lors de la suppréssion du panier") : Ok();
    }
}
