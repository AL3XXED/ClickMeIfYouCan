using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMe.ViewModels;

public partial class OptionenViewModel : BaseViewModel
{
    private int _schwierigkeit;
    public int Schwierigkeit
    {
        get => _schwierigkeit;
        set
        {
            if (_schwierigkeit != value)
            {
                _schwierigkeit = value;
                Preferences.Set("Schwierigkeit", value);
                EinstellungenAnwenden();
                OnPropertyChanged();
                OnPropertyChanged(nameof(SchwierigkeitText));
            }
        }
    }

    public string SchwierigkeitText
    {
        get
        {
            return Schwierigkeit switch
            {
                1 => "Einsteiger",
                2 => "Fortgeschritten",
                3 => "Experte",
                _ => "Unbekannt"
            };
        }
    }

    public double FormGroesse { get; set; }
    public double TeleportInterval { get; set; }
    public double BasisGeschwindigkeit { get; set; }
    public int TeleportTimerInterval { get; set; }

    public OptionenViewModel()
    {
        Schwierigkeit = Preferences.Get("Schwierigkeit", 1);
        EinstellungenAnwenden();
    }

    private void EinstellungenAnwenden()
    {
        switch (Schwierigkeit)
        {
            case 1:
                FormGroesse = 80;
                TeleportInterval = 5.0;
                BasisGeschwindigkeit = 5.0;
                TeleportTimerInterval = 1800;
                break;
            case 2:
                FormGroesse = 55;
                TeleportInterval = 3.5;
                BasisGeschwindigkeit = 7.0;
                TeleportTimerInterval = 1200;
                break;
            case 3:
                FormGroesse = 40;
                TeleportInterval = 1.5;
                BasisGeschwindigkeit = 10.0;
                TeleportTimerInterval = 800;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Schwierigkeit), "Unbekannte Schwierigkeit");
        }
    }

    public List<string> SchwierigkeitTextOptions => new()
    {
        "Einsteiger",
        "Fortgeschritten",
        "Experte"
    };
}

