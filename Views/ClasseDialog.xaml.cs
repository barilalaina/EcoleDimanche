using System.Windows;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class ClasseDialog : Window
{
    private readonly Classe? _existante;
    private readonly List<Moniteur> _moniteurs;

    public ClasseDialog(Classe? classe)
    {
        InitializeComponent();
        _existante = classe;

        _moniteurs = MoniteurService.GetAll().ToList();
        CboMoniteur.ItemsSource = _moniteurs;

        if (classe != null)
        {
            Title = "Modifier la classe";
            TxtNomClasse.Text = classe.NomClasse;
            TxtTrancheAge.Text = classe.TrancheAge ?? "";
            if (classe.IdMoniteurPrincipal.HasValue)
                CboMoniteur.SelectedItem = _moniteurs
                    .FirstOrDefault(m => m.Id == classe.IdMoniteurPrincipal.Value);
        }
        else
        {
            Title = "Nouvelle classe";
        }
    }

    private void Annuler(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Enregistrer(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNomClasse.Text))
        {
            MessageBox.Show("Le nom de la classe est obligatoire.", "Champ requis",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var classe = new Classe
        {
            Id = _existante?.Id ?? 0,
            NomClasse = TxtNomClasse.Text.Trim(),
            TrancheAge = string.IsNullOrWhiteSpace(TxtTrancheAge.Text) ? null : TxtTrancheAge.Text.Trim(),
            IdMoniteurPrincipal = (CboMoniteur.SelectedItem as Moniteur)?.Id
        };

        try
        {
            if (_existante == null)
                ClasseService.Ajouter(classe);
            else
                ClasseService.Modifier(classe);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
