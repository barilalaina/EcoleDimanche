using System.Windows;
using System.Windows.Controls;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class MoniteursWindow : Window
{
    public MoniteursWindow()
    {
        InitializeComponent();
        Charger();
    }

    private void Charger() => Grille.ItemsSource = MoniteurService.GetAll(TxtRecherche.Text);

    private void Rechercher(object sender, TextChangedEventArgs e) => Charger();

    private void Ajouter(object sender, RoutedEventArgs e)
    {
        var boite = new MoniteurDialog(null) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Modifier(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not Moniteur m)
        {
            MessageBox.Show("Sélectionnez un moniteur à modifier.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var boite = new MoniteurDialog(m) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Supprimer(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not Moniteur m)
        {
            MessageBox.Show("Sélectionnez un moniteur à supprimer.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var confirmation = MessageBox.Show(
            $"Supprimer le moniteur « {m.NomComplet} » ?\nLes classes qu'il supervise perdront leur moniteur principal.",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirmation != MessageBoxResult.Yes) return;

        try
        {
            MoniteurService.Supprimer(m.Id);
            Charger();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
