using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers;

[Route("[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int size = 10, int page = 1)
    {
        var users = await _userService.GetAllAsync(size, page);

        return Json(new PagedUsersDto
        {
            PageNumber = page,
            PageSize = size,
            Users = _mapper.Map<UserDto[]>(users)
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var users = await _userService.GetByIdAsync(id);
        if (users == null)
        {
            return NotFound();
        }

        return Json(_mapper.Map<UserDto>(users));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto user)
    {
        var createdUser = await _userService.CreateUserAsync(_mapper.Map<CreateUserDto>(user));

        return Json(_mapper.Map<UserDto>(createdUser));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (success)
        {
            return Ok();
        }

        return NoContent();
    }
}
