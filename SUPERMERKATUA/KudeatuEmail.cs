using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO; // HAU BEHARREZKOA DA File.Exists erabiltzeko

namespace SUPERMERKATUA
{
    public class KudeatuEmail
    {
        public static void Bidali(string adjuntoPath)
        {
            Console.WriteLine("--> Gmail bidaltzen...");
            try
            {
                // ZURE DATUAK JARRI DITUT HEMEN:
                string nireEmail = "jhuegun25@izarraitz.eus";
                string pasahitza = "hrps itji yjzf fuqd";

                // Zure buruari bidaltzeko:
                string noraBidali = "jhuegun25@izarraitz.eus";

                MailMessage correua = new MailMessage();
                correua.From = new MailAddress(nireEmail);
                correua.To.Add(noraBidali);
                correua.Subject = "Supermerkatua - Backup XML - " + DateTime.Now.ToString();
                correua.Body = "Hemen duzu XML fitxategia atxikita.";

                // Fitxategia erantsi
                if (File.Exists(adjuntoPath))
                {
                    Attachment adjunto = new Attachment(adjuntoPath);
                    correua.Attachments.Add(adjunto);
                }

                SmtpClient server = new SmtpClient("smtp.gmail.com");
                server.Port = 587;
                server.Credentials = new NetworkCredential(nireEmail, pasahitza);
                server.EnableSsl = true;

                server.Send(correua);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   -> ONDO: Emaila bidalita!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   -> ERROREA EMAILAREKIN (Ez da kritikoa): " + ex.Message);
                Console.ResetColor();
            }
        }
    }
}