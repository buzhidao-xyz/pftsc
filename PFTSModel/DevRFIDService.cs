using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class DevRFIDService : Service<dev_rfid>
    {
        public dev_rfid GetByNo(string no)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_rfid> table = db.GetTable<dev_rfid>();
                    var query = from q in table
                                where q.no == no
                                select q;
                    return query.LastOrDefault<dev_rfid>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
