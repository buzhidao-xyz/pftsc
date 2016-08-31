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
        /// 通过马甲使用状态查询马甲
        /// </summary>
        /// <param name="used"></param>
        /// <returns></returns>
        public List<view_vest_info> GetVestByStatus(bool? used)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_vest_info> table = db.GetTable<view_vest_info>();
                    if (used == null)
                    {
                        var query = from q in table
                                    select q;
                        return query.ToList();
                    }else if (used.Value)
                    {
                        var query = from q in table
                                    where q.btracker_vest_id != null
                                    select q;
                        return query.ToList();

                    }
                    else
                    {
                        var query = from q in table
                                    where q.btracker_vest_id == null
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

        public int GetVestCount(bool? used)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_vest_info> table = db.GetTable<view_vest_info>();
                    var query = from q in table
                                select q;

                    if (used != null)
                    {
                        if (used.Value)
                        {
                            query = query.Where(p => p.btracker_vest_id != null);
                        }
                        else
                        {
                            query = query.Where(p => p.btracker_vest_id == null);
                        }
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
        public List<view_vest_info> GetPageVestByStatus(bool? used, int pageIndex, int pageSize)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_vest_info> table = db.GetTable<view_vest_info>();
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
                                    where q.btracker_vest_id != null
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                    where q.btracker_vest_id == null
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                    //var query = from q in table
                    //             select new VestInfoEntity
                    //             {
                    //                 id = q.id,
                    //                 no_left = q.no_left,
                    //                 name = q.name,
                    //                 create_time = q.create_time,
                    //                 btracker_name = q.btracker1.name,
                    //                 status = q.status
                    //             };

                    //if (status != null)
                    //{
                    //    query = query.Where(p => p.status == status);
                    //}
                    //query = query.Skip(pageIndex).Take(pageSize);
                    //return query.ToList();
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
        public dev_vest GetByNo(string no, string oldNo)
        {
            try
            {
                //using (PFTSDbDataContext db = new PFTSDbDataContext())
                //{
                //    System.Data.Linq.Table<dev_vest> table = db.GetTable<dev_vest>();
                //    var query = from dv in db.dev_vest
                //                where dv.no == no
                //                select dv;
                //    if (!String.IsNullOrEmpty(oldNo) && oldNo == no)
                //        query = query.Where(c => c.no == oldNo + no);
                //    return query.FirstOrDefault();
                //}
            }
            catch
            {

            }
            return null;
        }

        public bool ModifyVest(view_vest_info model)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    dev_vest vest = db.dev_vest.SingleOrDefault<dev_vest>(rec => rec.id == model.id);
                    vest.name = model.name;
                    vest.no_left = model.no_left;
                    vest.no_right = model.no_right;
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
