#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JumpStarter.Models;


public class UserAndProject
{
    [Key]
    public int UserAndProjectId {get;set;}

    public int UserId{get;set;}
    public int ProjectId{get;set;}

    public int Donations {get;set;}

    public Project? Project { get; set; }    
    public User? User { get; set; }
}