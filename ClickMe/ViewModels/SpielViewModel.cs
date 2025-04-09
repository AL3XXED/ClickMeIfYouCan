using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using ClickMe.Views;
using Timer = System.Timers.Timer;

namespace ClickMe.ViewModels
{
    public partial class SpielViewModel : BaseViewModel
    {
        public ICommand FormKlicktCommand { get; }
        public ICommand PointerNearCommand { get; }

        // Timer für  Teleportation
        private Timer _sprungTimer;

        // Animationstimer
        private readonly IDispatcherTimer _animationTimer;
        private CancellationTokenSource _cts;

        private readonly Random _random = new();
        private int _punkte;
        private int _zeit;
        private double _formX, _formY;
        private bool _isGameRunning;
        private string _spielerName;
        private int _countdownSeconds;
        private bool _countdownVisible = true;

        // Variablen für Bewegung
        private double _velocityX;
        private double _velocityY;
        private DateTime _lastTeleportTime;
        private const double TeleportInterval = 3.0;

        // Eigenschaften
        public int GameDurationSeconds { get; set; } = 60;
        public int Countdown { get; set; }
        public double FensterBreite { get; set; }
        public double FensterHoehe { get; set; }
        public double FormGroesse { get; set; } = 80; // Standardgröße
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

        public int Punktzahl
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
        public bool CountdownVisible
        {
            get => _countdownVisible;
            set
            {
                if (_countdownVisible != value)
                {
                    _countdownVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rect FormPosition => new Rect(FormX, FormY, FormGroesse, FormGroesse);

        public SpielViewModel(IDispatcher dispatcher)
        {
            FormKlicktCommand = new Command(FormKlickt);
            PointerNearCommand = new Command(PointerNear);

            _animationTimer = dispatcher.CreateTimer();
            _animationTimer.Interval = TimeSpan.FromMilliseconds(16); // ca. 60 FPS
            _animationTimer.Tick += AnimationsTick;
        }

        public void Initialize(string spielerName)
        {
            SpielerName = spielerName;
            FormX = 100;
            FormY = 100;
            CountdownSeconds = 3; // Direkt sichtbar
            StartCountdown();
        }

        private async void StartCountdown()
        {
            CountdownVisible = true;

            for (int i = 5; i > 0; i--)
            {
                CountdownSeconds = i;
                await Task.Delay(1000);
            }

            CountdownVisible = false;
            StartGame();
            }

        private void StartGame()
        {
            _isGameRunning = true;
            Punktzahl = 0;
            _cts = new CancellationTokenSource();

            //  Zufallsrichtung für die Animation
            _velocityX = (_random.NextDouble() - 0.5) * 10;
            _velocityY = (_random.NextDouble() - 0.5) * 10;
            _lastTeleportTime = DateTime.Now;

            _animationTimer.Start();

            // Teleport-Timer
            _sprungTimer = new Timer(1800);
            _sprungTimer.Elapsed += (s, e) => TeleportForm();
            _sprungTimer.AutoReset = true;
            _sprungTimer.Start();


            Task.Run(() => RunGameTimer());
            TeleportForm();
        }

        private void AnimationsTick(object sender, EventArgs e)
        {
            if (!_isGameRunning)
                return;

            // Neue Position
            FormX += _velocityX;
            FormY += _velocityY;

            // Überprüfe Kollision mit den Rändern (X-Achse)
            if (FormX <= 0 || FormX >= ScreenWidth - FormGroesse)
            {
                _velocityX = -_velocityX * 0.9;
                FormX = Math.Clamp(FormX, 0, ScreenWidth - FormGroesse);
            }

            // Überprüfe Kollision mit den Rändern (Y-Achse)
            if (FormY <= 0 || FormY >= ScreenHeight - FormGroesse)
            {
                _velocityY = -_velocityY * 0.9;
                FormY = Math.Clamp(FormY, 0, ScreenHeight - FormGroesse);
            }

            if ((DateTime.Now - _lastTeleportTime).TotalSeconds > TeleportInterval && _random.NextDouble() < 0.2)
            {
                TeleportForm();
                _lastTeleportTime = DateTime.Now;
            }
        }

        private void TeleportForm()
        {
            if (!_isGameRunning)
                return;

            // UI-Aktualisierungen auf dem Hauptthread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                FormX = _random.Next(0, (int)(ScreenWidth - FormGroesse));
                FormY = _random.Next(0, (int)(ScreenHeight - FormGroesse));

                // Neue, leicht höhere Geschwindigkeiten
                _velocityX = (_random.NextDouble() - 0.5) * 15;
                _velocityY = (_random.NextDouble() - 0.5) * 15;
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

        private void FormKlickt()
        {
            if (!_isGameRunning)
                return;

            Punktzahl++;
            TeleportForm();
        }

        private void PointerNear()
        {
            if (!_isGameRunning)
                return;

            FormX += _random.Next(-MinTeleportDistance, MinTeleportDistance);
            FormY += _random.Next(-MinTeleportDistance, MinTeleportDistance);

            FormX = Math.Clamp(FormX, 0, ScreenWidth - FormGroesse);
            FormY = Math.Clamp(FormY, 0, ScreenHeight - FormGroesse);
        }

        private async Task EndGame()
        {
            _isGameRunning = false;
            _sprungTimer?.Stop();
            _sprungTimer?.Dispose();
            _cts?.Cancel();

            _animationTimer.Stop();

            try
            {
                var bestenliste = new BestenlisteViewModel();
                bestenliste.EintragHinzufuegen(SpielerName, Punktzahl);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Spiel beendet",
                        $"{SpielerName}, du hast {Punktzahl} Punkte erreicht!",
                        "Ok");

                    if (Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
                        await Application.Current.MainPage.Navigation.PopAsync();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EndGame Exception] {ex}");
            }
        }
    }
}