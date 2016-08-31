using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSScene
{
    class PFTSResourceLoader
    {
        // 所有房间
        private List<PFTSModel.view_rfid_room_info> _room_infos;
        private List<PFTSModel.view_position_camera_info> _camera_positions;
        private List<PFTSModel.path_rfid> _rfid_paths;

        /// <summary>
        /// 获取所有room数据
        /// </summary>
        public List<PFTSModel.view_rfid_room_info> RoomInfos
        {
            get
            {
                if (_room_infos == null)
                {
                    _room_infos = (new PFTSModel.Service<PFTSModel.view_rfid_room_info>()).GetAll();
                }
                return _room_infos;
            }
        }

        /// <summary>
        /// 获取所有摄像头位置数据
        /// </summary>
        public List<PFTSModel.view_position_camera_info> PositionCameras
        {
            get
            {
                if (_camera_positions == null)
                {
                    _camera_positions = (new PFTSModel.Service<PFTSModel.view_position_camera_info>()).GetAll();
                }
                return _camera_positions;
            }
        }

        /// <summary>
        /// 获取所有路径信息
        /// </summary>
        public List<PFTSModel.path_rfid> Paths
        {
            get
            {
                if (_rfid_paths == null)
                {
                    _rfid_paths = (new PFTSModel.Service<PFTSModel.path_rfid>()).GetAll();
                }
                return _rfid_paths;
            }
        }
    }
}
