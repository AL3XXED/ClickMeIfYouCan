using System.Text.Json;
using System.Collections.ObjectModel;
using ClickMe.ViewModels;
using System.Diagnostics;
using ClickMe.Models;

namespace ClickMe.ViewModels;

public partial class BestenlisteViewModel : BaseViewModel
{
    private const int MaxEintraege = 10;
    private static List<Spieler> _bestenliste = new();
    private static readonly string SpeicherPfad = Path.Combine(FileSystem.AppDataDirectory, "bestenliste.json");

    private ObservableCollection<Spieler> _bestenlisteObservable = new();

    public ObservableCollection<Spieler> BestenlisteObservable
    {
        get => _bestenlisteObservable;
        set
        {
            if (_bestenlisteObservable == value)
                return;

            _bestenlisteObservable = value;
            OnPropertyChanged();
        }
    }

    public BestenlisteViewModel()
    {
        LadeBestenliste();
        AktualisiereBestenliste();
    }

    public void EintragHinzufuegen(string spielerName, int punkte)
    {
        if (string.IsNullOrWhiteSpace(spielerName))
            throw new ArgumentException("Spielername darf nicht leer sein.");

        if (punkte < 0)
            throw new ArgumentException("Punkte dürfen nicht negativ sein");

        var vorhandenerEintrag = _bestenliste.FirstOrDefault(e => e.SpielerName.Equals(spielerName, StringComparison.OrdinalIgnoreCase));

        if (vorhandenerEintrag != null)
        {
            if (punkte > vorhandenerEintrag.Punktzahl)
            {
                vorhandenerEintrag.Punktzahl = punkte;
                SpeichereBestenliste();
                AktualisiereBestenliste();
            }
        }
        else
        {
            _bestenliste.Add(new Spieler
            {
                SpielerName = spielerName.Trim(),
                Punktzahl = punkte
            });

            SpeichereBestenliste();
            AktualisiereBestenliste();
        }
    }

    private static void LadeBestenliste()
    {
        if (File.Exists(SpeicherPfad))
        {
            try
            {
                string json = File.ReadAllText(SpeicherPfad);
                var geladeneListe = JsonSerializer.Deserialize<List<Spieler>>(json);
                if (geladeneListe != null)
                    _bestenliste = geladeneListe;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Laden der Bestenliste: {ex.Message}");
            }
        }
    }

    private void SpeichereBestenliste()
    {
        try
        {
            _bestenliste = _bestenliste
                .OrderByDescending(e => e.Punktzahl)
                .Take(MaxEintraege)
                .ToList();

            string json = JsonSerializer.Serialize(_bestenliste);
            File.WriteAllText(SpeicherPfad, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void AktualisiereBestenliste()
    {
        var topEintraege = _bestenliste
            .OrderByDescending(e => e.Punktzahl)
            .Take(MaxEintraege)
            .ToList();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            BestenlisteObservable = new ObservableCollection<Spieler>(topEintraege);
        });
    }
}
