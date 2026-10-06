using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using EcoleDimanche.Data;
using EcoleDimanche.Models;

namespace EcoleDimanche.Views;

public partial class EleveDialog : Window
{
    private readonly Eleve? _existant;
    private readonly List<Classe> _classes;

    public EleveDialog(Eleve? eleve, List<Classe> classes)
    {
        InitializeComponent();
        _existant = eleve;

        _classes = new List<Classe> { new() { Id = 0, NomClasse = "— Aucune classe —" } };
        _classes.AddRange(classes);
        CboClasse.ItemsSource = _classes;
        CboClasse.SelectedIndex = 0;
        CboGenre.SelectedIndex = 0;

        if (eleve != null)
        {
            Title = "Modifier l'élève";
            TxtNom.Text = eleve.Nom;
            TxtPrenom.Text = eleve.Prenom;
            CboGenre.SelectedIndex = eleve.Genre.StartsWith("F", true, CultureInfo.InvariantCulture) ? 1 : 0;
            TxtAdresse.Text = eleve.Adresse;
            if (DateTime.TryParse(eleve.DateNaissance, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var naissance))
                DtpNaissance.SelectedDate = naissance;
            TxtLieuNaissance.Text = eleve.LieuNaissance;
            TxtNomPere.Text = eleve.NomPere;
            TxtTelPere.Text = eleve.TelephonePere;
            TxtFbPere.Text = eleve.FbPere;
            TxtNomMere.Text = eleve.NomMere;
            TxtTelMere.Text = eleve.TelephoneMere;
            TxtFbMere.Text = eleve.FbMere;
            ChkVitParents.IsChecked = eleve.VitAvecParents;
            TxtNbFreres.Text = eleve.NombreFreres.ToString();
            TxtNbSoeurs.Text = eleve.NombreSoeurs.ToString();
            ChkBaptise.IsChecked = eleve.Baptise;
            if (eleve.IdClasse.HasValue)
                CboClasse.SelectedItem = _classes.FirstOrDefault(c => c.Id == eleve.IdClasse.Value);
        }
        else
        {
            Title = "Nouvel élève";
        }
    }

    private void Annuler(object sender, RoutedEventArgs e) => DialogResult = false;

    private static bool EstVide(params TextBox[] champs) =>
        champs.Any(c => string.IsNullOrWhiteSpace(c.Text));

    private void Enregistrer(object sender, RoutedEventArgs e)
    {
        if (EstVide(TxtNom, TxtPrenom, TxtAdresse, TxtLieuNaissance,
                    TxtNomPere, TxtTelPere, TxtFbPere, TxtNomMere, TxtTelMere, TxtFbMere))
        {
            MessageBox.Show("Veuillez remplir tous les champs obligatoires (*).", "Champs requis",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (DtpNaissance.SelectedDate == null)
        {
            MessageBox.Show("La date de naissance est obligatoire.", "Champ requis",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(TxtNbFreres.Text, out var freres) || freres < 0 ||
            !int.TryParse(TxtNbSoeurs.Text, out var soeurs) || soeurs < 0)
        {
            MessageBox.Show("Le nombre de frères et de sœurs doit être un entier positif.", "Valeur invalide",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var genre = (CboGenre.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Masculin";
        var classe = CboClasse.SelectedItem as Classe;

        var eleve = new Eleve
        {
            Id = _existant?.Id ?? 0,
            Nom = TxtNom.Text.Trim(),
            Prenom = TxtPrenom.Text.Trim(),
            Genre = genre,
            Adresse = TxtAdresse.Text.Trim(),
            DateNaissance = DtpNaissance.SelectedDate!.Value.ToString("yyyy-MM-dd"),
            LieuNaissance = TxtLieuNaissance.Text.Trim(),
            NomPere = TxtNomPere.Text.Trim(),
            TelephonePere = TxtTelPere.Text.Trim(),
            FbPere = TxtFbPere.Text.Trim(),
            NomMere = TxtNomMere.Text.Trim(),
            TelephoneMere = TxtTelMere.Text.Trim(),
            FbMere = TxtFbMere.Text.Trim(),
            VitAvecParents = ChkVitParents.IsChecked == true,
            NombreFreres = freres,
            NombreSoeurs = soeurs,
            Baptise = ChkBaptise.IsChecked == true,
            IdClasse = classe != null && classe.Id != 0 ? classe.Id : null
        };

        try
        {
            if (_existant == null)
                EleveService.Ajouter(eleve);
            else
                EleveService.Modifier(eleve);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
