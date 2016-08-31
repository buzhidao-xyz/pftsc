using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class DevRFIDService : Service<dev_rfid>
    {
        /// <summary>
        /// 通过name获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_rfid GetByName(string name, string oldName)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_rfid> table = db.GetTable<dev_rfid>();
                    var query = from dv in db.dev_rfid
                                where dv.name == name
                                select dv;
                    if (!String.IsNullOrEmpty(oldName) && oldName == name)
                        return null;
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
        public dev_rfid GetByNo(string no, string oldNo = null)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_rfid> table = db.GetTable<dev_rfid>();
                    var query = from dv in db.dev_rfid
                                where dv.no == no
                                select dv;
                    if (!String.IsNullOrEmpty(oldNo) && oldNo == no)
                        return null;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }
            return null;
        }

        public int GetCount(bool? used)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_rfid> table = db.GetTable<dev_rfid>();
                    IQueryable<dev_rfid> query = null;
                    if (used == null)
                    {
                        query = from q in table
                                select q;
                    }
                    else if (used.Value)
                    {
                        query = from q in table
                                where q.room_id != null
                                select q;
                    }
                    else
                    {
                        query = from q in table
                                where q.room_id == null
                                select q;
                    }

                    return query.Count();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        /// <summary>
        /// 根据状态获取数据列表（分页）
        /// </summary>
        /// <param name="status">状态，为null查询所有</param>
        /// <returns></returns>
        public List<view_rfid_info> GetPageByStatus(bool? used, int pageIndex, int pageSize)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_rfid_info> table = db.GetTable<view_rfid_info>();
                    if (used == null)
                    {
                        var query = from q in table
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                    else if (used.Value)
                    {
                        var query = from q in table
                                    where q.room_id != null
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                    where q.room_id == null
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public bool ModifyLocker(view_rfid_info model)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    dev_rfid rfid = db.dev_rfid.SingleOrDefault<dev_rfid>(rec => rec.id == model.id);
                    rfid.name = model.name;
                    rfid.no = model.no;
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
