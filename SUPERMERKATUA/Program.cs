using System;
using SUPERMERKATUA; // Ziurtatu hau jarrita dagoela beste klaseak aurkitzeko

namespace SUPERMERKATUA
{
    class Program
    {
        // ---------------------------------------------------------
        // KONEXIO KATEA (Connection String)
        // GARRANTZITSUA: Ziurtatu 'Pwd' (pasahitza) MySQL Workbench-ekoa dela.
        // Batzuetan hutsik dago ("Pwd=;") edo "root" edo "1234" da.
        // ---------------------------------------------------------
        static string connectionString = "Server=127.0.0.1;Database=supermerkatua;Uid=root;Pwd=root;";

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
                Console.WriteLine("1.INPORTATU");
                Console.WriteLine("2. ESTADISTIKAK");
                Console.WriteLine("0. IRTEN");
                Console.WriteLine("========================================");
                Console.Write("Aukeratu zenbaki bat: ");

                string aukera = Console.ReadLine();

                switch (aukera)
                {
                    case "1":
                        // 1. INPORTATU
                        ErakutsiDatuak();
                        break;

                    case "2":
                        // 2. ESTADISTIKAK
                        Estadistika();
                        break;

                    case "0":
                        Console.WriteLine("IRTETEN....");
                        irten = true;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Aukera okerra. Saiatu berriro.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        // --- FUNTZIO LAGUNTZAILEAK ---

        static void ErakutsiDatuak()
        {
            // FitxategiaKargatu klaseari deitzen diogu, konexio katea pasatuz
            try
            {
                FitxategiaKargatu.DatuakInportatu(connectionString);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Errore kritikoa Program.cs-en: " + ex.Message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        static void Estadistika()
        {
            bool atzera = false;
            while (!atzera)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== ESTATISTIKAK MENU ===");
                Console.ResetColor();
                Console.WriteLine("1. Gehien saldu den produktua");
                Console.WriteLine("2. Gutxien saldu den produktua");
                Console.WriteLine("3. Gehien saldu duen saltzailea");
                Console.WriteLine("4. Gutxien saldu duen saltzailea");
                Console.WriteLine("5. Gehien saldu den eguna");
                Console.WriteLine("6. Saldutakoa saltzaileko (Zerrenda)");
                Console.WriteLine("7. DENAK IKUSI BATERA");
                Console.WriteLine("0. ATZERA");
                Console.WriteLine("=========================");
                Console.Write("Aukeratu: ");

                string aukera = Console.ReadLine();

                Console.WriteLine("\n--- EMAITZA ---");

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