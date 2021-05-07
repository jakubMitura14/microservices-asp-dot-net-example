using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektCtiMRI.Modules
{
    public class CTiMRI
    {
        [Key]
        public int ID_column { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string RodzajBadania { get; set; }
    }
}
