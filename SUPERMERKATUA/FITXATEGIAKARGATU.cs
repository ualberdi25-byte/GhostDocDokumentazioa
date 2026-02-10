using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using MySql.Data.MySqlClient;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Aplikazioaren hasierako datuak kanpoko fitxategietatik (txt, csv) kargatzeko ardura duen klasea.
    /// </summary>
    public class FitxategiaKargatu
    {
        // Karpeta nagusia
        static string rootPath = @"C:\TicketBAI";
        static string backupPath = Path.Combine(rootPath, "BACKUP_HISTORIAL");

        /// <summary>
        /// Fitxategiak irakurri, balidatu, datu basean sartu eta backup-a egiten du.
        /// </summary>
        /// <param name="connectionString">Datu basera konektatzeko katea.</param>
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
                            string prodIzena = rdr["izena"].ToString().ToLower().Trim();
                            int prodId = Convert.ToInt32(rdr["id_produktua"]);
                            if (!dbProduktuMapaketa.ContainsKey(prodIzena))
                            {
                                dbProduktuMapaketa.Add(prodIzena, prodId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROREA Datu Basearekin: " + ex.Message);
                    return;
                }
            }

            // ---------------------------------------------------------
            // 1. URRATSA: XML FITXATEGIAK IRAKURRI
            // ---------------------------------------------------------
            Console.WriteLine("\n1. XML fitxategiak bilatzen...");
            string[] xmlFiles = Directory.GetFiles(rootPath, "*.xml");

            // Lehenengo XSD eskema sortu balidaziorako (Behin bakarrik)
            SortuXSD();

            foreach (string file in xmlFiles)
            {
                if (file.EndsWith("eskema.xsd")) continue; // Eskema saltatu

                Console.WriteLine($"   -> Prozesatzen: {Path.GetFileName(file)}");

                // 1.1 BALIDAZIOA (XSD)
                if (!BalidatuXML(file))
                {
                    Console.WriteLine("      ERROREA: XML honek ez du eskema betetzen. Saltatzen...");
                    continue;
                }

                // 1.2 DESERIALIZAZIOA
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<TIKETA>));
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        List<TIKETA> xmlTiketak = (List<TIKETA>)serializer.Deserialize(fs);

                        // Datu basean txertatu banan-banan
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            foreach (var t in xmlTiketak)
                            {
                                string insertQ = "INSERT INTO ticketa (data, kantitatea, prezioa_kg, totala, id_saltzailea, id_produktua) VALUES (@d, @k, @p, @t, @s, @pr)";
                                MySqlCommand cmd = new MySqlCommand(insertQ, conn);
                                cmd.Parameters.AddWithValue("@d", t.Data);
                                cmd.Parameters.AddWithValue("@k", t.Kantitatea);
                                cmd.Parameters.AddWithValue("@p", t.PrezioaKg);
                                cmd.Parameters.AddWithValue("@t", t.Totala);
                                cmd.Parameters.AddWithValue("@s", t.IdSaltzailea);
                                cmd.Parameters.AddWithValue("@pr", t.IdProduktua);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tiketaGuztiak.AddRange(xmlTiketak);
                        prozesatutakoFitxategiak.Add(file);
                        Console.WriteLine($"      Ondo: {xmlTiketak.Count} tiketa inportatu dira.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("      ERROREA XML irakurtzean: " + ex.Message);
                }
            }

            // ---------------------------------------------------------
            // 2. URRATSA: TXT FITXATEGIAK IRAKURRI
            // ---------------------------------------------------------
            Console.WriteLine("\n2. TXT fitxategiak bilatzen...");
            string[] txtFiles = Directory.GetFiles(rootPath, "*.txt");

            foreach (string file in txtFiles)
            {
                Console.WriteLine($"   -> Prozesatzen: {Path.GetFileName(file)}");
                string[] lines = File.ReadAllLines(file);

                // Saltzailea (fitxategiaren izenetik edo ausaz)
                // Sinplifikatzeko: Ausazko saltzailea (1, 2 edo 3)
                int saltzaileId = rnd.Next(1, 4);

                List<TIKETA> txtTiketak = new List<TIKETA>();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try { conn.Open(); } catch { }

                    foreach (string line in lines)
                    {
                        // Formatua: Produktua#Kantitatea#Prezioa
                        // Adib: "Sagarra#2.5#1.20"
                        try
                        {
                            string[] parts = line.Split('#');
                            string pIzena = parts[0].Trim();
                            decimal kant = Convert.ToDecimal(parts[1]);
                            decimal prezioa = Convert.ToDecimal(parts[2]);
                            decimal totala = kant * prezioa;

                            // Produktuaren IDa bilatu
                            int prodId = 1; // Default
                            if (dbProduktuMapaketa.ContainsKey(pIzena.ToLower()))
                            {
                                prodId = dbProduktuMapaketa[pIzena.ToLower()];
                            }

                            TIKETA t = new TIKETA()
                            {
                                Data = DateTime.Now,
                                Kantitatea = kant,
                                PrezioaKg = prezioa,
                                Totala = totala,
                                IdSaltzailea = saltzaileId,
                                IdProduktua = prodId
                            };

                            // DB INSERT
                            string insertQ = "INSERT INTO ticketa (data, kantitatea, prezioa_kg, totala, id_saltzailea, id_produktua) VALUES (@d, @k, @p, @t, @s, @pr)";
                            MySqlCommand cmd = new MySqlCommand(insertQ, conn);
                            cmd.Parameters.AddWithValue("@d", t.Data);
                            cmd.Parameters.AddWithValue("@k", t.Kantitatea);
                            cmd.Parameters.AddWithValue("@p", t.PrezioaKg);
                            cmd.Parameters.AddWithValue("@t", t.Totala);
                            cmd.Parameters.AddWithValue("@s", t.IdSaltzailea);
                            cmd.Parameters.AddWithValue("@pr", t.IdProduktua);
                            cmd.ExecuteNonQuery();

                            txtTiketak.Add(t);
                        }
                        catch
                        {
                            // Lerro okerra bada, saltatu
                        }
                    }
                }
                if (txtTiketak.Count > 0)
                {
                    tiketaGuztiak.AddRange(txtTiketak);
                    prozesatutakoFitxategiak.Add(file);
                    Console.WriteLine($"      Ondo: {txtTiketak.Count} tiketa sortu dira TXTtik.");
                }
            }

            // ---------------------------------------------------------
            // 3. URRATSA: FITXATEGIAK MUGITU BACKUP KARPETARA
            // ---------------------------------------------------------
            Console.WriteLine("\n3. Fitxategiak artxibatzen...");
            foreach (string file in prozesatutakoFitxategiak)
            {
                string destFile = Path.Combine(backupPath, Path.GetFileName(file));
                // Izen bera badu, gainidatzi edo ezabatu aurretik
                if (File.Exists(destFile)) File.Delete(destFile);
                File.Move(file, destFile);
            }

            // ---------------------------------------------------------
            // 4. URRATSA: XML SORTU, EMAIL BIDALI ETA EXCEL EGUNERATU
            // ---------------------------------------------------------
            if (tiketaGuztiak.Count > 0)
            {
                // A) XML Berria sortu
                string xmlPath = XMLsortu.SortuXML(tiketaGuztiak);

                // B) Email bidali
                KudeatuEmail.Bidali(xmlPath);

                // C) Excel (CSV) eguneratu
                EXCELkudeatu.KudeatuExcel.GordeCSV(tiketaGuztiak);
            }
            else
            {
                Console.WriteLine("Ez da daturik inportatu.");
            }
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