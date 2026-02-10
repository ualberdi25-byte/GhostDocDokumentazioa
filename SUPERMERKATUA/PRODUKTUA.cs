using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Supermerkatuko produktu baten ezaugarriak (izena, kategoria, id) definitzen dituen klasea.
    /// </summary>
    public class Produktua
    {
        /// <summary>
        /// Produktuaren identifikazio zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Produktuaren kategoria (fruta, barazkia, etab.).
        /// </summary>
        public string Kategoria { get; set; }

        /// <summary>
        /// Produktuaren instantzia berri bat sortzen du.
        /// </summary>
        public Produktua() { }
    }
}