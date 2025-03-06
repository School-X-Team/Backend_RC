using System.ComponentModel.DataAnnotations;

namespace Backend_RC.Models;

public class RegisterModel
{
    [Required]
    public string FirstName { get; set; }

    [Required] 
    public string LastName { get; set;}

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
public class RegisterStep2Model
{
    public string Email
    {
        get; set;
    }
    public string FirstName
    {
        get; set;
    }
    public string LastName
    {
        get; set;
    }
    public string VerificationCode
    {
        get; set;
    }
}
