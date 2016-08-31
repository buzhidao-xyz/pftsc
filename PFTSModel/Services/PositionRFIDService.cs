using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class PositionRFIDService:Service<position_rfid>
    {
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<position_rfid> GetAllByStatus(System.Nullable<int> status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<position_rfid> table = db.GetTable<position_rfid>();
                    var query = from q in table
                                select q;

                    if (status != null)
                    {
                        query = query.Where(p => p.status == status);
                    }

                    return query.ToList();
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
