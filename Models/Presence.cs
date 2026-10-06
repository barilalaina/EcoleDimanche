namespace EcoleDimanche.Models
{
    public class Presence
    {
        public int Id { get; set; }
        public int IdEleve { get; set; }
        public string DateDimanche { get; set; } = string.Empty;
        public bool EstPresent { get; set; }
        public string? Remarque { get; set; }

        // Champs d'affichage (jointure)
        public string? EleveNom { get; set; }
        public string? NomClasse { get; set; }
        public int? IdClasse { get; set; }
    }
}
