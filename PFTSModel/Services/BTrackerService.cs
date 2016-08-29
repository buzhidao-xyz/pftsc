using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class BTrackerService : Service<btracker>
    {
        /// <summary>
        /// 获取所有场内的数据
        /// </summary>
        /// <returns></returns>
        public List<btracker> GetAllInscene()
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    var query = from q in table
                                where q.status == 0
                                select q;
                    return query.ToList<btracker>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// 通过马甲RFID序列号获取疑犯信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public btracker GetByVestNo(string no)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    var query = from q in table
                                from vt in db.GetTable<dev_vest>()
                                where q.vest_id == vt.id && ( vt.no_left == no || vt.no_right == no)
                                select q;
                    return query.FirstOrDefault<btracker>();
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
