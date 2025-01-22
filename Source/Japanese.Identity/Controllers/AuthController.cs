using Japanese.Identity.Models;
using khothemegiatot.WebApi.Attributes;
using khothemegiatot.WebApi.Enums;
using khothemegiatot.WebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Japanese.Identity.Controllers;

[Route("api")]
[ApiController]
[HandlerException]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtToken _jwtToken;

    public AuthController(UserManager<ApplicationUser> userManager, JwtToken jwtToken)
    {
        _userManager = userManager;
        _jwtToken = jwtToken;
    }

    [Route("auth-login")]
    [HttpPost]
    [ProducesResponseType(typeof(ExecResult<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return NotFound(new ExecResult { Status = ExecStatus.NotFound });

        bool checkPwd = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!checkPwd)
            return Unauthorized(new ExecResult { Status = ExecStatus.Failed });

        return Ok(new ExecResult<string> { 
            Status = ExecStatus.Success, 
            Data = _jwtToken.Generate(user) 
        });
    }

    [Route("auth-register")]
    [HttpPost]
    [ProducesResponseType(typeof(ExecResult<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ExecResult { Status = ExecStatus.Invalid });

        ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(new ExecResult { Status = ExecStatus.Failed });

        return Ok(new ExecResult { Status = ExecStatus.Success });
    }
}
