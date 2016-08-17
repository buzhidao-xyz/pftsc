using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class OperatorService : Service<@operator>
    {
        /// <summary>
        /// 通过id获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public @operator Get(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<@operator> table = db.GetTable<@operator>();
                    var query = from op in db.@operator
                                where op.id == id
                                select op;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }
    }
}
