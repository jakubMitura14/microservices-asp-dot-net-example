using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektCtiMRI.Modules
{
    public class CTiMRIDataContext : Microsoft.EntityFrameworkCore.DbContext

    {
        public CTiMRIDataContext(DbContextOptions options) : base(options) { 
        }
        public DbSet<CTiMRI> CTiMRI { get; set; }
    }
}
