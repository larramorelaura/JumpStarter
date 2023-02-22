#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace JumpStarter.Models;

public class ViewModel
{
    
    public Project? Project{get;set;}
    public List<Project>? Projects{get;set;}
    public UserAndProject? UserAndProject{get;set;}
    public int SupportersCount { get; set; }
    public decimal TotalDonations { get; set; }
    public TimeSpan UntilEnd { get; set; }
}