using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSModel.Services
{
    public class RFIDRoomService:Service<view_rfid_room_info>
    {
        /// <summary>
        /// 根据状态获取数据列表
        /// </summary>
        /// <param name="status">状态，为null查询所有</param>
        /// <returns></returns>
        public List<view_rfid_room_info> GetPositionByStatus(bool? used)
        {
            try
            {
                using (PFTSDbDataContext db = new PFTSDbDataContext())
                {
                    System.Data.Linq.Table<view_rfid_room_info> table = db.GetTable<view_rfid_room_info>();
                    if (used == null)
                    {
                        var query = from q in table
                                    select q;
                        return query.ToList();
                    }
                    else if (used.Value)
                    {
                        var query = from q in table
                                    where q.rfid_room_id != null
                                    select q;
                        return query.ToList();
                    }
                    else
                    {
                        var query = from q in table
                                    where q.rfid_room_id == null
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
    }
}
