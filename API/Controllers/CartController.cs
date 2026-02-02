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

        return Ok(cart ?? new ShoppingCart{Id = id});
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updateCart = await cartService.SetCartAsync(cart);

        if (updateCart == null) return BadRequest("Un problème est survenu avec le panier");

        return updateCart;
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);

        // if (!result) return BadRequest("Un problème est survenu lors de la suppréssion du panier");
        // return Ok();
        return !result ? BadRequest("Un problème est survenu lors de la suppréssion du panier") : Ok();
    }
}
