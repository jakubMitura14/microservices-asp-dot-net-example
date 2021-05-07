using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Patients.Model
{
    public class serwisGiveBack
    {
        [Key]
        public int ID_column { get; set; }
        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string studyType { get; set; }
        public bool isOktoNFZ { get; set; }
        public bool noContraIndications { get; set; }
        public bool isSavedToPax { get; set; }



    }
}

