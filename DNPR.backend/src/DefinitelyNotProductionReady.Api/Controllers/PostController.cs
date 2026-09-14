using DefinitelyNotProductionReady.Application.PostApplication.PostRequestDTO;
using Microsoft.AspNetCore.Mvc;
using PostApp = DefinitelyNotProductionReady.Application.PostApplication.PostApplication;

namespace DefinitelyNotProductionReady.Api.Controllers;

[ApiController]
[Route("posts")]
public class PostController(PostApp postApplication) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequestDTO request)
    {
        var post = await postApplication.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var post = await postApplication.GetById(id);
        return post is null ? NotFound() : Ok(post);
    }
}
