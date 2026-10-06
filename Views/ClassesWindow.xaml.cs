using System.Windows;
using System.Windows.Controls;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class ClassesWindow : Window
{
    public ClassesWindow()
    {
        InitializeComponent();
        Charger();
    }

    private void Charger()
    {
        var classes = ClasseService.GetAll(TxtRecherche.Text);
        // Nombre d'élèves par classe pour l'affichage
        var vues = classes.Select(c => new ClasseVue
        {
            Id = c.Id,
            NomClasse = c.NomClasse,
            TrancheAge = c.TrancheAge,
            IdMoniteurPrincipal = c.IdMoniteurPrincipal,
            MoniteurPrincipal = c.MoniteurPrincipal,
            NombreEleves = ClasseService.NombreEleves(c.Id)
        }).ToList();
        Grille.ItemsSource = vues;
    }

    private void Rechercher(object sender, TextChangedEventArgs e) => Charger();

    private void Ajouter(object sender, RoutedEventArgs e)
    {
        var boite = new ClasseDialog(null) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Modifier(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not ClasseVue c)
        {
            MessageBox.Show("Sélectionnez une classe à modifier.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var boite = new ClasseDialog(c) { Owner = this };
        if (boite.ShowDialog() == true)
            Charger();
    }

    private void Supprimer(object sender, RoutedEventArgs e)
    {
        if (Grille.SelectedItem is not ClasseVue c)
        {
            MessageBox.Show("Sélectionnez une classe à supprimer.", "Information",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var confirmation = MessageBox.Show(
            $"Supprimer la classe « {c.NomClasse} » ?\nLes élèves de cette classe ne seront pas supprimés mais perdront leur classe.",
            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirmation != MessageBoxResult.Yes) return;

        try
        {
            ClasseService.Supprimer(c.Id);
            Charger();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

/// Classe + nombre d'élèves pour l'affichage dans la grille.
public class ClasseVue : Classe
{
    public int NombreEleves { get; set; }
}
