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
        private List<PFTSModel.room> _rooms;
        private List<PFTSModel.position_camera> _camera_positions;
        private List<PFTSModel.position_rfid> _rfid_positions;
        private List<PFTSModel.path_rfid> _rfid_paths;

        /// <summary>
        /// 获取所有room数据
        /// </summary>
        public List<PFTSModel.room> Rooms
        {
            get
            {
                if (_rooms == null)
                {
                    _rooms = (new PFTSModel.Service<PFTSModel.room>()).GetAll();
                }
                return _rooms;
            }
        }

        /// <summary>
        /// 获取所有摄像头位置数据
        /// </summary>
        public List<PFTSModel.position_camera> PositionCameras
        {
            get
            {
                if (_camera_positions == null)
                {
                    _camera_positions = (new PFTSModel.Service<PFTSModel.position_camera>()).GetAll();
                }
                return _camera_positions;
            }
        }

        /// <summary>
        /// 获取所有rfid天线位置数据
        /// </summary>
        public List<PFTSModel.position_rfid> PositionRFIDs
        {
            get
            {
                if (_rfid_positions == null)
                {
                    _rfid_positions = (new PFTSModel.Service<PFTSModel.position_rfid>()).GetAll();
                }
                return _rfid_positions;
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
