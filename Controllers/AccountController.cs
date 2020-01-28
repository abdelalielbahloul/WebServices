using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebServices.Models;
using WebServices.ModelViews;

namespace WebServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _manager;

        public AccountController(ApplicationDbContext db, UserManager<User> manager)
        {
            _db = db;
            _manager = manager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (ModelState.IsValid)
            {
                if (EmailExists(model.Email))
                {
                    return BadRequest("Email already used");
                }
                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = model.Password
                };
                var result = await _manager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest);

        }

        private bool EmailExists(string email)
        {
            return _db.Users.Any(x => x.Email == email);
        }
    }
}