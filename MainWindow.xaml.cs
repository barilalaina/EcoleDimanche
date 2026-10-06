using System.Windows;
using EcoleDimanche.Data;
using EcoleDimanche.Views;

namespace EcoleDimanche;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ChargerStatistiques();
    }

    private void Fenetre_Activated(object? sender, EventArgs e)
    {
        // Rafraîchit le tableau de bord au retour d'une fenêtre de gestion
        ChargerStatistiques();
    }

    private void ChargerStatistiques()
    {
        TxtDate.Text = DateTime.Now.ToString("dddd d MMMM yyyy",
            new System.Globalization.CultureInfo("fr-FR"));

        TxtNbEleves.Text = StatsService.NombreEleves().ToString();
        TxtGenres.Text = $"{StatsService.NombreGarcons()} garçon(s) · {StatsService.NombreFilles()} fille(s)";

        TxtNbClasses.Text = StatsService.NombreClasses().ToString();
        TxtBaptises.Text = $"{StatsService.NombreBaptises()} élève(s) baptisé(s)";

        TxtNbMoniteurs.Text = StatsService.NombreMoniteurs().ToString();

        var dernierDimanche = StatsService.DernierDimanche();
        TxtNbPresents.Text = StatsService.PresentsPourDate(dernierDimanche).ToString();
        TxtDateDimanche.Text = DateTime.Parse(dernierDimanche).ToString("dd/MM/yyyy");

        GrilleClasses.ItemsSource = StatsService.ElevesParClasse();
        GrilleInscriptions.ItemsSource = StatsService.DernieresInscriptions();
    }

    private void OuvrirEleves(object sender, RoutedEventArgs e) =>
        new ElevesWindow { Owner = this }.Show();

    private void OuvrirClasses(object sender, RoutedEventArgs e) =>
        new ClassesWindow { Owner = this }.Show();

    private void OuvrirMoniteurs(object sender, RoutedEventArgs e) =>
        new MoniteursWindow { Owner = this }.Show();

    private void OuvrirPresences(object sender, RoutedEventArgs e) =>
        new PresencesWindow { Owner = this }.Show();
}
