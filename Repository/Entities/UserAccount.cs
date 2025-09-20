namespace Repository.Entities;

public class UserAccount
{
    public int UserAccountId { get; set; }

    public string UserPassword { get; set; } = null!;

    public string UserFullName { get; set; } = null!;

    public string? UserEmail { get; set; }

    public int? Role { get; set; }
}