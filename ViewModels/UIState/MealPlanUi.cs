namespace RecipesApp.ViewModels.UIState;

public class MealPlanUi
{
    public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snacks
    public string TimeLabel { get; set; }
    public RecipeUi? Recipe { get; set; } // Null if the slot is empty
    public bool IsEmpty => Recipe == null;
}