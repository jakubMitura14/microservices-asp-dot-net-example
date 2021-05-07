using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Patients.Model
{
    public class serwisDbDat
    {
        public int pesel { get; set; }
        public string studyType { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isFerroMagnetic { get; set; }// prawda jesli sa ferromagnetyczne elementy metalowe w ciele
        public bool isNiewydolNerek { get; set; } // prawda jesli pacjent ma niewydolnosc nerek
        public bool isNadczynnoscTarczycy { get; set; }/// prawda jesli pacjent ma nadczynnosc tarczycy

    }
}

