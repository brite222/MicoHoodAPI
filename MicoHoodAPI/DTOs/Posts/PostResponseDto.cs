namespace MicoHood.API.DTOs.Posts;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public bool LikedByCurrentUser { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}