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
            string str = System.Text.Encoding.Default.GetString(data,0,len);
            protocal.ParseStr(str);
            return protocal;
        }

        public void ParseStr(string str)
        {
            // TODO: test
            //    RFID(a57245545):VEST(989123)
            this.State = ProtocolParseState.PPSParsing;
            int rs = str.IndexOf("RFID(");
            int vs = str.IndexOf("VEST(");
            if (rs >= 0)
            {
                int re = str.IndexOf(")", rs);
                if (re > rs + 5)
                {
                    string rfidNo = str.Substring(rs + 5, re - rs - 5);
                    var devRfid = (new PFTSModel.Services.DevRFIDService()).GetByNo(rfidNo);
                    if (devRfid != null)
                    {
                        this.DevRFID = devRfid;
                    }
                }
            }
            if (vs >= 0)
            {
                int ve = str.IndexOf(")", vs);
                if (ve > vs + 5)
                {
                    string vestNo = str.Substring(vs + 5, ve - vs - 5);
                    var bt = (new PFTSModel.BTrackerService()).GetByVestNo(vestNo);
                    if (bt != null)
                    {
                        this.BTracker = bt;
                    }
                }
            }
            if (this.DevRFID != null && this.BTracker != null)
            {
                this.Type = ProtocolType.PTRFIDTrigger;
                this.State = ProtocolParseState.PPSParsed;
            }

        }
    }
}
