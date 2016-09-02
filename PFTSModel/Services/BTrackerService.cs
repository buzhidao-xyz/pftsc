using System;
using System.Collections.Generic;
using System.Data;
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

        public btracker Get(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    var query = from q in table
                                where q.id == id
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

        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<view_btracker_info> GetAllByIn(System.Nullable<int> status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_btracker_info> table = db.GetTable<view_btracker_info>();
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

        public bool MoveTo(int id,view_rfid_info position)
        {
            PFTSDbDataContext db = new PFTSDbDataContext();
            if (db.Connection.State != ConnectionState.Open)
            {
                db.Connection.Open();
            }

            System.Data.Common.DbTransaction tran = db.Connection.BeginTransaction();
            db.Transaction = tran; //初始化本地事务
            try
            {
                btracker bt = db.btracker.SingleOrDefault<btracker>(rec => rec.id == id);

                if (bt == null) return false;
                var pathQuery = from q in db.GetTable<path_rfid>()
                                where q.start_room_id == bt.room_id && q.end_room_id == position.room_id
                                select q;
                path_rfid path = pathQuery.FirstOrDefault<path_rfid>();

                var n = DateTime.Now;

                var bp = new btracker_path();
                bp.btracker_id = bt.id;
                bp.path_id = path.id;
                bp.start_time = bt.in_room_time.Value;
                bp.end_time = n;

                db.btracker_path.InsertOnSubmit(bp);
                db.SubmitChanges();

                bt.room_id = position.room_id;
                bt.in_room_time = n;
                db.SubmitChanges();

                tran.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tran.Rollback();
            }
            return false;
        }

        public List<path_rfid> GetPaths(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    var query = from q in db.GetTable<btracker_path>()
                                from p in db.GetTable<path_rfid>()
                                where q.btracker_id == id && q.path_id == p.id
                                orderby q.start_time ascending
                                select p;
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
