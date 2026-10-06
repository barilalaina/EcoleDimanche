using System.Windows;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class MoniteurDialog : Window
{
    private readonly Moniteur? _existant;

    public MoniteurDialog(Moniteur? moniteur)
    {
        InitializeComponent();
        _existant = moniteur;
        if (moniteur != null)
        {
            Title = "Modifier le moniteur";
            TxtNom.Text = moniteur.Nom;
            TxtPrenom.Text = moniteur.Prenom;
            TxtTelephone.Text = moniteur.Telephone ?? "";
            TxtEmail.Text = moniteur.Email ?? "";
        }
        else
        {
            Title = "Nouveau moniteur";
        }
    }

    private void Annuler(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Enregistrer(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNom.Text) || string.IsNullOrWhiteSpace(TxtPrenom.Text))
        {
            MessageBox.Show("Le nom et le prénom sont obligatoires.", "Champs requis",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var moniteur = new Moniteur
        {
            Id = _existant?.Id ?? 0,
            Nom = TxtNom.Text.Trim(),
            Prenom = TxtPrenom.Text.Trim(),
            Telephone = string.IsNullOrWhiteSpace(TxtTelephone.Text) ? null : TxtTelephone.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim()
        };

        try
        {
            if (_existant == null)
                MoniteurService.Ajouter(moniteur);
            else
                MoniteurService.Modifier(moniteur);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            var message = ex.Message.Contains("UNIQUE")
                ? "Cet email est déjà utilisé par un autre moniteur."
                : $"Erreur : {ex.Message}";
            MessageBox.Show(message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
