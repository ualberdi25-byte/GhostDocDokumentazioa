using System;
using SUPERMERKATUA;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Aplikazioaren klase nagusia. Programaren exekuzioa hemen hasten da.
    /// </summary>
    class Program
    {
        // ---------------------------------------------------------
        // KONEXIO KATEA (Connection String)
        // ---------------------------------------------------------
        static string connectionString = "Server=127.0.0.1;Database=supermerkatua;Uid=root;Pwd=root;";

        /// <summary>
        /// Aplikazioaren sarrera puntua (Main). Menua erakutsi eta erabiltzailearen aukerak kudeatzen ditu.
        /// </summary>
        /// <param name="args">Komando lerroko argumentuak.</param>
        static void Main(string[] args)
        {
            bool irten = false;
            while (!irten)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("       SUPERMERKATUA KUDEAKETA");
                Console.WriteLine("========================================");
                Console.ResetColor();
                Console.WriteLine("1. INPORTATU");
                Console.WriteLine("2. ESTADISTIKAK");
                Console.WriteLine("0. IRTEN");
                Console.WriteLine("========================================");
                Console.Write("Aukeratu zenbaki bat: ");

                string aukera = Console.ReadLine();

                switch (aukera)
                {
                    case "1":
                        // XML eta TXT fitxategiak kargatu, DB bete, eta Backup egin
                        FitxategiaKargatu.DatuakInportatu(connectionString);
                        Console.ReadKey();
                        break;
                    case "2":
                        ErakutsiEstadistikakMenu();
                        break;
                    case "0":
                        irten = true;
                        Console.WriteLine("Agur!");
                        break;
                    default:
                        Console.WriteLine("Aukera okerra. Saiatu berriro.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Estatistikak ikusteko azpimenu bat erakusten du.
        /// </summary>
        static void ErakutsiEstadistikakMenu()
        {
            bool atzera = false;
            while (!atzera)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---------- ESTADISTIKAK ----------");
                Console.ResetColor();
                Console.WriteLine("1. Gehien saldu den produktua");
                Console.WriteLine("2. Gutxien saldu den produktua");
                Console.WriteLine("3. Gehien saldu duen saltzailea");
                Console.WriteLine("4. Gutxien saldu duen saltzailea");
                Console.WriteLine("5. Gehien saldu den eguna");
                Console.WriteLine("6. Saltzaile bakoitzak zenbat saldu du");
                Console.WriteLine("7. DENAK BATERA IKUSI");
                Console.WriteLine("0. ATZERA");
                Console.Write("Aukeratu: ");

                string aukera = Console.ReadLine();

                switch (aukera)
                {
                    case "1":
                        ESTADISTIKAK.GehienSalduDenProduktua(connectionString);
                        break;
                    case "2":
                        ESTADISTIKAK.GutxienSalduDenProduktua(connectionString);
                        break;
                    case "3":
                        ESTADISTIKAK.GehienSalduDuenSaltzailea(connectionString);
                        break;
                    case "4":
                        ESTADISTIKAK.GutxienSalduDuenSaltzailea(connectionString);
                        break;
                    case "5":
                        ESTADISTIKAK.GehienSalduDenEguna(connectionString);
                        break;
                    case "6":
                        ESTADISTIKAK.SaldutakoaSaltzaileko(connectionString);
                        break;
                    case "7":
                        // Denak segidan 
                        ESTADISTIKAK.GehienSalduDenProduktua(connectionString);
                        ESTADISTIKAK.GutxienSalduDenProduktua(connectionString);
                        ESTADISTIKAK.GehienSalduDuenSaltzailea(connectionString);
                        ESTADISTIKAK.GutxienSalduDuenSaltzailea(connectionString);
                        ESTADISTIKAK.GehienSalduDenEguna(connectionString);
                        ESTADISTIKAK.SaldutakoaSaltzaileko(connectionString);
                        break;
                    case "0":
                        atzera = true;
                        break;
                    default:
                        Console.WriteLine("Aukera okerra.");
                        break;
                }

                if (!atzera)
                {
                    Console.WriteLine("\nSakatu tekla bat jarraitzeko...");
                    Console.ReadKey();
                }
            }
        }
    }
}