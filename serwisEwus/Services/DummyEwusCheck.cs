using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwisEwus.Services
{
    /**
     udajemy ze sprawdzamy czy pacjent jest zarejestrowany w EWUS
     */
    public class DummyEwusCheck
    {
        public async Task<bool> checkEwus(DP.Patients.Services.serwisDbDat payload)
        {
            return true;
        }

    }
}

