using System.Collections.ObjectModel;
using EcoleDimanche.Models;
using Microsoft.Data.Sqlite;

namespace EcoleDimanche.Data
{
    public static class MoniteurService
    {
        public static ObservableCollection<Moniteur> GetAll(string? recherche = null)
        {
            var liste = new ObservableCollection<Moniteur>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id_moniteur, nom, prenom, telephone, email, date_inscription
                                FROM moniteurs";
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                cmd.CommandText += " WHERE nom LIKE @r OR prenom LIKE @r OR telephone LIKE @r OR email LIKE @r";
                cmd.Parameters.AddWithValue("@r", $"%{recherche}%");
            }
            cmd.CommandText += " ORDER BY nom, prenom;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(Lire(reader));
            }
            return liste;
        }

        private static Moniteur Lire(SqliteDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Nom = r.GetString(1),
            Prenom = r.GetString(2),
            Telephone = r.IsDBNull(3) ? null : r.GetString(3),
            Email = r.IsDBNull(4) ? null : r.GetString(4),
            DateInscription = r.IsDBNull(5) ? "" : r.GetString(5)
        };

        public static void Ajouter(Moniteur m)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO moniteurs (nom, prenom, telephone, email)
                                VALUES (@nom, @prenom, @tel, @email);";
            cmd.Parameters.AddWithValue("@nom", m.Nom);
            cmd.Parameters.AddWithValue("@prenom", m.Prenom);
            cmd.Parameters.AddWithValue("@tel", (object?)m.Telephone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)m.Email ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public static void Modifier(Moniteur m)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE moniteurs SET nom = @nom, prenom = @prenom,
                                telephone = @tel, email = @email WHERE id_moniteur = @id;";
            cmd.Parameters.AddWithValue("@nom", m.Nom);
            cmd.Parameters.AddWithValue("@prenom", m.Prenom);
            cmd.Parameters.AddWithValue("@tel", (object?)m.Telephone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)m.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.ExecuteNonQuery();
        }

        public static void Supprimer(int id)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM moniteurs WHERE id_moniteur = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
