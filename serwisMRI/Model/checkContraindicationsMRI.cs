using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ctver3.Services
{
    public class checkContraindicationsMRI
    {
        public bool checkContraindication(DP.Patients.Model.serwisDbDat payload)
        { return !(payload.isFerroMagnetic); }

    }
}
