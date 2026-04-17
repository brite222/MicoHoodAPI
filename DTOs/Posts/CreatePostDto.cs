using System.ComponentModel.DataAnnotations;

namespace MicoHood.API.DTOs.Posts;

public class CreatePostDto
{
    [Required]
    [MinLength(3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(10)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;
}