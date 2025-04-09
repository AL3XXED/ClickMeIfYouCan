using System.Text.Json;
using System.Collections.ObjectModel;
using ClickMe.ViewModels;
using System.Diagnostics;

public class BestenlisteEintrag
{
    public string SpielerName { get; set; }
    public int Punkte { get; set; }
}
public partial class BestenlisteViewModel : BaseViewModel
{
    private static List<BestenlisteEintrag> _bestenliste = new();
    private static readonly string SpeicherPfad = Path.Combine(FileSystem.AppDataDirectory, "bestenliste.json");

    private ObservableCollection<BestenlisteEintrag> _bestenlisteObservable = new();

    public ObservableCollection<BestenlisteEintrag> BestenlisteObservable
    {
        get => _bestenlisteObservable;
        set
        {
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
        _bestenliste.Add(new BestenlisteEintrag
        {
            SpielerName = spielerName,
            Punkte = punkte
        });

        SpeichereBestenliste();
        AktualisiereBestenliste();
    }

    private void LadeBestenliste()
    {
        if (File.Exists(SpeicherPfad))
        {
            try
            {
                string json = File.ReadAllText(SpeicherPfad);
                var geladeneListe = JsonSerializer.Deserialize<List<BestenlisteEintrag>>(json);
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
        string json = JsonSerializer.Serialize(_bestenliste);
        File.WriteAllText(SpeicherPfad, json);
    }

    private void AktualisiereBestenliste()
    {
        BestenlisteObservable = new ObservableCollection<BestenlisteEintrag>(
            _bestenliste.OrderByDescending(e => e.Punkte));
    }
}
