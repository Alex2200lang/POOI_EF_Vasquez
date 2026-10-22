using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace ExamenFinal.Models
{
    public class Consumidor
    {
        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string dnicont { get; set; }

        [Required]
        [StringLength(40)]
        public string nomcont { get; set; }

        [Required]
        [StringLength(40)]
        public string apecont { get; set; }

        [EmailAddress]
        [StringLength(250)]
        public string emailcont { get; set; }

        [StringLength(3)]
        public string idpais { get; set; }
    }
}