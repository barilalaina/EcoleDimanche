using System.Windows;
using EcoleDimanche.Data;

namespace EcoleDimanche;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            Database.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'initialisation de la base de données :\n{ex.Message}",
                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
