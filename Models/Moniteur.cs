namespace EcoleDimanche.Models
{
    public class Moniteur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string DateInscription { get; set; } = string.Empty;

        public string NomComplet => $"{Prenom} {Nom}";
    }
}
