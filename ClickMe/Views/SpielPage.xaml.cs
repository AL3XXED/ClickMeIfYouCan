using ClickMe.ViewModels;

namespace ClickMe.Views;

[QueryProperty(nameof(PlayerName), "playerName")]
public partial class SpielPage : ContentPage
{
    private string _playerName;

    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            if (BindingContext is SpielViewModel vm)
            {
                vm.Initialize(_playerName);
            }
        }
    }

    public SpielPage()
    {
        InitializeComponent();
        BindingContext = new SpielViewModel(Dispatcher);
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
}