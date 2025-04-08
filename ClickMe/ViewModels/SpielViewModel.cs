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

    private Timer _sprungTimer;
    private readonly Random _random = new();
    private int _punkte;
    private int _zeit;
    private double _formX, _formY;
    private CancellationTokenSource _cts;
    private bool _isGameRunning;
    private string _spielerName;
    private int _countdownSeconds;

    public int GameDurationSeconds { get; set; } = 60;
    public int CountdownSeconds
    {
        get => _countdownSeconds;
        set
        {
            if (_countdownSeconds != value)
            {
                _countdownSeconds = value;
                OnPropertyChanged();
            }
        }
    }
    public int MinTeleportDistance { get; set; } = 50;
    public double ScreenWidth { get; set; } = 300;
    public double ScreenHeight { get; set; } = 500;

    public string SpielerName
    {
        get => _spielerName;
        set
        {
            if (_spielerName != value)
            {
                _spielerName = value;
                OnPropertyChanged();
            }
        }
    }

    public int Punkte
    {
        get => _punkte;
        private set
        {
            if (_punkte != value)
            {
                _punkte = value;
                OnPropertyChanged();
            }
        }
    }

    public int Zeit
    {
        get => _zeit;
        private set
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
        private set
        {
            if (_formX != value)
            {
                _formX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormPosition));
            }
        }
    }

    public double FormY
    {
        get => _formY;
        private set
        {
            if (_formY != value)
            {
                _formY = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormPosition));
            }
        }
    }

    public Rect FormPosition => new Rect(FormX, FormY, 80, 80);

    public SpielViewModel()
    {
        FormKlicktCommand = new Command(FormKlickt);
        PointerNearCommand = new Command(PointerNear);
    }

    public void Initialize(string spielerName)
    {
        SpielerName = spielerName;
        StartNewGame();
    }

    public void StartNewGame()
    {
        Punkte = 0;
        Zeit = GameDurationSeconds;
        _isGameRunning = false;

        _cts?.Cancel();
        _sprungTimer?.Stop();

        StartCountdown();
    }

    private async void StartCountdown()
    {
        for (int i = 5; i > 0; i--)
        {
            CountdownSeconds = i;
            await Task.Delay(1000);
        }

        StartGame();
    }

    private void StartGame()
    {
        _isGameRunning = true;
        _cts = new CancellationTokenSource();

        _sprungTimer = new Timer(800);
        _sprungTimer.Elapsed += (s, e) => TeleportForm();
        _sprungTimer.AutoReset = true;
        _sprungTimer.Start();

        Task.Run(() => RunGameTimer());
        TeleportForm();
    }

    private void TeleportForm()
    {
        if (!_isGameRunning) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            FormX = _random.Next(0, (int)(ScreenWidth - 80));
            FormY = _random.Next(0, (int)(ScreenHeight - 80));
        });
    }

    private async Task RunGameTimer()
    {
        int remainingTime = GameDurationSeconds;

        while (remainingTime > 0 && !_cts.Token.IsCancellationRequested)
        {
            await Task.Delay(1000);
            remainingTime--;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Zeit = remainingTime;
            });
        }

        EndGame();
    }

    private void EndGame()
    {
        _isGameRunning = false;
        _sprungTimer?.Stop();
        _cts?.Cancel();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.DisplayAlert("Spiel beendet", $"Punkte: {Punkte}", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        });
    }

    private void FormKlickt()
    {
        if (!_isGameRunning) return;

        Punkte++;
        TeleportForm();
    }

    private void PointerNear()
    {
        if (!_isGameRunning) return;

        FormX += _random.Next(-MinTeleportDistance, MinTeleportDistance);
        FormY += _random.Next(-MinTeleportDistance, MinTeleportDistance);

        FormX = Math.Max(0, Math.Min(ScreenWidth - 80, FormX));
        FormY = Math.Max(0, Math.Min(ScreenHeight - 80, FormY));
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}