using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebServices.Models;
using WebServices.ModelViews;
using WebServices.Services;

namespace WebServices.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(ApplicationDbContext db, UserManager<User> manager, SignInManager<User> signInManager)
        {
            _db = db;
            _manager = manager;
            _signInManager = signInManager;
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
                    var token = await _manager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmRegister", "Account", new { Id = user.Id, Token = HttpUtility.UrlEncode(token) }, Request.Scheme);
                    //return StatusCode(StatusCodes.Status201Created, "User was created successfully");
                    var subject = "Registration confirm";
                    var content = "Confirm your registration";
                    var HTMLContent = "<a href=" + confirmationLink + ">Confirmation link</a>";
                    if (await SendMailsApi.Execute(user.Email, user.UserName, subject, content, HTMLContent))
                    {
                        return Ok("An email of confirmation has sent to " + user.Email);

                    }

                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest);

        }

       
        [HttpPost]
        [Route("Login")]
        [Produces("application/json")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model == null)
                return StatusCode(StatusCodes.Status404NotFound);

            var user = await _manager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status404NotFound);

            if (!user.EmailConfirmed)
                return Unauthorized("Email not confirmed yet!");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (result.Succeeded)
                return Ok("login success");
            else if(result.IsLockedOut)
                return BadRequest("You can not access to you account for the moment try after a while");
            else
                return BadRequest(result.IsNotAllowed);
        }

        [HttpGet]
        [Route("ConfirmRegister")]
        [Produces("application/json")]
        public async Task<IActionResult> ConfirmRegister(string id, string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            var user = await _manager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _manager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(token));
            if (result.Succeeded)
            {
                return Ok("You are now confirm your registration");
            }
            else
            {
                 return BadRequest(result.Errors);
            }
        }

        /**
         * This is all functions we need
         */
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