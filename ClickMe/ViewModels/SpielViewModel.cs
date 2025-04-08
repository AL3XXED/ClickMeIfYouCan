using System.ComponentModel;
using System.Windows.Input;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Runtime.CompilerServices;

namespace ClickMe.ViewModels;

public class SpielViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand FormKlicktCommand { get; }
    public ICommand PointerNearCommand { get; }

    private  Timer _sprungTimer;
    private readonly Random zufall = new();
    private int _punkte;
    private string _zeit;
    private double _formX, _formY;

    private CancellationTokenSource _cts;

    public int Punkte
    {
        get => _punkte;
        set
        {
            if (_punkte != value)
            {
                _punkte = value;
                OnPropertyChanged();
            }
        }
    }


    public string Zeit
    {
        get => _zeit;
        set
        {
            if (_zeit != value)
            {
                _zeit = value;
                OnPropertyChanged();
            }
        }
    }
    public double FormX
    {
        get => _formX;
        set
        {
            if (_formX != value)
            {
                _formX = value;
                OnPropertyChanged();
            }
        }
    }
    public double FormY
    {
        get => _formY;
        set
        {
            if (_formY != value)
            {
                _formY = value;
                OnPropertyChanged();
            }
        }
    }

    public SpielViewModel()
    {
        FormKlicktCommand = new Command(FormKlickt);
        PointerNearCommand = new Command(PointerNear);
        Zeit = "5";
    }

    private async void StarteCountdown()
    {
        for (int i = 5; i > 0; i--)
        {
            Zeit = i.ToString();
            await Task.Delay(1000);
        }
        Zeit = "";
        StarteSpiel();
    }

    private void StarteSpiel()
    {
        Punkte = 0;
        _cts = new CancellationTokenSource();
        _sprungTimer = new Timer(1000);
        _sprungTimer.Elapsed += (s, e) => TeleportiereForm();
        _sprungTimer.Start();


    }

    private void TeleportiereForm()
    {
        FormX = zufall.Next(0, 300);
        FormY = zufall.Next(0, 500);
    }

    private async Task SpielTimer()
    {
        int spielzeit = 60;
        while (spielzeit > 0)
        {
            spielzeit--;
            await Task.Delay(1000);
        }

        _sprungTimer.Stop();
        _cts.Cancel();

        await Application.Current.MainPage.DisplayAlert("Spiel beendet", $"Punkte: {Punkte}", "OK");
        Application.Current.MainPage.Navigation.PopAsync();
    }

    private void FormKlickt()
    {
        Punkte++;
        TeleportiereForm();
    }
    private void PointerNear()
    {
        FormX = zufall.Next(-100, 100);
        FormY = zufall.Next(-100, 100);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
