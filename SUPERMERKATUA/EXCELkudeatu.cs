using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Datuak Excel formatuan (CSV) antolatzeko eta esportatzeko metodoak dituen klasea.
    /// </summary>
    internal class EXCELkudeatu
    {
        public class KudeatuExcel
        {
            /// <summary>
            /// Tiketen zerrenda CSV fitxategi batean gordetzen du.
            /// </summary>
            /// <param name="zerrenda">Gorde beharreko tiketen zerrenda.</param>
            public static void GordeCSV(List<TIKETA> zerrenda)
            {
                Console.WriteLine("--> Excel (CSV) eguneratzen...");
                string fitxategia = "Salmentak.csv";

                // ez bada exisitzen tituloa jarri
                if (!File.Exists(fitxategia))
                {
                    File.AppendAllText(fitxategia, "DATA;ID_PRODUKTUA;ID_SALTZAILEA;KANTITATEA;TOTALA\n");
                }

                foreach (var t in zerrenda)
                {
                    // punto eta komaz separatuta
                    string lerroa = $"{t.Data};{t.IdProduktua};{t.IdSaltzailea};{t.Kantitatea};{t.Totala}\n";
                    File.AppendAllText(fitxategia, lerroa);
                }
                Console.WriteLine("    Ondo: Datuak Excel-en gehitu dira.");
            }
        }
    }
}