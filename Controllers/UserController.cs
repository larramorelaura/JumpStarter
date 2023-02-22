using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using JumpStarter.Models;

namespace JumpStarter.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    
    private MyContext _context;  
    public UserController(ILogger<UserController> logger, MyContext context)    
    {        
        _logger = logger;
        _context = context;    
    } 

[HttpGet("")]
public IActionResult Index()
{
    return View("Index");
}

[HttpPost("registration")]   
    public IActionResult CreateUser(User newUser)    
    {        
        if(ModelState.IsValid)        
        {
            // Initializing a PasswordHasher object, providing our User class as its type            
            PasswordHasher<User> Hasher = new PasswordHasher<User>();   
            // Updating our newUser's password to a hashed version         
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);            
            //Save your user object to the database 
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction( "AllProjects", "Project");       
        } else {
            return View("Index");
        }   
    }

[HttpPost("login")]
    public IActionResult Login(LoginUser userSubmission)
    {    
        if(ModelState.IsValid)    
        {        
            // If initial ModelState is valid, query for a user with the provided email        
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);        
            // If no user exists with the provided email        
            if(userInDb == null)        
            {            
                // Add an error to ModelState and return to RedirectToAction!            
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");            
                return View("Index");        
            }   
            // Otherwise, we have a user, now we need to check their password                 
            // Initialize hasher object        
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();                    
            // Verify provided password against hash stored in db        
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);                                    

            if(result == 0)        
            {            
                ModelState.AddModelError("LoginPassword", "Invalid Email/Password");            
                return View("Index");        
            } 
            
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            
            return RedirectToAction( "AllProjects", "Project");
        } else 
        {
            return View("Index");
        }
    }

    
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}