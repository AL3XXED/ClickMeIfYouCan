using ClickMe.ViewModels;

namespace ClickMe.Views;

public partial class Optionen : ContentPage
{
	public Optionen()
	{
		InitializeComponent();
		BindingContext = new OptionenViewModel();
	}
}