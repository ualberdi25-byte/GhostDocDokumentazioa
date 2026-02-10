using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Datuak (tiketak, salmentak) XML fitxategi formatuan esportatzeko eta egituratzeko klasea.
    /// </summary>
    public class XMLsortu
    {
        /// <summary>
        /// Emandako objektuen zerrenda XML fitxategi batean serializatzen (bihurtzen) du.
        /// </summary>
        /// <param name="zerrenda">Tiketen zerrenda.</param>
        /// <returns>Sortutako fitxategiaren bidea (path).</returns>
        public static string SortuXML(List<TIKETA> zerrenda)
        {
            Console.WriteLine("--> XML Backup sortzen...");

            // 1. KARPETA SORTU
            string karpeta = "Backup";
            if (!Directory.Exists(karpeta)) Directory.CreateDirectory(karpeta);

            // 2. IZENA ETA DATA
            string izena = $"Backup_Tickets_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            string bidea = Path.Combine(karpeta, izena);


            XmlSerializer serializadorea = new XmlSerializer(typeof(List<TIKETA>));
            using (TextWriter writer = new StreamWriter(bidea))
            {
                serializadorea.Serialize(writer, zerrenda);
            }

            Console.WriteLine($"    Ondo: {bidea}");
            return bidea; // RUTA EMAIL
        }
    }
}