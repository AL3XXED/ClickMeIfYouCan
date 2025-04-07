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
    private Timer _spielTimer;
    private Timer _sprungTimer;
    private IDispatcherTimer _bewegungsTimer;
    private int _spielDauer = 60;
    private int _restlicheZeit;
    private int _schwierigkeit = 1;
    private double _buttonX;
    private double _buttonY;
    private double _buttonSpeedX;
    private double _buttonSpeedY;
    private bool _spielLauft;

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

    public SpielViewModel()
    {
        ButtonClickedCommand = new Command(OnButtonClicked);

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

        _bewegungsTimer = Application.Current.Dispatcher.CreateTimer();
        _bewegungsTimer.Interval = TimeSpan.FromMilliseconds(16);
        _bewegungsTimer.Tick += BewegungsTimer_Tick;

        _sprungTimer = new Timer();
        _sprungTimer.Elapsed += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(BewegeButton);
        };
    }

    public void StartGame(string spielerName, int schwierigkeit)
    {
        SpielerName = spielerName;
        Schwierigkeit = schwierigkeit;
        Punkte = 0;
        Zeit = SpielDauer;
        _spielLauft = true;

        double geschwindigkeit = Schwierigkeit switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            _ => 1
        };

        var zufall = new Random();
        _buttonSpeedX = geschwindigkeit * (zufall.NextDouble() < 0.7 ? zufall.Next(2, 4) : 1) * (zufall.Next(2) == 0 ? -1 : 1);
        _buttonSpeedY = geschwindigkeit * (zufall.NextDouble() < 0.7 ? zufall.Next(2, 4) : 1) * (zufall.Next(2) == 0 ? -1 : 1);

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        ButtonX = (displayInfo.Width / displayInfo.Density - 100) / 2;
        ButtonY = (displayInfo.Height / displayInfo.Density - 100) / 2;

        _spielTimer.Start();
        _bewegungsTimer.Start();

        if (schwierigkeit >= 2)
        {
            _sprungTimer.Interval = schwierigkeit == 2 ? 500 : 250;
            _sprungTimer.Start();
        }
    }

    private void BewegungsTimer_Tick(object sender, EventArgs e)
    {
        if (_spielLauft) return;

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        double width = displayInfo.Width / displayInfo.Density;
        double height = displayInfo.Height / displayInfo.Density;

        double newX = ButtonX + _buttonSpeedX;
        double newY = ButtonY + _buttonSpeedY;

        if (newX <= 0 || newX >= width - 100)
        {
            _buttonSpeedX = -_buttonSpeedX;
            newX = Math.Clamp(newX, 0, width - 100);
        }

        if (newY <= 0 || newY >= height - 100)
        {
            _buttonSpeedY = -_buttonSpeedY;
            newY = Math.Clamp(newY, 0, height - 100);
        }

        ButtonX = newX;
        ButtonY = newY;
        AbsoluteLayout.SetLayoutBounds((Button)sender, new(ButtonX, ButtonY, 100, 100));
    }

    private void BewegeButton()
    {
        var random = new Random();
        var width = Application.Current.MainPage.Width;
        var height = Application.Current.MainPage.Height;


        ButtonX = random.Next(0, (int)(width - 100));
        ButtonY = random.Next(0, (int)(height - 100));
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (!_spielLauft)
        {
            StartGame(SpielerName, Schwierigkeit);
        }
        else
        {
            Punkte++;
            _buttonSpeedX = -_buttonSpeedX;
            _buttonSpeedY = -_buttonSpeedY;

            if (!_bewegungsTimer.IsRunning)
                _bewegungsTimer.Start();
        }
    }

    private async void SpielEnde()
    {
        _spielLauft = false;
        _spielTimer.Stop();
        _bewegungsTimer.Stop();
        _sprungTimer.Stop();

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Spiel beendet", $"Du hast {Punkte} Punkte erreicht!", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        });
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var button = sender as Button;
        var position = e.GetPosition((Button)sender);

        if (position.HasValue)
        {
            double centerX = button.Width / 2;
            double centerY = button.Height / 2;
            if (position.Value.X < centerX)
                _buttonSpeedX = -Math.Abs(_buttonSpeedX);
            else
                _buttonSpeedX = Math.Abs(_buttonSpeedX);

            if (position.Value.Y < centerY)
                _buttonSpeedY = -Math.Abs(_buttonSpeedY);
            else
                _buttonSpeedY = Math.Abs(_buttonSpeedY);
        }
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}