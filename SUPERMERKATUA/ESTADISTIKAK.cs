using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUPERMERKATUA
{
    public class ESTADISTIKAK
    {
        // 1. GEHIEN SALDU DEN PRODUKTUA (Kantitatearen arabera)
        public static void GehienSalduDenProduktua(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // JOIN egiten dugu izena lortzeko
                    string query = @"
                        SELECT p.izena, SUM(t.kantitatea) as Guztira 
                        FROM ticketa t 
                        JOIN produktua p ON t.id_produktua = p.id_produktua 
                        GROUP BY p.izena 
                        ORDER BY Guztira DESC 
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Console.WriteLine($"\n★ GEHIEN SALDU DEN PRODUKTUA: {rdr["izena"]} ({rdr["Guztira"]} unitate/kg)");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }

        // 2. GUTXIEN SALDU DEN PRODUKTUA
        public static void GutxienSalduDenProduktua(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT p.izena, SUM(t.kantitatea) as Guztira 
                        FROM ticketa t 
                        JOIN produktua p ON t.id_produktua = p.id_produktua 
                        GROUP BY p.izena 
                        ORDER BY Guztira ASC 
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Console.WriteLine($"\n☹ GUTXIEN SALDU DEN PRODUKTUA: {rdr["izena"]} ({rdr["Guztira"]} unitate/kg)");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }

        // 3. GEHIEN SALDU DUEN SALTZAILEA (Diruaren arabera)
        public static void GehienSalduDuenSaltzailea(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Hemen id_saltzailea erabiltzen dugu. Izenak badituzu beste taula batean, JOIN egin.
                    string query = @"
                        SELECT id_saltzailea, SUM(totala) as Dirua 
                        FROM ticketa 
                        GROUP BY id_saltzailea 
                        ORDER BY Dirua DESC 
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Console.WriteLine($"\n★ SALTZAILE ONENA (ID): {rdr["id_saltzailea"]} - Irabaziak: {rdr["Dirua"]}€");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }

        // 4. GUTXIEN SALDU DUEN SALTZAILEA
        public static void GutxienSalduDuenSaltzailea(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT id_saltzailea, SUM(totala) as Dirua 
                        FROM ticketa 
                        GROUP BY id_saltzailea 
                        ORDER BY Dirua ASC 
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Console.WriteLine($"\n☹ SALTZAILE TXARRENA (ID): {rdr["id_saltzailea"]} - Irabaziak: {rdr["Dirua"]}€");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }

        // 5. GEHIEN SALDU DEN EGUNA
        public static void GehienSalduDenEguna(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Egunaren arabera taldekatzen dugu (ordua kenduta DATE erabiliz)
                    string query = @"
                        SELECT DATE(data) as Eguna, SUM(totala) as Dirua 
                        FROM ticketa 
                        GROUP BY DATE(data) 
                        ORDER BY Dirua DESC 
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            // Data formatua garbitu
                            DateTime eguna = Convert.ToDateTime(rdr["Eguna"]);
                            Console.WriteLine($"\n📅 EGUN ONENA: {eguna.ToShortDateString()} - Salmentak: {rdr["Dirua"]}€");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }

        // 6. SALTZAILE BAKOITZAK ZENBAT SALDU DUEN (LISTA)
        public static void SaldutakoaSaltzaileko(string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_saltzailea, SUM(totala) as Dirua FROM ticketa GROUP BY id_saltzailea";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- SALTZAILE BAKOITZA ---");
                        while (rdr.Read())
                        {
                            Console.WriteLine($"Saltzailea {rdr["id_saltzailea"]}: {rdr["Dirua"]}€");
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Errorea: " + ex.Message); }
            }
        }
    }
}

