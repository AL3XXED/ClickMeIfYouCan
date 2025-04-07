using ClickMe.ViewModels;

namespace ClickMe.Views;

public partial class SpielPage : ContentPage
{
    private SpielViewModel? ViewModel => BindingContext as SpielViewModel;
    public SpielPage()
    {
        InitializeComponent();
    }

    private void Bewegung(object sender, TappedEventArgs e)
    {
        if (ViewModel == null) return;

        var tapPosition = e.GetPosition((View)sender);
        if (tapPosition.HasValue)
        {
            ViewModel.HandleTap(new Point(tapPosition.Value.X, tapPosition.Value.Y));
        }
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        ViewModel?.ButtonClicked();
    }
    
}
