using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        [Produces("application/json")]
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
                if (!isEmailValid(model.Email))
                {
                    return BadRequest("Email not valid!");
                }
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };
                var result = await _manager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, "User was created successfully");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest);

        }

       
        [HttpGet]
        [Route("Login")]
        [Produces("application/json")]
        public IActionResult Login()
        {
            return StatusCode(StatusCodes.Status200OK, "login method");
            
        }
        private bool EmailExists(string email)
        {
            return _db.Users.Any(x => x.Email == email);
        }

        private bool isEmailValid(string email)
        {
            Regex regexEmail = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regexEmail.IsMatch(email) ? true : false;
        }
       

    }
}