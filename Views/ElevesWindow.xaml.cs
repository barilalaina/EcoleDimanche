using System.Windows;
using System.Windows.Controls;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class ElevesWindow : Window
{
    private List<Classe> _classes = new();

    public ElevesWindow()
    {
        InitializeComponent();
        ChargerFiltreClasses();
        Charger();
    }

    private void ChargerFiltreClasses()
    {
        _classes = ClasseService.GetAll().ToList();
        var options = new List<Classe> { new() { Id = 0, NomClasse = "Toutes les classes" } };
        options.AddRange(_classes);
        CboFiltreClasse.ItemsSource = options;
        CboFiltreClasse.SelectedIndex = 0;
    }

    private int? ClasseSelectionnee()
    {
        if (CboFiltreClasse.SelectedItem is Classe c && c.Id != 0)
            return c.Id;
        return null;
    }

    private void Charger() =>
        Grille.ItemsSource = EleveService.GetAll(TxtRecherche.Text, ClasseSelectionnee());

    private void Rechercher(object sender, TextChangedEventArgs e) => Charger();

    private void FiltrerClasse(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded) Charger();
    }

    private void Ajouter(object sender, RoutedEventArgs e)
    {
        var boite = new EleveDialog(null, _classes) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Modifier(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not Eleve eleve)
        {
            MessageBox.Show("Sélectionnez un élève à modifier.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var boite = new EleveDialog(eleve, _classes) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Supprimer(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not Eleve eleve)
        {
            MessageBox.Show("Sélectionnez un élève à supprimer.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var confirmation = MessageBox.Show(
            $"Supprimer l'élève « {eleve.NomComplet} » ?\nSes enregistrements de présence seront aussi supprimés.",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirmation != MessageBoxResult.Yes) return;

        try
        {
            EleveService.Supprimer(eleve.Id);
            Charger();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
