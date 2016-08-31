using PFTSModel.Entitys;
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
        /// 根据物品箱状态获取数据列表
        /// </summary>
        /// <param name="status">状态，为null查询所有</param>
        /// <returns></returns>
        public List<LockerInfoEntity> GetLockersByStatus(System.Nullable<int> status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_lockers> table = db.GetTable<dev_lockers>();
                    var query = from q in table
                                select new LockerInfoEntity
                                {
                                    id = q.id,
                                    no = q.no,
                                    name = q.name,
                                    officer_name = q.officer.name,
                                    create_time = q.create_time,
                                    btracker_name = q.btracker1.name,
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
        /// <summary>
        /// 获取物品箱记录总数
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int GetLockerCount(System.Nullable<int> status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_lockers> table = db.GetTable<dev_lockers>();
                    var query = from q in table
                                select q;

                    if (status != null)
                    {
                        query = query.Where(p => p.status == status);
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
        /// 根据物品箱状态获取数据列表（分页）
        /// </summary>
        /// <param name="status">状态，为null查询所有</param>
        /// <returns></returns>
        public List<LockerInfoEntity> GetPageLockerByStatus(System.Nullable<int> status, int pageIndex, int pageSize)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_lockers> table = db.GetTable<dev_lockers>();
                    var query = from q in table
                                select new LockerInfoEntity
                                {
                                    id = q.id,
                                    no = q.no,
                                    name = q.name,
                                    officer_name = q.officer.name,
                                    create_time = q.create_time,
                                    btracker_name = q.btracker1.name,
                                    status = q.status
                                };

                    if (status != null)
                    {
                        query = query.Where(p => p.status == status);
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

        public bool ModifyLocker(LockerInfoEntity model)
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
