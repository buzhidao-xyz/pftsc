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
                var bp = new btracker_path();
                bp.path_id = path.id;
                bp.start_time = DateTime.Now;
                bp.end_time = DateTime.Now;

                db.btracker_path.InsertOnSubmit(bp);
                db.SubmitChanges();

                bt.room_id = position.room_id;
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
    }
}
