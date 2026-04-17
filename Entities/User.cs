namespace MicoHoodApi.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}
