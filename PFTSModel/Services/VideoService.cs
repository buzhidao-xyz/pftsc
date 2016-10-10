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
                    vr.start_time = vd.start_time;
                    vr.end_time = vd.end_time;
                    vrs.Add(vr);
                }
                db.video_btracker_r.InsertAllOnSubmit(vrs);
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

        public bool AddVideo(video vd, Dictionary<int,video_btracker_r> mvrs)
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
                db.video.InsertOnSubmit(vd);
                db.SubmitChanges();

                foreach( var k in mvrs.Keys)
                {
                    mvrs[k].video_id = vd.id;
                }
                db.video_btracker_r.InsertAllOnSubmit(mvrs.Values);
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
