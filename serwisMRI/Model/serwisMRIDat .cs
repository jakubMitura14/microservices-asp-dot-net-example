﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ctver3.Model
{
    public class serwisMRIDat
    {
        [Key]
        public int ID_column { get; set; }

        public int Pesel { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public bool isOktoMRI { get; set; }


    }
}
