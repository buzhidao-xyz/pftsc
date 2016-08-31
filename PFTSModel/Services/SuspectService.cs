using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class SuspectService : Service<btracker>
    {

        //public bool AddBreackerTransaction(btracker btrModel)
        //{
        //    PFTSDbDataContext db = new PFTSDbDataContext();
        //    if (db.Connection.State != ConnectionState.Open)
        //    {
        //        db.Connection.Open();
        //    }

        //    System.Data.Common.DbTransaction tran = db.Connection.BeginTransaction();
        //    db.Transaction = tran; //初始化本地事务
        //    try
        //    {
        //        System.Data.Linq.Table<btracker> table = db.GetTable<btracker>();
        //        table.InsertOnSubmit(btrModel);
        //        db.SubmitChanges();


        //        dev_vest vest = db.dev_vest.SingleOrDefault<dev_vest>(rec => rec.id == btrModel.vest_id);
        //        vest.status = 1;
        //        vest.btracker_id = btrModel.id;
        //        db.SubmitChanges();

        //        dev_lockers locker = db.dev_lockers.SingleOrDefault<dev_lockers>(rec => rec.id == btrModel.locker_id);
        //        locker.status = 1;
        //        locker.btracker_id = btrModel.id;
        //        locker.officer_id = btrModel.officer_id;
        //        db.SubmitChanges();

        //        tran.Commit();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        tran.Rollback();
        //    }
        //    return false;
        //}

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
    }
}
