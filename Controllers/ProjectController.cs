using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;
using JumpStarter.Models;

namespace JumpStarter.Controllers;

public class ProjectController : Controller
{
    private readonly ILogger<ProjectController> _logger;
    
    private MyContext _context;  
    public ProjectController(ILogger<ProjectController> logger, MyContext context)    
    {        
        _logger = logger;
        _context = context;    
    } 

[SessionCheck]
    [HttpGet("projects")]
    public IActionResult AllProjects()
    {
        List<Project> allProjects = _context.Projects
                .Include(p => p.Creator)
                .Include(p => p.Supporters)
                    .ThenInclude(s => s.User)
                .ToList();

            foreach (var project in allProjects)
            {
                project.Supporters = project.Supporters
                    .Select(s => new UserAndProject
                    {
                        User = s.User,
                        Donations = s.Donations
                    })
                    .ToList();
            }
            return View("AllProjects", allProjects);
    }

[SessionCheck]
    [HttpGet("projects/new")]
    public IActionResult NewProject()
    {
        return View("NewProject");
    }

    [HttpPost("projects/new")]
    public IActionResult CreateProject(Project newProject)
    {

        int? userId= HttpContext.Session.GetInt32("UserId");

        if(!ModelState.IsValid)
        {
            return View("NewProject");
        }

        User? creator= _context.Users.FirstOrDefault(u=> u.UserId==userId);
        // newProject.CreatorId=(int)userId;
        newProject.Creator=creator;
        _context.Projects.Add(newProject);
        _context.SaveChanges();

        return RedirectToAction("OneProject", new{projectId=newProject.ProjectId});
    }

[SessionCheck]
    [HttpPost("delete/{projectId}")]
    public IActionResult DeleteProject(int projectId)
    {
        Project? ToDelete= _context.Projects.FirstOrDefault(p => p.ProjectId== projectId);
        if(ToDelete != null)
        {
            _context.Projects.Remove(ToDelete);
            _context.SaveChanges();
        }
        
        return RedirectToAction("AllProjects");
    }

[SessionCheck]
    [HttpGet("projects/{projectId}")]
    public IActionResult OneProject(int projectId)
    {
        var projectInfo = _context.Projects
            .Where(p => p.ProjectId == projectId)
            .Select(p => new
            {
                Project = p,
                SupportersCount = p.Supporters.Any() ? p.Supporters.Count() : 0,
                TotalDonations = p.Supporters.Any() ? p.Supporters.Sum(s => s.Donations) : 0,
                EndDate=p.EndDate
            })
            .FirstOrDefault();
            
            
        ViewModel viewModel = new ViewModel
        {
            Project = projectInfo.Project,
            SupportersCount = projectInfo.SupportersCount,
            TotalDonations = projectInfo.TotalDonations,
            UntilEnd = projectInfo.EndDate - DateTime.Now
        };

return View("OneProject", viewModel);
    
    }

[SessionCheck]
    [HttpPost("support/{projectId}")]
    public IActionResult SupportProject(int projectId, ViewModel newViewModel)
    {
        
        int? userId = HttpContext.Session.GetInt32("UserId");
        
        int actualUserId = userId.Value;
        
        Project supportingProject= _context.Projects.Include(p=>p.Creator).FirstOrDefault(p=>p.ProjectId==projectId);

        if (DateTime.Now > supportingProject.EndDate)
        {
            return OneProject(projectId);
        }

        if(supportingProject.CreatorId==actualUserId){
                return OneProject(projectId);
            }
        else { UserAndProject newLink= new UserAndProject
        {
            UserId= actualUserId,
            ProjectId= projectId,
            Donations= newViewModel.UserAndProject.Donations
        };

        _context.UsersAndProjects.Add(newLink);
        _context.SaveChanges();

        return RedirectToAction("OneProject", new{projectId=projectId}); 
        }
        
    }   
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
