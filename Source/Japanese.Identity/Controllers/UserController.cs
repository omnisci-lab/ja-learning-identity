using Japanese.Identity.Models;
using khothemegiatot.WebApi.Attributes;
using khothemegiatot.WebApi.Enums;
using khothemegiatot.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Japanese.Identity.Controllers;

[Route("api")]
[ApiController]
[HandlerException]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [Route("user-list")]
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ExecResult<PagedResult<ApplicationUser>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserList([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        List<ApplicationUser> users = await _userManager.Users
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        int userCount = await _userManager.Users.CountAsync();

        return Ok(new PagedResult<ApplicationUser> { 
            Items = users, 
            Page = page, 
            PageSize = pageSize,
            TotalItems = userCount,
        });
    }

    [Route("user-details/{id}")]
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ExecResult<ApplicationUser>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        ApplicationUser? user = await _userManager.Users
            .Where(x => x.Id == id).FirstOrDefaultAsync();

        if(user is null)
            return NotFound(new ExecResult { Status = ExecStatus.NotFound });

        return Ok(new ExecResult<ApplicationUser> { Status= ExecStatus.Success, Data = user });
    }
}
