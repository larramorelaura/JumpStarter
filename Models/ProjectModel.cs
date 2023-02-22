#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JumpStarter.Models;


public class Project
{
    [Key]
    public int ProjectId{get;set;}

    [Required(ErrorMessage =" is required")]
    public string Title{get;set;}

    [Required(ErrorMessage =" is required")]
    [Range(0, int.MaxValue, ErrorMessage = " must be greater than 0")]
    public int Goal{get;set;}

    [Display(Name ="End Date")]
    [Required(ErrorMessage =" is required")]
    [FutureDate]
    public DateTime EndDate {get;set;}

    [Required(ErrorMessage =" is required")]
    [MinLength(20, ErrorMessage = " must be at least 20 characters")]
    public string Description{get;set;}

    
    [ForeignKey("Creator")]
    public int CreatorId { get; set; }
    public User? Creator { get; set; }
    
    // public int? Pledges {get;set;}

    public List<UserAndProject>? Supporters { get; set; } = new List<UserAndProject>();

    [NotMapped]
    public int TotalDonations
    {
        get { return Supporters.Sum(s => s.Donations); }
    }
    
    public DateTime CreatedAt{get;set;} =DateTime.Now;
    public DateTime UpdatedAt{get;set;} =DateTime.Now;
}


public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Convert the value to a DateTime? (nullable)
        var dateValue = value as DateTime?;

        // Check if the value is null or in the past
        if (dateValue == null || dateValue.Value <= DateTime.Now.Date)
        {
            return new ValidationResult(ErrorMessage ?? "The date must be in the future.");
        }

        // The value is valid
        return ValidationResult.Success;
    }
}