namespace RecipesApp.Data;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("meal_plans")]
public sealed class MealPlan : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("recipe_id")]
    public Guid RecipeId { get; set; }

    [Column("plan_date")]
    public DateTime PlanDate { get; set; }

    [Column("meal_type")]
    public string MealType { get; set; } = string.Empty;
}
