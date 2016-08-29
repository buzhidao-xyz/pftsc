using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class SuspectService : Service<btracker>
    {

        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<BtrackerInfoEntity> GetAllByIn(System.Nullable<int> status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    var query = from q in table
                                from r in db.GetTable<officer>()
                                where q.officer_id == r.id
                                select new BtrackerInfoEntity
                                {
                                    id = q.id,
                                    no = q.no,
                                    name = q.name,
                                    number = q.number,
                                    sex = q.sex,
                                    officer_name = r.name,
                                    in_time = q.in_time,
                                    status = q.status
                                };

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
