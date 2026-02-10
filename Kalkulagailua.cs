
using System;

namespace Kalkulagailua
{
    /// <summary>
    /// Programa nagusiaren klasea.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            Kalkulagailu kalkulagailua = new Kalkulagailu();
            kalkulagailua.Hasieratu();
        }
    }

    /// <summary>
    /// Erabiltzailearekin interakzioa kudeatzen duen klasea.
    /// Datuak eskatu eta emaitzak pantailaratzen ditu.
    /// </summary>
    public class Kalkulagailu
    {
        /// <summary>
        /// Kalkulagailuaren fluxu nagusia abiarazten du.
        /// Zenbakiak eskatu, eragiketa aukeratu eta emaitza bistaratzen du.
        /// </summary>
        public void Hasieratu()
        {
            Console.WriteLine("Sartu lehen zenbakia:");
            double zenbaki1 = SartuZenbakia();

            Console.WriteLine("Sartu bigarren zenbakia:");
            double zenbaki2 = SartuZenbakia();

            Console.WriteLine("Hautatu eragiketa (+, -, *, /, %):");
            string eragiketa = Console.ReadLine();

            EragiketaAukera eragiketaObj = new EragiketaAukera();
            double emaitza = eragiketaObj.ErrekinEragiketa(eragiketa, zenbaki1, zenbaki2);

            Console.WriteLine("Emaitza: " + emaitza);
        }

        /// <summary>
        /// /// Kontsolatik sarrera irakurri eta doublen bihurtzen du.
        /// <returns>Erabiltzaileak sartutako zenbakia double formatuan.</returns>
        /// </summary>
        private double SartuZenbakia()
        {
            return Convert.ToDouble(Console.ReadLine());
        }
    }

    /// <summary>
    /// Eragiketak egiten ditu
    /// </summary>
    public class EragiketaAukera
    {
        /// <summary>
        /// Errekins the eragiketa.
        /// </summary>
        /// <param name="eragiketa">The eragiketa.</param>
        /// <param name="zenbaki1">The zenbaki1.</param>
        /// <param name="zenbaki2">The zenbaki2.</param>
        /// <returns></returns>
        public double ErrekinEragiketa(string eragiketa, double zenbaki1, double zenbaki2)
        {
            switch (eragiketa)
            {
                case "+":
                    return ZenbakiakGehitu(zenbaki1, zenbaki2);
                case "-":
                    return ZenbakiakKendu(zenbaki1, zenbaki2);
                case "*":
                    return ZenbakiakBiderkatu(zenbaki1, zenbaki2);
                case "/":
                    return Zatiketa(zenbaki1, zenbaki2);
                case "%":
                    return Moduloa(zenbaki1, zenbaki2);
                default:
                    Console.WriteLine("Errorea: eragiketa ezezaguna.");
                    return 0;
            }
        }

        /// <summary>
        /// Gehiketa egiten du a eta b artean
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double ZenbakiakGehitu(double a, double b)
        {
            return a + b;
        }

        /// <summary>
        /// Kenketa egiten du a eta b artean
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double ZenbakiakKendu(double a, double b)
        {
            return a - b;
        }

        /// <summary>
        /// Biderketa egiten du a eta b artean
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double ZenbakiakBiderkatu(double a, double b)
        {
            return a * b;
        }

        /// <summary>
        /// a eta b zatitzen ditu
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double Zatiketa(double a, double b)
        {
            if (b != 0)
                return a / b;
            else
            {
                Console.WriteLine("Errorea: zeroz zatitzea ezinezkoa da.");
                return 0;
            }
        }

        /// <summary>
        /// zatiketaren hondarra kalkulatzen du eta 0kin zatitzea ekiditen du.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private double Moduloa(double a, double b)
        {
            if (b != 0)
                return a % b;
            else
            {
                Console.WriteLine("Errorea: zeroz zatitzea ezinezkoa da.");
                return 0;
            }
        }
    }
}
