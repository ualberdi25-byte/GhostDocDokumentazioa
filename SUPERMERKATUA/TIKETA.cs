using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Salmenta baten xehetasunak, erositako produktuak eta totala kudeatzen dituen klasea.
    /// </summary>
    public class TIKETA
    {
        /// <summary>
        /// Tiketaren identifikazio zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Salmenta egin den data eta ordua.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Saldutako kantitatea.
        /// </summary>
        public decimal Kantitatea { get; set; }

        /// <summary>
        /// Produktuaren prezioa kilogramoko.
        /// </summary>
        public decimal PrezioaKg { get; set; }

        /// <summary>
        /// Tiketaren prezio totala.
        /// </summary>
        public decimal Totala { get; set; }

        /// <summary>
        /// Salmenta egin duen saltzailearen IDa.
        /// </summary>
        public int IdSaltzailea { get; set; }

        /// <summary>
        /// Saldutako produktuaren IDa.
        /// </summary>
        public int IdProduktua { get; set; }

        /// <summary>
        /// Tiketa objektu berri bat sortzen du.
        /// </summary>
        public TIKETA() { }
    }
}