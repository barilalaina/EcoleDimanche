using System.Collections.ObjectModel;
using EcoleDimanche.Models;
using Microsoft.Data.Sqlite;

namespace EcoleDimanche.Data
{
    public static class EleveService
    {
        private const string SelectBase = @"
            SELECT e.id_eleve, e.nom, e.prenom, e.genre, e.adresse, e.date_naissance, e.lieu_naissance,
                   e.nom_pere, e.telephone_pere, e.fb_pere, e.nom_mere, e.telephone_mere, e.fb_mere,
                   e.vivre_parent, e.nombre_frere, e.nombre_soeur, e.baptise, e.id_classe,
                   e.date_inscription, c.nom_classe
            FROM eleves e
            LEFT JOIN classes c ON c.id_classe = e.id_classe";

        public static ObservableCollection<Eleve> GetAll(string? recherche = null, int? idClasse = null)
        {
            var liste = new ObservableCollection<Eleve>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = SelectBase;

            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                cmd.CommandText += " WHERE (e.nom LIKE @r OR e.prenom LIKE @r OR e.nom_pere LIKE @r OR e.nom_mere LIKE @r)";
                cmd.Parameters.AddWithValue("@r", $"%{recherche}%");
                if (idClasse.HasValue)
                {
                    cmd.CommandText += " AND e.id_classe = @cl";
                    cmd.Parameters.AddWithValue("@cl", idClasse.Value);
                }
            }
            else if (idClasse.HasValue)
            {
                cmd.CommandText += " WHERE e.id_classe = @cl";
                cmd.Parameters.AddWithValue("@cl", idClasse.Value);
            }
            cmd.CommandText += " ORDER BY e.nom, e.prenom;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(Lire(reader));
            }
            return liste;
        }

        private static Eleve Lire(SqliteDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Nom = r.GetString(1),
            Prenom = r.GetString(2),
            Genre = r.GetString(3),
            Adresse = r.GetString(4),
            DateNaissance = r.GetString(5),
            LieuNaissance = r.GetString(6),
            NomPere = r.GetString(7),
            TelephonePere = r.GetString(8),
            FbPere = r.GetString(9),
            NomMere = r.GetString(10),
            TelephoneMere = r.GetString(11),
            FbMere = r.GetString(12),
            VitAvecParents = !r.IsDBNull(13) && r.GetInt32(13) == 1,
            NombreFreres = r.IsDBNull(14) ? 0 : r.GetInt32(14),
            NombreSoeurs = r.IsDBNull(15) ? 0 : r.GetInt32(15),
            Baptise = !r.IsDBNull(16) && r.GetInt32(16) == 1,
            IdClasse = r.IsDBNull(17) ? null : r.GetInt32(17),
            DateInscription = r.IsDBNull(18) ? "" : r.GetString(18),
            NomClasse = r.IsDBNull(19) ? null : r.GetString(19)
        };

        private static void AjouterParametres(SqliteCommand cmd, Eleve e)
        {
            cmd.Parameters.AddWithValue("@nom", e.Nom);
            cmd.Parameters.AddWithValue("@prenom", e.Prenom);
            cmd.Parameters.AddWithValue("@genre", e.Genre);
            cmd.Parameters.AddWithValue("@adresse", e.Adresse);
            cmd.Parameters.AddWithValue("@dn", e.DateNaissance);
            cmd.Parameters.AddWithValue("@ln", e.LieuNaissance);
            cmd.Parameters.AddWithValue("@npere", e.NomPere);
            cmd.Parameters.AddWithValue("@tpere", e.TelephonePere);
            cmd.Parameters.AddWithValue("@fbpere", e.FbPere);
            cmd.Parameters.AddWithValue("@nmere", e.NomMere);
            cmd.Parameters.AddWithValue("@tmere", e.TelephoneMere);
            cmd.Parameters.AddWithValue("@fbmere", e.FbMere);
            cmd.Parameters.AddWithValue("@vp", e.VitAvecParents ? 1 : 0);
            cmd.Parameters.AddWithValue("@nf", e.NombreFreres);
            cmd.Parameters.AddWithValue("@ns", e.NombreSoeurs);
            cmd.Parameters.AddWithValue("@bap", e.Baptise ? 1 : 0);
            cmd.Parameters.AddWithValue("@classe", (object?)e.IdClasse ?? DBNull.Value);
        }

        public static void Ajouter(Eleve e)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO eleves (nom, prenom, genre, adresse, date_naissance, lieu_naissance,
                                    nom_pere, telephone_pere, fb_pere, nom_mere, telephone_mere, fb_mere,
                                    vivre_parent, nombre_frere, nombre_soeur, baptise, id_classe)
                                VALUES (@nom, @prenom, @genre, @adresse, @dn, @ln,
                                    @npere, @tpere, @fbpere, @nmere, @tmere, @fbmere,
                                    @vp, @nf, @ns, @bap, @classe);";
            AjouterParametres(cmd, e);
            cmd.ExecuteNonQuery();
        }

        public static void Modifier(Eleve e)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE eleves SET nom = @nom, prenom = @prenom, genre = @genre,
                                    adresse = @adresse, date_naissance = @dn, lieu_naissance = @ln,
                                    nom_pere = @npere, telephone_pere = @tpere, fb_pere = @fbpere,
                                    nom_mere = @nmere, telephone_mere = @tmere, fb_mere = @fbmere,
                                    vivre_parent = @vp, nombre_frere = @nf, nombre_soeur = @ns,
                                    baptise = @bap, id_classe = @classe
                                WHERE id_eleve = @id;";
            AjouterParametres(cmd, e);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void Supprimer(int id)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM eleves WHERE id_eleve = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
