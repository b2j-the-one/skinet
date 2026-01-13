using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        // return Unauthorized();
        return BadRequest("Non autorisé");
    }

    [HttpGet("badRequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("Une erreur est survenue");
    }
    
    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }
    
    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("Ceci est une exception de test");
    }
    
    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDto product)
    {
        // return Ok();
        return BadRequest("Une ou plusieurs erreurs de validation se sont produites");
    }
}
