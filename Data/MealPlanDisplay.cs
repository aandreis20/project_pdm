using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesApp.Data
{
    public class MealPlanDisplay
    {
        public MealPlan Plan { get; set; }
        public Recipe Recipe { get; set; } // We will attach this manually
    }
}
