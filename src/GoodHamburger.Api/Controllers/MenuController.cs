using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("menu")]
[Produces("application/json")]
public class MenuController(IMenuRepository menu) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MenuItemResponse>), StatusCodes.Status200OK)]
    public IActionResult GetMenu()
    {
        var items = menu.GetAll().Select(i => new MenuItemResponse
        {
            Id = i.Id,
            Name = i.Name,
            Price = i.Price,
            Type = i.Type.ToString()
        });
        return Ok(items);
    }
}
