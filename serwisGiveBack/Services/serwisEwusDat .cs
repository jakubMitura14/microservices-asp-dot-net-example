using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Patients.Model
{
    public class serwisEwusDat
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOkInNFZ { get; set; }


    }
}

