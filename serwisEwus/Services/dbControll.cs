using DP.Patients.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwisEwus.Services
{
    public class DbControll
    {
        private DpDataContext _context;

        public DbControll(DpDataContext context)
        {
            _context = context;
        }

        /**
     zapisujemy informacje do bazy danych
     */
        public async Task saveToDb(serwisEwusDat dat)
        {

            _context.serwisEwusDat.Add(dat);
            _context.SaveChanges();


          //  context.serwisEwusDat.Add(dat);
           // context.SaveChanges();
            Console.WriteLine("13");

        }
    }
}
