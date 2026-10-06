namespace EcoleDimanche.Models
{
    public class Classe
    {
        public int Id { get; set; }
        public string NomClasse { get; set; } = string.Empty;
        public string? TrancheAge { get; set; }
        public int? IdMoniteurPrincipal { get; set; }
        public string? MoniteurPrincipal { get; set; }

        public override string ToString() => NomClasse;
    }
}
