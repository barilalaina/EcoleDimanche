namespace EcoleDimanche.Data
{
    public record ClasseStat(string NomClasse, int NombreEleves, string? Moniteur);
    public record DerniereInscription(string Nom, string Prenom, string? Classe, string DateInscription);

    public static class StatsService
    {
        private static T Scalaire<T>(string sql, params (string, object)[] parametres)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            foreach (var (nom, valeur) in parametres)
                cmd.Parameters.AddWithValue(nom, valeur);
            var resultat = cmd.ExecuteScalar();
            if (resultat == null || resultat == DBNull.Value) return default!;
            return (T)Convert.ChangeType(resultat, typeof(T));
        }

        public static int NombreEleves() =>
            Scalaire<int>("SELECT COUNT(*) FROM eleves;");

        public static int NombreMoniteurs() =>
            Scalaire<int>("SELECT COUNT(*) FROM moniteurs;");

        public static int NombreClasses() =>
            Scalaire<int>("SELECT COUNT(*) FROM classes;");

        public static int NombreGarcons() =>
            Scalaire<int>("SELECT COUNT(*) FROM eleves WHERE UPPER(genre) LIKE 'M%' OR UPPER(genre) = 'GARCON';");

        public static int NombreFilles() =>
            Scalaire<int>("SELECT COUNT(*) FROM eleves WHERE UPPER(genre) LIKE 'F%';");

        public static int NombreBaptises() =>
            Scalaire<int>("SELECT COUNT(*) FROM eleves WHERE baptise = 1;");

        /// Date du dernier dimanche passé (ou aujourd'hui si dimanche).
        public static string DernierDimanche()
        {
            var date = DateTime.Today;
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(-1);
            return date.ToString("yyyy-MM-dd");
        }

        public static int PresentsPourDate(string dateDimanche) =>
            Scalaire<int>("SELECT COUNT(*) FROM presences WHERE date_dimanche = @d AND est_present = 1;",
                ("@d", dateDimanche));

        public static List<ClasseStat> ElevesParClasse()
        {
            var liste = new List<ClasseStat>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.nom_classe, COUNT(e.id_eleve), m.nom || ' ' || m.prenom
                FROM classes c
                LEFT JOIN eleves e ON e.id_classe = c.id_classe
                LEFT JOIN moniteurs m ON m.id_moniteur = c.id_moniteur_principal
                GROUP BY c.id_classe
                ORDER BY COUNT(e.id_eleve) DESC;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new ClasseStat(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2)));
            }
            return liste;
        }

        public static List<DerniereInscription> DernieresInscriptions(int limite = 5)
        {
            var liste = new List<DerniereInscription>();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT e.nom, e.prenom, c.nom_classe, e.date_inscription
                FROM eleves e
                LEFT JOIN classes c ON c.id_classe = e.id_classe
                ORDER BY e.date_inscription DESC, e.id_eleve DESC
                LIMIT @limite;";
            cmd.Parameters.AddWithValue("@limite", limite);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new DerniereInscription(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.IsDBNull(3) ? "" : reader.GetString(3)));
            }
            return liste;
        }
    }
}
