using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesApp.Services
{
    public class SettingsService : ISettingsService
    {
        public string ThemePreference
        {
            // Get the saved theme, or default to "System" if the user has never picked one
            get => Preferences.Default.Get(nameof(ThemePreference), "System");

            // Save the theme to local storage
            set => Preferences.Default.Set(nameof(ThemePreference), value);
        }
    }
}
