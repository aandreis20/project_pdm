using System.Globalization;

namespace RecipesApp.Converters;

public class SelectedToColorConverter : BindableObject, IValueConverter
{
    // These must be BindableProperties to support {AppThemeBinding}
    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(SelectedToColorConverter), Colors.Transparent);

    public static readonly BindableProperty UnselectedColorProperty =
        BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(SelectedToColorConverter), Colors.Transparent);

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public Color UnselectedColor
    {
        get => (Color)GetValue(UnselectedColorProperty);
        set => SetValue(UnselectedColorProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? SelectedColor : UnselectedColor;
        }
        return UnselectedColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}