using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class CameraPositionService : Service<view_position_camera_info>
    {
        /// <summary>
        /// 通过id获取记录值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public view_position_camera_info Get(int id)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_position_camera_info> table = db.GetTable<view_position_camera_info>();
                    var query = from op in db.view_position_camera_info
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
        /// 根据状态获取数据列表
        /// </summary>
        /// <param name="status">状态，为null查询所有</param>
        /// <returns></returns>
        public List<view_position_camera_info> GetPositionByStatus(bool? used)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_position_camera_info> table = db.GetTable<view_position_camera_info>();
                    if (used == null)
                    {
                        var query = from q in table
                                    select q;
                        return query.ToList();
                    }
                    else if (used.Value)
                    {
                        var query = from q in table
                                    where q.position_id != null
                                    select q;
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                    where q.position_id == null
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

        public dev_camera GetByRoomId(int roomId)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<dev_camera> table = db.GetTable<dev_camera>();
                        var query = from c in db.GetTable<dev_camera>()
                                    from p in db.GetTable<position_camera>()
                                    where c.position_id != null && c.position_id == p.id && p.room_id == roomId
                                    select c;
                        return query.SingleOrDefault();
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
