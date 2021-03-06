﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreReactReduxAdvanced.Models.Blog;
using MongoDB.Bson;
using NetCoreReactReduxAdvanced.Models;
using NetCoreReactReduxAdvanced.Services;

namespace NetCoreReactReduxAdvanced.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class BlogsController : ControllerBase
  {

    private readonly IBlogService _blogService;
    private readonly UserManager<ApplicationUser> _userManager;


    public BlogsController(UserManager<ApplicationUser> userManager, IBlogService blogService)
    {
      _blogService = blogService;
      _userManager = userManager;

    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      var response = await _blogService.Find(user.Id);

      return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      var response = await _blogService.FindOne(user.Id, new ObjectId(id));
      return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostBlog request)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      var response = await _blogService.Create(request, user.Id);
      return Ok(response);
    }

  }
}
