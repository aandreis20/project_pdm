using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace RecipesApp.Data
{
    [Table("meal_plans")]
    public class MealPlanSimple : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("recipe_id")]
        public Guid RecipeId { get; set; }

        [Column("plan_date")]
        public DateOnly PlanDate { get; set; }

        [Column("meal_type")]
        public string? MealType { get; set; }
    }
}