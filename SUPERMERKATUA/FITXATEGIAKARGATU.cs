using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using MySql.Data.MySqlClient;

namespace SUPERMERKATUA
{
    public class FitxategiaKargatu
    {
        // Karpeta nagusia
        static string rootPath = @"C:\TicketBAI";
        static string backupPath = Path.Combine(rootPath, "BACKUP_HISTORIAL");

        public static void DatuakInportatu(string connectionString)
        {
            // Karpetak sortu
            SortuKarpetak();

            List<TIKETA> tiketaGuztiak = new List<TIKETA>();
            List<string> prozesatutakoFitxategiak = new List<string>();
            Random rnd = new Random(); // Saltzaileak ausaz aukeratzeko

            // ---------------------------------------------------------
            // 0. URRATSA: PRESTAKETA - PRODUKTUEN ID-AK KARGATU DATU BASETIK
            // ---------------------------------------------------------
            Console.WriteLine("0. Produktuen katalogoa Datu Basetik kargatzen...");
            Dictionary<string, int> dbProduktuMapaketa = new Dictionary<string, int>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT izena, id_produktua FROM produktua";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Izena minuskulaz gorde konparaketak errazteko
                            string dbIzena = rdr["izena"].ToString().ToLower().Trim();
                            int dbId = Convert.ToInt32(rdr["id_produktua"]);

                            if (!dbProduktuMapaketa.ContainsKey(dbIzena))
                            {
                                dbProduktuMapaketa.Add(dbIzena, dbId);
                            }
                        }
                    }
                    Console.WriteLine($"   -> {dbProduktuMapaketa.Count} produktu kargatu dira memoriara.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROREA KONEXIOAN (Produktuak kargatzean): " + ex.Message);
                    return; // Ezin badugu DB irakurri, hobe gelditzea
                }
            }

            // ---------------------------------------------------------
            // 1. FITXATEGIAK BILATU ETA IRAKURRI
            // ---------------------------------------------------------
            Console.WriteLine("1. Fitxategiak bilatzen eta prozesatzen...");

            try
            {
                // Karpeta guztietan bilatu (SearchOption.AllDirectories)
                string[] fitxategiGuztiak = Directory.GetFiles(rootPath, "*.txt", SearchOption.AllDirectories);

                Console.WriteLine($"   -> {fitxategiGuztiak.Length} fitxategi aurkitu dira.");

                foreach (string fitxategia in fitxategiGuztiak)
                {
                    // "Backup" karpetan daudenak EZ ukitu
                    if (fitxategia.Contains("BACKUP_HISTORIAL")) continue;

                    try
                    {
                        // IRAKURRI FITXATEGIA LERROZ LERRO
                        string[] lerroak = File.ReadAllLines(fitxategia);
                        bool fitxategiaOndo = false;

                        foreach (string lerroa in lerroak)
                        {
                            // FORMATUA: limoiak$001$20,3$1,93$39,18
                            string[] zatiak = lerroa.Split('$');

                            // Balidazio minimoa (gutxienez 5 zati)
                            if (zatiak.Length < 5) continue;

                            TIKETA t = new TIKETA();
                            t.Data = DateTime.Now; // Gaurko data

                            // A) PRODUKTUA (Izena -> ID bihurtu)
                            string izenaFitxategian = zatiak[0].ToLower().Trim();

                            if (dbProduktuMapaketa.ContainsKey(izenaFitxategian))
                            {
                                t.IdProduktua = dbProduktuMapaketa[izenaFitxategian];
                            }
                            else
                            {
                                // Izen exactoa ez badago, saiatu "contains"
                                int idAurkitua = 1; // Default Binbo
                                foreach (var item in dbProduktuMapaketa)
                                {
                                    if (izenaFitxategian.Contains(item.Key))
                                    {
                                        idAurkitua = item.Value;
                                        break;
                                    }
                                }
                                t.IdProduktua = idAurkitua;
                            }

                            // B) SALTZAILEA (Ausaz 1, 2 edo 3)
                            t.IdSaltzailea = rnd.Next(1, 4);

                            // C) ZENBAKIAK (Prezioa, Kantitatea, Totala)
                            // Koma edo puntua onartzeko logika gehi dezakegu behar izanez gero
                            t.PrezioaKg = decimal.Parse(zatiak[2]);
                            t.Kantitatea = decimal.Parse(zatiak[3]);
                            t.Totala = decimal.Parse(zatiak[4]);

                            // Zerrendara gehitu
                            tiketaGuztiak.Add(t);
                            fitxategiaOndo = true;
                        }

                        if (fitxategiaOndo)
                        {
                            prozesatutakoFitxategiak.Add(fitxategia);
                            Console.WriteLine($"      Kargatuta: {Path.GetFileName(fitxategia)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errorea fitxategia irakurtzean ({Path.GetFileName(fitxategia)}): {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errorea fitxategiak bilatzean: " + ex.Message);
            }

            if (tiketaGuztiak.Count == 0)
            {
                Console.WriteLine("Ez da fitxategirik prozesatu edo zerrenda hutsik dago.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"--> {tiketaGuztiak.Count} lerro (ticket) memorian kargatuta.");

            // ---------------------------------------------------------
            // 3. XML SORTU
            // ---------------------------------------------------------
            Console.WriteLine("3. XML fitxategia sortzen...");
            string xmlIzena = $"TicketBAI_Bidalketa_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            string xmlPath = Path.Combine(rootPath, xmlIzena);

            try
            {
                SortuXSD(); // XSD sortu balidaziorako

                XmlSerializer serializer = new XmlSerializer(typeof(List<TIKETA>));
                using (FileStream fs = new FileStream(xmlPath, FileMode.Create))
                {
                    serializer.Serialize(fs, tiketaGuztiak);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROREA XML sortzean: " + ex.Message);
                return;
            }

            // ---------------------------------------------------------
            // 4. BALIDATU XML XSD ERABILIZ
            // ---------------------------------------------------------
            Console.WriteLine("4. XML Balidatzen...");
            if (!BalidatuXML(xmlPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROREA: XMLak ez du balidazioa pasatu. Prozesua gelditzen.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }
            Console.WriteLine("   -> XML ZUZENA DA.");

            // ---------------------------------------------------------
            // 5. EMAILA BIDALI
            // ---------------------------------------------------------
            Console.WriteLine("5. Emaila bidaltzen...");
            KudeatuEmail.Bidali(xmlPath);

            // ---------------------------------------------------------
            // 6. EXCEL ERREGISTROA
            // ---------------------------------------------------------
            Console.WriteLine("6. Excel-en erregistratzen...");
            try
            {
                string csvPath = Path.Combine(rootPath, "Bidalketa_Erregistroa.csv");
                string csvLerroa = $"{Path.GetFileName(xmlPath)};{DateTime.Now}";

                if (!File.Exists(csvPath))
                    File.AppendAllText(csvPath, "XML_Izena;Data_Ordua" + Environment.NewLine);

                File.AppendAllText(csvPath, csvLerroa + Environment.NewLine);
                Console.WriteLine("   -> Erregistroa eginda.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errorea Excel idaztean: " + ex.Message);
            }

            // ---------------------------------------------------------
            // 7. INSERT DATU BASEAN
            // ---------------------------------------------------------
            Console.WriteLine("7. Datu basean gordetzen...");
            bool dbOndo = false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO ticketa (data, kantitatea, prezioakg, totala, id_saltzailea, id_produktua) VALUES (@data, @kantitatea, @prezioakg, @totala, @idsaltzailea, @idproduktua)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        foreach (TIKETA t in tiketaGuztiak)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@data", t.Data);
                            cmd.Parameters.AddWithValue("@kantitatea", t.Kantitatea);
                            cmd.Parameters.AddWithValue("@prezioakg", t.PrezioaKg);
                            cmd.Parameters.AddWithValue("@totala", t.Totala);

                            // IDak jada zuzenak dira (mapaketan konpondu dira), 
                            // baina badaezpada 1 jartzen dugu 0 bada.
                            int sID = (t.IdSaltzailea < 1 || t.IdSaltzailea > 3) ? 1 : t.IdSaltzailea;
                            int pID = (t.IdProduktua < 1) ? 1 : t.IdProduktua;

                            cmd.Parameters.AddWithValue("@idsaltzailea", sID);
                            cmd.Parameters.AddWithValue("@idproduktua", pID);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   -> Datuak ondo gorde dira!");
                    Console.ResetColor();
                    dbOndo = true;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROREA DATU BASEAN: " + ex.Message);
                    dbOndo = false;
                }
            }

            // ---------------------------------------------------------
            // 8. MUGITU BACKUP KARPETARA
            // ---------------------------------------------------------
            if (dbOndo)
            {
                Console.WriteLine("Fitxategiak Backup karpetara mugitzen...");
                foreach (string f in prozesatutakoFitxategiak)
                {
                    try
                    {
                        string izena = Path.GetFileName(f);
                        string destinoa = Path.Combine(backupPath, izena);

                        // Izen berdina badago, gainidatzi edo izena aldatu (hemen gainidatzi)
                        if (File.Exists(destinoa)) File.Delete(destinoa);

                        File.Move(f, destinoa);
                    }
                    catch { /* Ignoratu mugitze erroreak */ }
                }
            }
            else
            {
                Console.WriteLine("Fitxategiak EZ dira mugituko erroreak egon direlako.");
            }

            Console.WriteLine("\nPROZESUA AMAITUTA. Sakatu tekla bat.");
            Console.ReadKey();
        }

       

        private static void SortuKarpetak()
        {
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);
        }

        private static void SortuXSD()
        {
            XmlSchemas schemas = new XmlSchemas();
            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);
            XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(List<TIKETA>));
            exporter.ExportTypeMapping(mapping);

            using (StreamWriter sw = new StreamWriter(Path.Combine(rootPath, "eskema.xsd")))
            {
                foreach (XmlSchema schema in schemas)
                {
                    schema.Write(sw);
                }
            }
        }

        private static bool BalidatuXML(string xmlPath)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, Path.Combine(rootPath, "eskema.xsd"));
                settings.ValidationType = ValidationType.Schema;

                using (XmlReader reader = XmlReader.Create(xmlPath, settings))
                {
                    while (reader.Read()) { }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("XSD ERROREA: " + ex.Message);
                return false;
            }
        }
    }
}