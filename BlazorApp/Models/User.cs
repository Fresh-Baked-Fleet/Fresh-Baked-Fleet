using Microsoft.AspNetCore.Identity;

namespace BlazorApp.Models
{
  public enum ContactPreference
  {
    Email,
    PhoneNumber
  }

  public class User : IdentityUser
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ContactPreference Preference { get; set; } = ContactPreference.Email;
  }
}