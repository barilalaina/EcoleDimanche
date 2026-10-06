using System.Collections.ObjectModel;
using EcoleDimanche.Models;
using Microsoft.Data.Sqlite;

namespace EcoleDimanche.Data
{
    public static class ClasseService
    {
        public static ObservableCollection<Classe> GetAll(string? recherche = null)
        {
            var liste = new ObservableCollection<Classe>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT c.id_classe, c.nom_classe, c.tranche_age, c.id_moniteur_principal,
                                       m.nom || ' ' || m.prenom
                                FROM classes c
                                LEFT JOIN moniteurs m ON m.id_moniteur = c.id_moniteur_principal";
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                cmd.CommandText += " WHERE c.nom_classe LIKE @r OR c.tranche_age LIKE @r";
                cmd.Parameters.AddWithValue("@r", $"%{recherche}%");
            }
            cmd.CommandText += " ORDER BY c.nom_classe;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(Lire(reader));
            }
            return liste;
        }

        private static Classe Lire(SqliteDataReader r) => new()
        {
            Id = r.GetInt32(0),
            NomClasse = r.GetString(1),
            TrancheAge = r.IsDBNull(2) ? null : r.GetString(2),
            IdMoniteurPrincipal = r.IsDBNull(3) ? null : r.GetInt32(3),
            MoniteurPrincipal = r.IsDBNull(4) ? null : r.GetString(4)
        };

        public static void Ajouter(Classe c)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO classes (nom_classe, tranche_age, id_moniteur_principal)
                                VALUES (@nom, @age, @mon);";
            cmd.Parameters.AddWithValue("@nom", c.NomClasse);
            cmd.Parameters.AddWithValue("@age", (object?)c.TrancheAge ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mon", (object?)c.IdMoniteurPrincipal ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public static void Modifier(Classe c)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE classes SET nom_classe = @nom, tranche_age = @age,
                                id_moniteur_principal = @mon WHERE id_classe = @id;";
            cmd.Parameters.AddWithValue("@nom", c.NomClasse);
            cmd.Parameters.AddWithValue("@age", (object?)c.TrancheAge ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mon", (object?)c.IdMoniteurPrincipal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.ExecuteNonQuery();
        }

        public static void Supprimer(int id)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM classes WHERE id_classe = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static int NombreEleves(int idClasse)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM eleves WHERE id_classe = @id;";
            cmd.Parameters.AddWithValue("@id", idClasse);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
