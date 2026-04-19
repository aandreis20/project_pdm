using RecipesApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesApp.Services
{
    public interface IMealPlanService
    {
        Task<List<MealPlanDisplay>> GetMealPlansForDateAsync(DateTime date);
        Task<bool> AddToMealPlanAsync(string recipeId, DateTime date, string mealType);
    }
}
