using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUPERMERKATUA
{
    /// <summary>
    /// Saltzailearen informazio pertsonala eta profesionala kudeatzen duen klasea.
    /// </summary>
    public class Saltzailea
    {
        /// <summary>
        /// Saltzailearen identifikazio zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Saltzailearen izena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Saltzailearen Nortasun Agiriaren Zenbakia (NAN).
        /// </summary>
        public string Nan { get; set; }

        /// <summary>
        /// Saltzailearen instantzia berri bat sortzen du.
        /// </summary>
        public Saltzailea() { }
    }
}