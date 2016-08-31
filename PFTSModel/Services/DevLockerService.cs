using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class DevLockerService : Service<dev_lockers>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<view_locker_info> GetLockersByStatus(bool? status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_locker_info> table = db.GetTable<view_locker_info>();
                    if (status == null)
                    {
                        var query = from q in table
                                    select q;
                        return query.ToList();
                    }
                    else if (status.Value)
                    {
                        var query = from q in table
                                    where q.btracker_officer_id != null
                                    select q;
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                    where q.btracker_officer_id == null
                                    select q;
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

        /// <summary>
        /// 通过name获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dev_lockers GetByName(string name, string oldName)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_lockers> table = db.GetTable<dev_lockers>();
                    var query = from dv in db.dev_lockers
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
        public dev_lockers GetByNo(string no, string oldNo)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_lockers> table = db.GetTable<dev_lockers>();
                    var query = from dv in db.dev_lockers
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
                    System.Data.Linq.Table<view_locker_info> table = db.GetTable<view_locker_info>();
                    IQueryable<view_locker_info> query = null;
                    if (used == null)
                    {
                        query = from q in table
                                select q;
                    }
                    else if (used.Value)
                    {
                        query = from q in table
                                where q.btracker_officer_id != null
                                select q;
                    }
                    else
                    {
                        query = from q in table
                                where q.btracker_officer_id == null
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
        public List<view_locker_info> GetPageByStatus(bool? used, int pageIndex, int pageSize)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_locker_info> table = db.GetTable<view_locker_info>();
                    IQueryable<view_locker_info> query = null;
                    if (used == null)
                    {
                        query = from q in table
                                select q;
                    }
                    else if (used.Value)
                    {
                        query = from q in table
                                where q.btracker_officer_id != null
                                select q;
                    }
                    else
                    {
                        query = from q in table
                                where q.btracker_officer_id == null
                                select q;

                    }
                    query = query.Skip(pageIndex).Take(pageSize);
                    return query.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public bool ModifyLocker(view_locker_info model)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    dev_lockers locker = db.dev_lockers.SingleOrDefault<dev_lockers>(rec => rec.id == model.id);
                    locker.name = model.name;
                    locker.no = model.no;
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
