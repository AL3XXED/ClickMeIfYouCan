using System.Windows.Input;
using ClickMe.Views;

namespace ClickMe.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private string _spielerName;
    public string SpielerName
    {
        get => _spielerName;
        set
        {
            _spielerName = value;
            OnPropertyChanged(SpielerName);
        }
    }

    public ICommand StarteSpiel { get; }
    public ICommand ZeigeListe { get; }
    public ICommand ZeigeOptionen { get; }
    public ICommand Beenden { get; }

    public MainViewModel()
    {
        StarteSpiel = new Command(async () => await GoToSpielPage());
        ZeigeListe = new Command(async () => await Shell.Current.GoToAsync(nameof(Bestenliste)));
        ZeigeOptionen = new Command(async () => await Shell.Current.GoToAsync(nameof(Optionen)));
        Beenden = new Command(Application.Current.Quit);
    }


    private async Task GoToSpielPage()
    {
         SpielerName = await Shell.Current.DisplayPromptAsync(
                "Spielername",
                "Gib deinen Namen ein:",
                "OK",
                "Abbrechen",
                maxLength: 20);
        if (!string.IsNullOrWhiteSpace(SpielerName))
        {
            await Shell.Current.GoToAsync($"{nameof(SpielPage)}?playerName={SpielerName}");

        }
    }
}

