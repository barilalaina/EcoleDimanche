using System.IO;
using Microsoft.Data.Sqlite;

namespace EcoleDimanche.Data
{
    public static class Database
    {
        public static string DbPath { get; } =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ecoledimanche.db");

        public static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection($"Data Source={DbPath}");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA foreign_keys = ON;";
            cmd.ExecuteNonQuery();
            return conn;
        }

        public static void Initialize()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS moniteurs (
    id_moniteur INTEGER PRIMARY KEY AUTOINCREMENT,
    nom TEXT NOT NULL,
    prenom TEXT NOT NULL,
    telephone TEXT,
    email TEXT UNIQUE,
    date_inscription TEXT DEFAULT CURRENT_DATE
);

CREATE TABLE IF NOT EXISTS classes (
    id_classe INTEGER PRIMARY KEY AUTOINCREMENT,
    nom_classe TEXT NOT NULL,
    tranche_age TEXT,
    id_moniteur_principal INTEGER,
    FOREIGN KEY (id_moniteur_principal) REFERENCES moniteurs(id_moniteur) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS eleves (
    id_eleve INTEGER PRIMARY KEY AUTOINCREMENT,
    nom TEXT NOT NULL,
    prenom TEXT NOT NULL,
    genre TEXT NOT NULL,
    adresse TEXT NOT NULL,
    date_naissance TEXT NOT NULL,
    lieu_naissance TEXT NOT NULL,
    nom_pere TEXT NOT NULL,
    telephone_pere TEXT NOT NULL,
    fb_pere TEXT NOT NULL,
    nom_mere TEXT NOT NULL,
    telephone_mere TEXT NOT NULL,
    fb_mere TEXT NOT NULL,
    vivre_parent INTEGER,
    nombre_frere INTEGER,
    nombre_soeur INTEGER,
    baptise INTEGER,
    id_classe INTEGER,
    date_inscription TEXT DEFAULT CURRENT_DATE,
    FOREIGN KEY (id_classe) REFERENCES classes(id_classe) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS presences (
    id_presence INTEGER PRIMARY KEY AUTOINCREMENT,
    id_eleve INTEGER NOT NULL,
    date_dimanche TEXT NOT NULL,
    est_present INTEGER DEFAULT 0,
    remarque TEXT,
    FOREIGN KEY (id_eleve) REFERENCES eleves(id_eleve) ON DELETE CASCADE,
    UNIQUE (id_eleve, date_dimanche)
);";
            cmd.ExecuteNonQuery();
        }
    }
}
