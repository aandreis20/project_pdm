using RecipesApp.Data;
using RecipesApp.Services;

using System.Collections.ObjectModel;

namespace RecipesApp.Services;

public sealed class MealPlanService
{
    private readonly ISupabaseClientProvider supabase;

    public MealPlanService(ISupabaseClientProvider supabase)
    {
        this.supabase = supabase;
    }

    public async Task<IReadOnlyList<MealPlan>> GetMealPlansAsync()
    {
        await supabase.EnsureInitializedAsync();

        var response = await supabase.Client
            .From<MealPlan>()
            .Order(mp => mp.PlanDate, Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        return response.Models;
    }

    public async Task<IReadOnlyList<MealPlan>> GetMealPlansForUserAsync(Guid userId)
    {
        await supabase.EnsureInitializedAsync();

        var response = await supabase.Client
            .From<MealPlan>()
            .Where(x => x.UserId == userId)
            .Order(x => x.PlanDate, Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        return response.Models;
    }

    public async Task<MealPlan?> GetMealPlanByIdAsync(Guid id)
    {
        await supabase.EnsureInitializedAsync();

        var response = await supabase.Client.From<MealPlan>().Where(x => x.Id == id).Get();
        return response.Models.FirstOrDefault();
    }

    // Populate related Recipe for each MealPlan.
    // Avoids using a non-existing .In(...) method on the Supabase table by fetching recipes
    // and matching locally. This is compatible with the Supabase client used in the project.
    public async Task<IReadOnlyList<MealPlan>> GetMealPlansWithRecipesAsync()
    {
        await supabase.EnsureInitializedAsync();

        var mealPlansResp = await supabase.Client
            .From<MealPlan>()
            .Order(mp => mp.PlanDate, Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        var mealPlans = mealPlansResp.Models;
        if (mealPlans == null || mealPlans.Count == 0)
            return mealPlans;

        // Collect recipe ids
        var recipeIds = mealPlans.Select(mp => mp.RecipeId).Where(id => id != Guid.Empty).Distinct().ToList();
        if (recipeIds.Count == 0)
            return mealPlans;

        // The Supabase client in this workspace does not expose a working .In(...) on ISupabaseTable<T,...>.
        // Instead fetch recipes (could be scoped/filter later) and match locally.
        var allRecipesResp = await supabase.Client.From<Recipe>().Get();
        var recipes = allRecipesResp.Models ?? new List<Recipe>();

        var recipeDict = recipes.Where(r => recipeIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r);

        foreach (var mp in mealPlans)
        {
            if (recipeDict.TryGetValue(mp.RecipeId, out var recipe))
                mp.Recipe = recipe;
        }

        return mealPlans;
    }

    // Convenience used by PlannerPageViewModel: get mealplans for a user on a specific date, grouped by meal type
    public async Task<Dictionary<string, List<MealPlan>>> GetMealsGroupedByTypeAsync(Guid userId, DateOnly date)
    {
        await supabase.EnsureInitializedAsync();

        var response = await supabase.Client
            .From<MealPlan>()
            .Where(x => x.UserId == userId && x.PlanDate == date)
            .Order(x => x.PlanDate, Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        var mealPlans = response.Models ?? new List<MealPlan>();

        // populate recipes similarly to GetMealPlansWithRecipesAsync to avoid .In usage
        var recipeIds = mealPlans.Select(mp => mp.RecipeId).Where(id => id != Guid.Empty).Distinct().ToList();
        if (recipeIds.Count > 0)
        {
            var allRecipesResp = await supabase.Client.From<Recipe>().Get();
            var recipes = allRecipesResp.Models ?? new List<Recipe>();
            var recipeDict = recipes.Where(r => recipeIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r);

            foreach (var mp in mealPlans)
            {
                if (recipeDict.TryGetValue(mp.RecipeId, out var recipe))
                    mp.Recipe = recipe;
            }
        }

        var grouped = mealPlans
            .GroupBy(mp => string.IsNullOrWhiteSpace(mp.MealType) ? "Other" : mp.MealType!)
            .ToDictionary(g => g.Key, g => g.ToList());

        return grouped;
    }

    // Add or ensure this method exists in MealPlanService
    public async Task DeleteMealPlanAsync(Guid id)
    {
        await supabase.EnsureInitializedAsync();

        try
        {
            await supabase.Client
                .From<MealPlan>()
                .Where(x => x.Id == id)
                .Delete();
        }
        catch
        {
            // Rethrow to let the caller handle UI/errors
            throw;
        }
    }

    // Update the existing AddMealPlanAsync method in MealPlanService
    // Option B: Insert an explicit payload (recommended to avoid accidental mapping)
    public async Task AddMealPlanAsync(MealPlan mealPlan)
    {
        await supabase.EnsureInitializedAsync();

        var user = supabase.Client.Auth.CurrentUser;
        if (user != null && Guid.TryParse(user.Id, out var userId))
            mealPlan.UserId = userId;

        if (mealPlan.Id == Guid.Empty)
            mealPlan.Id = Guid.NewGuid();

        // Build a plain payload with only the DB columns
        var payload = new MealPlanSimple
        {
            Id = mealPlan.Id,
            UserId = mealPlan.UserId,
            RecipeId = mealPlan.RecipeId,
            PlanDate = mealPlan.PlanDate,
            MealType = mealPlan.MealType
        };



        await supabase.Client
            .From<MealPlanSimple>()
            .Insert(payload);
    }


}