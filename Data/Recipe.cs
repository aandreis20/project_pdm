namespace RecipesApp.Data;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("recipes")]
public sealed class Recipe : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("ingredients")]
    public string? Ingredients { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("prep_time")]
    public int? PrepTime { get; set; }

    [Column("calories")]
    public int? Calories { get; set; }
}
