namespace MicoHood.API.DTOs.Posts;

public class PostQueryDto
{
    public string? Location { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}