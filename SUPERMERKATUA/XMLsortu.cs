using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SUPERMERKATUA
{
    public class XMLsortu
    {
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

