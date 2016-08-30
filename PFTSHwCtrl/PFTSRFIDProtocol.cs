using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFTSModel;

namespace PFTSHwCtrl
{
    public class PFTSRFIDProtocol
    {
        /// <summary>
        /// 协议类型
        /// </summary>
        public enum ProtocolType
        {
            PTRFIDTrigger = 0,  //RFID马甲触发
            PTRFIDUnkown = -1
        }

        public enum ProtocolParseState
        {
            // 未解析
            PPSUnParse = 0,
            PPSParsed = 1,
            PPSParsing = 2
        }

        /// <summary>
        /// 帧头      类型  数据长度    RFID天线编号    卡号      帧尾
        ///   2B      1B    1B          1B              4B          1B
        /// 0x68 0x69 0x31  0x05        XX            XX XX XX XX   0x16
        /// </summary>

        // 帧头
        public static byte[] FRAME_HEAD =new byte[]{0x68,0x69};
        public static byte FRAME_TYPE = 0x31;
        public static byte FRAME_DATA_SIZE = 1 + 4;
        public static byte FRAME_END = 0x16;


        #region private member
        private ProtocolType m_type;
        private ProtocolParseState m_state;
        // 嫌疑人参数
        private btracker m_paramBTracker;
        private dev_rfid m_paramRFID;
        #endregion


        #region property
        public ProtocolType Type
        {
            private set
            {
                m_type = value;
            }
            get
            {
                return m_type;
            }
        }

        public btracker BTracker
        {
            private set
            {
                m_paramBTracker = value;
            }
            get
            {
                return m_paramBTracker;
            }
        }
        
        public dev_rfid DevRFID
        {
            private set
            {
                m_paramRFID = value;
            }
            get
            {
                return m_paramRFID;
            }
        }

        public ProtocolParseState State
        {
            private set
            {
                m_state = value;
            }
            get
            {
                return m_state;
            }
        }
        #endregion

        public static PFTSRFIDProtocol Parse(byte[] data,int len)
        {
            var protocal = new PFTSRFIDProtocol();
            protocal.State = ProtocolParseState.PPSUnParse;
            protocal.ParseRaw(data, len);
            return protocal;
        }

        public void ParseRaw(byte[] data, int len)
        {
            if (len < 5) return;
            this.State = ProtocolParseState.PPSParsing;
            string rfidNo = string.Format("{0:X2}",data[0]);
            var devRfid = (new PFTSModel.Services.DevRFIDService()).GetByNo(rfidNo);
            if (devRfid != null)
            {
                this.DevRFID = devRfid;
            }

            string vestNo = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", data[1], data[2], data[3], data[4]);
            var bt = (new PFTSModel.Services.BTrackerService()).GetByVestNo(vestNo);
            if (bt != null)
            {
                this.BTracker = bt;
            }
            if (this.DevRFID != null && this.BTracker != null)
            {
                this.Type = ProtocolType.PTRFIDTrigger;
                this.State = ProtocolParseState.PPSParsed;
            }

        }

        public class ProtocolBuffer
        {
            private byte[] datas = new byte[4096];
            private int pos_start = 0;
            private int pos_end = 0; //不包含在数据当中

            public List<byte[]> Put(byte[] d,int index,int len)
            {
                List<byte[]> rets = new List<byte[]>();
                // 分多次填充
                if (pos_end + len >= 4096)
                {
                    int startIndex = index;
                    int remain = len;
                    while(startIndex < index + len && remain > 0)
                    {
                        int limit = remain > 4096 - pos_end ? 4096 - pos_end : remain;
                        for (int i = startIndex; i < startIndex + limit; i++)
                        {
                            datas[pos_end + (i - startIndex)] = d[i];
                        }
                        pos_end += limit;
                        startIndex += limit;
                        remain -= limit;
                        var rs = ScanFrames();
                        if (rs != null && rs.Count > 0)
                        {
                            foreach( var r in rs)
                            {
                                rets.Add(r);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = index;i < index + len; i++)
                    {
                        datas[pos_end + (i - index)] = d[i];
                    }
                    pos_end += len;
                    var rs = ScanFrames();
                    if (rs != null && rs.Count > 0)
                    {
                        foreach (var r in rs)
                        {
                            rets.Add(r);
                        }
                    }
                }
                return rets;
            }

            public List<byte[]> ScanFrames()
            {
                List<byte[]> rets = new List<byte[]>();
                while(pos_end - pos_start >= 10)
                {
                    for (int i = pos_start; i < pos_end - 9; i++)
                    {
                        if (datas[i] == FRAME_HEAD[0] && datas[i+1] == FRAME_HEAD[1] && datas[i+2] == FRAME_TYPE && datas[i+3] == FRAME_DATA_SIZE
                            && datas[i+9] == FRAME_END)
                        {
                            byte[] frameData = new byte[5];
                            for (int j = i+4;j < i + 9; j++)
                            {
                                frameData[j-i-4] = datas[j];
                            }
                            rets.Add(frameData);
                            i += 10;
//                            pos_start = i + 10;
                        }
                    }
                    pos_start = pos_end;
                }
                // 数据迁移
                if (pos_start >= pos_end)
                {
                    pos_start = 0;
                    pos_end = 0;
                }else
                {
                    try
                    {
                        Array.Copy(datas, pos_start, datas, 0, pos_end - pos_start);
                        pos_end = pos_end - pos_start;
                        pos_start = 0;
                    }
                    catch (Exception e)
                    {

                    }

                }
                return rets;
            }
        }
    }
}
