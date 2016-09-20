using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class VideoService : Service<video>
    {
        public bool AddVideo(video vd,List<btracker> btrackers,btracker appendBt = null,btracker exceptBt = null)
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
                if (exceptBt != null)
                {
                    btrackers = btrackers.Where(l => l.id != exceptBt.id).ToList();
                }
                if (appendBt != null)
                {
                    btrackers = btrackers.Where(l => l.id != appendBt.id).ToList();
                    btrackers.Add(appendBt);
                }

                db.video.InsertOnSubmit(vd);
                db.SubmitChanges();

                var vrs = new List<video_btracker_r>();
                foreach(var b in btrackers)
                {
                    var vr = new video_btracker_r();
                    vr.video_id = vd.id;
                    vr.btracker_id = b.id;
                    vrs.Add(vr);
                }
                db.video_btracker_r.InsertAllOnSubmit(vrs);
                db.SubmitChanges();

                //var bp = new btracker_path();
                //bp.btracker_id = bt.id;
                //bp.path_id = path.id;
                //bp.start_time = bt.in_room_time.Value;
                //bp.end_time = n;

                //db.btracker_path.InsertOnSubmit(bp);
                //db.SubmitChanges();

                //bt.room_id = position.room_id;
                //bt.in_room_time = n;
                //db.SubmitChanges();

                tran.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tran.Rollback();
            }
            return false;
            return false;
        }
    }
}
