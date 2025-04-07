using System.ComponentModel;
using System.Windows.Input;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Runtime.CompilerServices;

namespace ClickMe.ViewModels;

public class SpielViewModel : INotifyPropertyChanged
{
    private string _spielerName;
    private int _punkte;
    private int _spielDauer = 60;
    private int _restlicheZeit;
    private int _schwierigkeit = 1;
    private double _buttonX;
    private double _buttonY;
    private bool _spielLauft;

    private Timer _spielTimer;
    private Timer _sprungTimer;

    Random random = new Random();

    public event PropertyChangedEventHandler? PropertyChanged;

    // Properties
    public string SpielerName
    {
        get => _spielerName;
        set
        {
            if (_spielerName != value)
            {
                _spielerName = value;
                OnPropertyChanged(nameof(SpielerName));
            }
        }
    }

    public int Punkte
    {
        get => _punkte;
        set
        {
            if (_punkte != value)
            {
                _punkte = value;
                OnPropertyChanged(nameof(Punkte));
            }
        }
    }

    public int SpielDauer
    {
        get => _spielDauer;
        set
        {
            if (_spielDauer != value)
            {
                _spielDauer = value;
                OnPropertyChanged(nameof(SpielDauer));
            }
        }
    }

    public int Zeit
    {
        get => _restlicheZeit;
        set
        {
            if (_restlicheZeit != value)
            {
                _restlicheZeit = value;
                OnPropertyChanged(nameof(Zeit));
            }
        }
    }

    public int Schwierigkeit
    {
        get => _schwierigkeit;
        set
        {
            if (_schwierigkeit != value)
            {
                _schwierigkeit = value;
                OnPropertyChanged(nameof(Schwierigkeit));
            }
        }
    }

    public double ButtonX
    {
        get => _buttonX;
        set
        {
            if (_buttonX != value)
            {
                _buttonX = value;
                OnPropertyChanged(nameof(ButtonX));
            }
        }
    }

    public double ButtonY
    {
        get => _buttonY;
        set
        {
            if (_buttonY != value)
            {
                _buttonY = value;
                OnPropertyChanged(nameof(ButtonY));
            }
        }
    }

    public ICommand ButtonClickedCommand { get; }
    public ICommand TapCommand { get; }

    public SpielViewModel()
    {
        TapCommand = new Command<Point>(HandleTap);
        ButtonClickedCommand = new Command(ButtonClicked);

        _spielTimer = new Timer(1000);
        _spielTimer.Elapsed += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Zeit--;
                if (Zeit <= 0)
                    SpielEnde();
            });
        };

        _sprungTimer = new Timer
        {
            AutoReset = true
        };
        _sprungTimer.Elapsed += (s, e) => BewegeButton();
    }

    public void ButtonClicked()
    {
        if (!_spielLauft)
        {
            StartGame(SpielerName, Schwierigkeit);
        }
        else
        {
            Punkte++;
        }
    }
    public void StartGame(string spielerName, int schwierigkeit)
    {
        SpielerName = spielerName;
        Schwierigkeit = schwierigkeit;
        Punkte = 0;
        Zeit = SpielDauer;
        _spielLauft = true;

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        var width = displayInfo.Width / displayInfo.Density;
        var height = displayInfo.Height / displayInfo.Density;

        ButtonX = (width - 100) / 2;
        ButtonY = (height - 100) / 2;

        _spielTimer.Start();

        _sprungTimer.Interval = Schwierigkeit switch
        {
            1 => 1500,       
            2 => 600,     
            3 => 300,     
            _ => 1000     
        };
        _sprungTimer.Start();
        
    }

    private void BewegeButton()
    {
        if (!_spielLauft) return;

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        var width = displayInfo.Width / displayInfo.Density;
        var height = displayInfo.Height / displayInfo.Density;

        ButtonX = random.Next(20, (int)(width - 120));
        ButtonY = random.Next(20, (int)(height - 120));
    }

    public void HandleTap(Point tapPosition)
    {
        if (!_spielLauft) return;

        double strecke = Math.Sqrt(Math.Pow(tapPosition.X - ButtonX, 2) + Math.Pow(tapPosition.Y - ButtonY, 2));

        if (strecke < 25)
        {
            BewegeButton();
        }

    }

    private async void SpielEnde()
    {
        _spielLauft = false;
        _spielTimer.Stop();
        _sprungTimer.Stop();

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Application.Current.MainPage.DisplayAlert("Spiel beendet", $"Du hast {Punkte} Punkte erreicht!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        });
    }



    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
