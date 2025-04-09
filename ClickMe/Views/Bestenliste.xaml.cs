using ClickMe.ViewModels;

namespace ClickMe.Views;

public partial class Bestenliste : ContentPage
{
    public Bestenliste()
    {
        InitializeComponent();
        BindingContext = new BestenlisteViewModel();
    }
}