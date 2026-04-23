namespace himchistka.practic.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsBlocked { get; set; }
    public string Role => IsAdmin ? "Admin" : "User";
}
