namespace MicoHoodApi.Entities;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;  
    public int LikesCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public Guid UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}
