using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class PresencesWindow : Window
{
    private List<Classe> _classes = new();
    private ObservableCollection<Presence> _presences = new();

    public PresencesWindow()
    {
        InitializeComponent();

        var options = new List<Classe> { new() { Id = 0, NomClasse = "Toutes les classes" } };
        options.AddRange(ClasseService.GetAll());
        _classes = options;
        CboClasse.ItemsSource = _classes;
        CboClasse.SelectedIndex = 0;

        DtpDate.SelectedDate = DateTime.Parse(StatsService.DernierDimanche());
        // SelectedDateChanged déclenche le premier chargement
    }

    private string DateChoisie() =>
        (DtpDate.SelectedDate ?? DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private int? ClasseChoisie()
    {
        if (CboClasse.SelectedItem is Classe c && c.Id != 0)
            return c.Id;
        return null;
    }

    private void Charger()
    {
        if (!IsLoaded && DtpDate.SelectedDate == null) return;
        _presences = PresenceService.GetFeuillePresence(DateChoisie(), ClasseChoisie());
        Grille.ItemsSource = _presences;
        MettreAJourCompteur();
    }

    private void MettreAJourCompteur()
    {
        var presents = _presences.Count(p => p.EstPresent);
        TxtCompteur.Text = $"{presents} présent(s) / {_presences.Count} élève(s)";
    }

    private void Date_Changed(object? sender, SelectionChangedEventArgs e) => Charger();

    private void Classe_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded) Charger();
    }

    private void Grille_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        // Diffère la mise à jour du compteur après validation de la cellule
        Dispatcher.BeginInvoke(new Action(MettreAJourCompteur),
            System.Windows.Threading.DispatcherPriority.Background);
    }

    private void Enregistrer(object sender, RoutedEventArgs e)
    {
        Grille.CommitEdit(DataGridEditingUnit.Row, true);
        try
        {
            foreach (var presence in _presences)
                PresenceService.Enregistrer(presence);

            MettreAJourCompteur();
            MessageBox.Show("Les présences ont été enregistrées.", "Succès",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
