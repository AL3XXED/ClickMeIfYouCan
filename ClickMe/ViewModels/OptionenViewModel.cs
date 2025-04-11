using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls;

namespace ClickMe.ViewModels;

public class ImageOption : INotifyPropertyChanged
{
    public string Name { get; set; }
    public string ImagePath { get; set; } 
    public ImageSource Image => ImageSource.FromFile(ImagePath);

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class OptionenViewModel : BaseViewModel
{
    public ObservableCollection<ImageOption> AvailableImages { get; } = new();

    private ImageOption _selectedImage;
    public ImageOption SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (_selectedImage != value)
            {
                _selectedImage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableImages)); 
                Preferences.Set("SelectedImageName", value?.Name ?? string.Empty);
            }
        }
    }

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
            }
        }
    }

    public double FormGroesse { get; set; }
    public double TeleportInterval { get; set; }
    public double BasisGeschwindigkeit { get; set; }
    public int TeleportTimerInterval { get; set; }

    public OptionenViewModel()
    {
        LoadAvailableImages();
        LoadSavedPreferences();
        EinstellungenAnwenden();
    }

    private void LoadAvailableImages()
    {
        try
        {
            AvailableImages.Clear();

            
            AvailableImages.Add(new ImageOption { Name = "Shuriken", ImagePath = "shuriken.png" });
            AvailableImages.Add(new ImageOption { Name = "Knife", ImagePath = "knife.png" });
            AvailableImages.Add(new ImageOption { Name = "Sharingan", ImagePath = "sharingan.png" });
        }
        catch (Exception)
        {
            AvailableImages.Add(new ImageOption { Name = "Default", ImagePath = "shuriken.png" });
        }
    }



    private void LoadSavedPreferences()
    {
        Schwierigkeit = Preferences.Get("Schwierigkeit", 1);

        var savedImageName = Preferences.Get("SelectedImageName", string.Empty);
        if (!string.IsNullOrEmpty(savedImageName))
        {
            SelectedImage = AvailableImages.FirstOrDefault(img => img.Name == savedImageName) ?? AvailableImages.First();
        }
        else
        {
            SelectedImage = AvailableImages.First();
        }
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
}