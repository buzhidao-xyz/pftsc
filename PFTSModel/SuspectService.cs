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
        public List<btracker> GetAllByIn(int status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    var query = from q in table
                                where q.status == status
                                select q;
                    List<btracker> retList = new List<btracker>();
                    foreach (btracker entity in query)
                    {
                        retList.Add(entity);
                    }
                    return retList;
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
