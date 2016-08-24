using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel
{
    public class Service<TEntity> where TEntity : class
    {
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="record">数据表对象</param>
        /// <returns>返回成功与否</returns>
        public bool Insert(TEntity record) {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<TEntity> table = db.GetTable<TEntity>();
                    table.InsertOnSubmit(record);
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }


        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAll()
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<TEntity> table = db.GetTable<TEntity>();
                    var query = from q in table
                                select q;
                    return query.ToList<TEntity>();
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
