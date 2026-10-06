using System.Collections.ObjectModel;
using EcoleDimanche.Models;
using Microsoft.Data.Sqlite;

namespace EcoleDimanche.Data
{
    public static class PresenceService
    {
        /// Renvoie, pour une date donnée, l'état de présence de chaque élève
        /// (jointure gauche : les élèves sans enregistrement ont EstPresent = false).
        public static ObservableCollection<Presence> GetFeuillePresence(string dateDimanche, int? idClasse = null)
        {
            var liste = new ObservableCollection<Presence>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.id_presence, e.id_eleve, e.nom || ' ' || e.prenom, c.nom_classe, e.id_classe,
                       COALESCE(p.est_present, 0), p.remarque
                FROM eleves e
                LEFT JOIN presences p ON p.id_eleve = e.id_eleve AND p.date_dimanche = @date
                LEFT JOIN classes c ON c.id_classe = e.id_classe";
            cmd.Parameters.AddWithValue("@date", dateDimanche);
            if (idClasse.HasValue)
            {
                cmd.CommandText += " WHERE e.id_classe = @cl";
                cmd.Parameters.AddWithValue("@cl", idClasse.Value);
            }
            cmd.CommandText += " ORDER BY c.nom_classe, e.nom, e.prenom;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Presence
                {
                    Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    IdEleve = reader.GetInt32(1),
                    DateDimanche = dateDimanche,
                    EleveNom = reader.GetString(2),
                    NomClasse = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IdClasse = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    EstPresent = reader.GetInt32(5) == 1,
                    Remarque = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }
            return liste;
        }

        /// Insère ou met à jour la présence d'un élève pour une date (UPSERT).
        public static void Enregistrer(Presence p)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO presences (id_eleve, date_dimanche, est_present, remarque)
                VALUES (@eleve, @date, @present, @remarque)
                ON CONFLICT(id_eleve, date_dimanche) DO UPDATE SET
                    est_present = excluded.est_present,
                    remarque = excluded.remarque;";
            cmd.Parameters.AddWithValue("@eleve", p.IdEleve);
            cmd.Parameters.AddWithValue("@date", p.DateDimanche);
            cmd.Parameters.AddWithValue("@present", p.EstPresent ? 1 : 0);
            cmd.Parameters.AddWithValue("@remarque", (object?)p.Remarque ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public static void Supprimer(int id)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM presences WHERE id_presence = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
