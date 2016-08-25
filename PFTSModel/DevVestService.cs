using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class DevVestService : Service<dev_vest>
    {
        /// <summary>
        /// 通过name获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_vest GetByName(string name, string oldName)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_vest> table = db.GetTable<dev_vest>();
                    var query = from dv in db.dev_vest
                                where dv.name == name
                                select dv;
                    if (!String.IsNullOrEmpty(oldName) && oldName == name)
                        query = query.Where(c => c.name == oldName + name);
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 通过no获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_vest GetByNo(string no, string oldNo)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_vest> table = db.GetTable<dev_vest>();
                    var query = from dv in db.dev_vest
                                where dv.no == no
                                select dv;
                    if (!String.IsNullOrEmpty(oldNo) && oldNo == no)
                        query = query.Where(c => c.no == oldNo + no);
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }

        public bool ModifyVest(dev_vest model)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    dev_vest vest = db.dev_vest.SingleOrDefault<dev_vest>(rec => rec.id == model.id);
                    vest.name = model.name;
                    vest.no = model.no;
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
