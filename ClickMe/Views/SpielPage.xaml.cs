using ClickMe.ViewModels;
using Microsoft.Maui.Controls;

namespace ClickMe.Views;

public partial class SpielPage : ContentPage
{
    public SpielPage()
    {
        InitializeComponent();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (BindingContext is SpielViewModel vm)
        {
            vm.ScreenWidth = width;
            vm.ScreenHeight = height;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is SpielViewModel vm)
        {
            vm.StartNewGame();
        }
    }
}