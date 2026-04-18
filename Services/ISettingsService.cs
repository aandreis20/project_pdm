using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesApp.Services
{
    public interface ISettingsService
    {
        // We use a string so it can hold "Light", "Dark", or "System"
        string ThemePreference { get; set; }
    }
}
