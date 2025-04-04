using ClickMe.ViewModels;

namespace ClickMe.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel mvm)
	{
		InitializeComponent();

        BindingContext = mvm;
    }
}