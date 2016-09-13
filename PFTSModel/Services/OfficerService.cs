using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class OfficerService : Service<officer>
    {
        /// <summary>
        /// 获取警员信息 通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public officer GetOfficerByID(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<officer> table = db.GetTable<officer>();
                    var query = from op in db.@officer
                                where op.id == id
                                select op;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// 查询警员指纹信息
        /// </summary>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public List<officer_fingerprint> GetFingerPrintList(int officerID)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<officer_fingerprint> table = db.GetTable<officer_fingerprint>();
                    var query = from op in db.officer_fingerprint
                                where op.officer_id == officerID
                                select op;
                    return query.ToList();
                }
            }
            catch
            {

            }

            return null;
        }
        /// <summary>
        /// 获取警员信息 通过no
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public officer GetOfficerByNo(string no, int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<officer> table = db.GetTable<officer>();
                    var query = from op in db.@officer
                                where op.no == no && op.id != id
                                select op;
                    return query.FirstOrDefault();
                }
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// 获取警员列表
        /// </summary>
        /// <returns></returns>
        public List<officer> GetOfficerList(int skip, int limit)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<officer> table = db.GetTable<officer>();
                    var query = from q in table
                                select q;

                    query = query.Skip(skip).Take(limit);

                    return query.ToList();
                }
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 更新警员
        /// </summary>
        /// <param name="PoliceInfo"></param>
        /// <returns></returns>
        public bool UpPoliceByID(officer PoliceInfo, List<officer_fingerprint> fingerList)
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
                System.Data.Linq.Table<officer> table = db.GetTable<officer>();
                var query = from op in db.officer
                            where op.id == PoliceInfo.id
                            select op;
                foreach (var p in query)
                {
                    p.no = PoliceInfo.no;
                    p.name = PoliceInfo.name;
                    p.sex = PoliceInfo.sex;
                }
                db.SubmitChanges();

                if (fingerList != null && fingerList.Count > 0)
                {
                    System.Data.Linq.Table<officer_fingerprint> fingerTable = db.GetTable<officer_fingerprint>();
                    foreach(officer_fingerprint model in fingerList)
                    {
                        if (model.id==0)
                        {
                            fingerTable.InsertOnSubmit(model);
                            db.SubmitChanges();
                        }
                        else
                        {
                            officer_fingerprint finger = db.officer_fingerprint.SingleOrDefault<officer_fingerprint>(rec => rec.id == model.id);
                            finger.img = model.img;
                            db.SubmitChanges();
                        }
                    }
                }
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

        public int GetOfficerCount()
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<officer> table = db.GetTable<officer>();
                    var query = from q in table
                                select q;

                    return query.Count();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }
    }
}
