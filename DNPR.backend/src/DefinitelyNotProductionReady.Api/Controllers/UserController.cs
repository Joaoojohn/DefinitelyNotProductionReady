using DefinitelyNotProductionReady.Application.UserApplication.UserRequestDTO;
using Microsoft.AspNetCore.Mvc;
using PostApp = DefinitelyNotProductionReady.Application.PostApplication.PostApplication;
using UserApp = DefinitelyNotProductionReady.Application.UserApplication.UserApplication;

namespace DefinitelyNotProductionReady.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(UserApp userApplication, PostApp postApplication) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDTO request)
    {
        var user = await userApplication.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await userApplication.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("{userId:guid}/posts")]
    public async Task<IActionResult> GetPosts(Guid userId)
    {
        var posts = await postApplication.GetByUserId(userId);
        return posts is null ? NotFound() : Ok(posts);
    }
}
