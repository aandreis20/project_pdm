using System;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipesApp.ViewModels;

namespace RecipesApp.Views;

public partial class PlannerPage : ContentPage
{
    public PlannerPage(PlannerPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Appearing += PlannerPage_Appearing;
    }

    private async void PlannerPage_Appearing(object? sender, EventArgs e)
    {
        if (BindingContext is PlannerPageViewModel vm)
        {
            try
            {
                if (vm.LoadMealsCommand is IAsyncRelayCommand asyncCmd)
                    await asyncCmd.ExecuteAsync(null);
                else
                    vm.LoadMealsCommand.Execute(null);
            }
            catch (Exception)
            {
                // swallow here for the simple test view; errors are shown by the ViewModel
            }
        }
    }

    private async void OnAddMealClicked(object? sender, EventArgs e)
    {
        // resolve add page from DI (registered in MauiProgram)
        var addPage = MauiProgram.Services.GetService<AddMealPage>();
        if (addPage == null)
        {
            await DisplayAlert("Error", "Unable to open Add Meal page.", "OK");
            return;
        }

        // If both viewmodels are available, pass the currently selected date from Planner -> Add VM
        if (BindingContext is PlannerPageViewModel plannerVm &&
            addPage.BindingContext is AddMealPlanViewModel addVm)
        {
            addVm.SelectedDate = plannerVm.SelectedDate;
        }

        // when the addPage disappears, refresh meals
        addPage.Disappearing += async (_, __) =>
        {
            if (BindingContext is PlannerPageViewModel vm)
            {
                await vm.LoadMealsAsync();
            }
        };

        // show as modal
        await Navigation.PushModalAsync(addPage);
    }
}