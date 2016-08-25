using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class OperatorService : Service<@operator>
    {
        /// <summary>
        /// 通过id获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public @operator Get(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<@operator> table = db.GetTable<@operator>();
                    var query = from op in db.@operator
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
        /// 通过账号获取操作员信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public @operator GetByAccount(string account)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<@operator> table = db.GetTable<@operator>();
                    var query = from op in db.@operator
                                where op.account == account
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
        /// 获取操作员列表
        /// </summary>
        /// <returns></returns>
        public List<@operator> GetOperatorList()
        {
            try
            {
                List<@operator> OperatorList = base.GetAll();

                return OperatorList;
            }
            catch
            {

            }

            return null;
        }

        /// <summary>
        /// 更新操作员信息
        /// </summary>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool OperatorUpInfo(int id, string password, string name)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<@operator> table = db.GetTable<@operator>();

                    var query = from op in db.@operator
                                where op.id == id
                                select op;
                    foreach (var p in query)
                    {
                        if (password.Length != 0)
                        {
                            p.password = password;
                        } 
                        p.name = name;
                    }

                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {

            }

            return true;
        }
    }
}
