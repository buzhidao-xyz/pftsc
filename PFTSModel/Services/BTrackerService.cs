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
        /// 人员转移
        /// </summary>
        /// <param name="id"></param>
        /// <param name="recover"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool TransferSuspect(int id, bool recover, string remark)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    btracker btk = db.btracker.SingleOrDefault<btracker>(rec => rec.id == id);
                    btk.status = 1;
                    btk.remark = remark;
                    btk.recover = recover;
                    btk.out_time = DateTime.Now;
                    btk.vest_id = null;
                    if (recover)
                        btk.locker_id = null;
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// 更改采集状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool GatherSuspect(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    btracker btk = db.btracker.SingleOrDefault<btracker>(rec => rec.id == id);
                    btk.gather_status = 1;
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// 更改搜身状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool FriskSuspect(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    btracker btk = db.btracker.SingleOrDefault<btracker>(rec => rec.id == id);
                    btk.frisk_status = 1;
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {

            }
            return false;
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
                                where q.vest_id == vt.id && (vt.no_left == no || vt.no_right == no)
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

        private static List<int[]> s_paths;
        public static List<int[]> SPaths
        {
            get
            {
                if (s_paths == null)
                {
                    s_paths = new List<int[]>();
                    s_paths.Add(new int[] {1,2,33,34,35,36,38,39,41,42,32,31,30,29 });
                    s_paths.Add(new int[] { 1, 2, 33,34,35,36,37});
                    s_paths.Add(new int[] { 1, 2, 33,34,35,36,38,39,40});
                    s_paths.Add(new int[] {37,38,39,40});
                    s_paths.Add(new int[] {37,38,39,41,42});
                    s_paths.Add(new int[] {40,41,42});
                }
                return s_paths;
            }
        }

        // 不存在的路径需要优化
        // 1 : 1 2 33 34 35 36 38 39 41 42 32 31 30 29
        // 2 : 1 2 33 34 35 36 37 
        // 3 : 1 2 33 34 35 36 38 39 40
        // 4 : 37 38 39 40 
        // 5 : 37 38 39 41 42
        // 6 : 40 41 42
        // 返回ids
        private List<int> OptimizationPath(PFTSDbDataContext db, int startRoomId,int endRoomId)
        {
            var pathSs = from q in db.GetTable<path_rfid>()
                         where q.start_room_id == startRoomId
                         select q;
            List<path_rfid> pss = pathSs.ToList();
            var pathEs = from q in db.GetTable<path_rfid>()
                         where q.end_room_id == endRoomId
                         select q;
            List<path_rfid> pes = pathEs.ToList();

            int min = 100;
            bool asc = true;
            int spos = 0, epos = 0;
            int index = -1;

            // 查找最短路径
            for( var x = 0; x < BTrackerService.SPaths.Count;x++)
            {
                var ips = BTrackerService.SPaths[x];
                int smin = ips.Length, smax = -1;
                int emin = ips.Length, emax = -1;
                bool ascTemp = true;    //正向 or 反向
                int sposTemp, eposTemp;
                foreach(var ps in pss)
                {
                    for (var i = 0;i < ips.Length;i++)
                    {
                        if (ps.end_room_id == ips[i])
                        {
                            if (i < smin) smin = i;
                            if (i > smax) smax = i;
                        }
                    }
                }
                foreach(var pe in pes)
                {
                    for (var i = 0;i < ips.Length; i++)
                    {
                        if (pe.start_room_id == ips[i])
                        {
                            if (i < emin) emin = i;
                            if (i > emax) emax = i;
                        }
                    }
                }
                int minPath = smin > emin ? smin - emin : emin - smin;
                sposTemp = smin;
                eposTemp = emin;
                ascTemp = smin < emin;
                int temp = smin > emax ? smin - emax : emax - smin;
                if (temp < minPath)
                {
                    minPath = temp;
                    sposTemp = smin;
                    eposTemp = emax;
                    ascTemp = smin < emax;
                }
                temp = smax > emin ? smax - emin : emin - smax;
                if (temp < minPath)
                {
                    minPath = temp;
                    sposTemp = smax;
                    eposTemp = emin;
                    ascTemp = smax < emin;
                }
                temp = smax > emax ? smax - emax : emax - smax;
                if (temp < minPath)
                {
                    minPath = temp;
                    sposTemp = smax;
                    eposTemp = emax;
                    ascTemp = smax < emax;
                }
                if (sposTemp >= 0 && sposTemp < ips.Length && eposTemp >= 0 && eposTemp < ips.Length && minPath < min)
                {
                    index = x;
                    min = minPath;
                    spos = sposTemp;
                    epos = eposTemp;
                    asc = ascTemp;
                }
            }
            // not found
            if (min == 100)
            {
                return (new List<int>());
            }
            var rets = new List<int>();
            if (asc)
            {
                for (int i = spos; i <= epos; i++)
                {
                    rets.Add(BTrackerService.SPaths[index][i]);
                }
            }
            else
            {
                for (int i = spos; i >= epos; i--)
                {
                    rets.Add(BTrackerService.SPaths[index][i]);
                }
            }
            return rets;
        }

        public bool MoveTo(int id, view_rfid_info position)
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

                if (path == null && bt.room_id != null && position.room_id != null)
                {
                    var optPaths = OptimizationPath(db, bt.room_id.Value, position.room_id.Value);
                    if (optPaths.Count > 0)
                    {
                        optPaths.Add(position.room_id.Value);
                        for (int i = 0; i < optPaths.Count - 1; i++)
                        {
                            var apathq = from q in db.GetTable<path_rfid>()
                                        where q.start_room_id == optPaths[i] && q.end_room_id == optPaths[i+1]
                                        select q;
                            path_rfid apath = apathq.SingleOrDefault<path_rfid>();
                            if (apath == null)
                            {
                                tran.Rollback();
                                return false;
                            }
                            var abp = new btracker_path();
                            abp.btracker_id = bt.id;
                            abp.path_id = apath.id;
                            if (i == 0)
                                abp.start_time = bt.in_room_time.Value;
                            else
                                abp.start_time = n;
                            abp.end_time = n;

                            db.btracker_path.InsertOnSubmit(abp);
                            db.SubmitChanges();
                        }
                        bt.room_id = position.room_id;
                        bt.in_room_time = n;
                        db.SubmitChanges();

                        tran.Commit();
                        return true;
                    }
                    tran.Rollback();
                    return false;
                }


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

        public int GetCount(int? status)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
                    IQueryable<btracker> query = null;
                    if (status == null)
                    {
                        query = from q in table
                                select q;
                    }
                    else
                    {
                        query = from q in table
                                where q.status == status
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
        public List<view_btracker_info> GetPageByStatus(int? status, int pageIndex, int pageSize)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_btracker_info> table = db.GetTable<view_btracker_info>();
                  
                    if (status == null)
                    {
                        var query = from q in table
                                    select q;
                        query = query.Skip(pageIndex).Take(pageSize);
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                where q.status == status
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


        /// <summary>
        /// 获取嫌疑犯的视频数据
        /// </summary>
        /// <returns></returns>
        public List<view_btracker_video> GetVideoByBtracker(int btracker_id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_btracker_video> table = db.GetTable<view_btracker_video>();
                    var query = from q in table
                                where q.btracker_id == btracker_id
                                orderby q.start_time ascending
                                select q;
                    return query.ToList<view_btracker_video>();
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
