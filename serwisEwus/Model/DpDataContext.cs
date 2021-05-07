using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DP.Patients.Model
{
    public class DpDataContext : DbContext
    {
        
                 public DpDataContext(DbContextOptions options) : base(options) {
                   
                }


                public DbSet<serwisEwusDat> serwisEwusDat  { get; set; }

            }
         

        /**
         
         
                  public DpDataContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<Patient> Patients { get; set; }

         */


    }



