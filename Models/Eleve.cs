namespace EcoleDimanche.Models
{
    public class Eleve
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string DateNaissance { get; set; } = string.Empty;
        public string LieuNaissance { get; set; } = string.Empty;
        public string NomPere { get; set; } = string.Empty;
        public string TelephonePere { get; set; } = string.Empty;
        public string FbPere { get; set; } = string.Empty;
        public string NomMere { get; set; } = string.Empty;
        public string TelephoneMere { get; set; } = string.Empty;
        public string FbMere { get; set; } = string.Empty;
        public bool VitAvecParents { get; set; }
        public int NombreFreres { get; set; }
        public int NombreSoeurs { get; set; }
        public bool Baptise { get; set; }
        public int? IdClasse { get; set; }
        public string DateInscription { get; set; } = string.Empty;
        public string? NomClasse { get; set; }

        public string NomComplet => $"{Prenom} {Nom}";
        public string BaptiseText => Baptise ? "Oui" : "Non";
    }
}
