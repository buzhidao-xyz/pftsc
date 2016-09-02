using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PFTSHwCtrl
{
    public delegate void RFIDNoReaderHandler(List<String> rfidNos);

    /// <summary>
    /// RFID标签读取,使用串口通信
    /// </summary>
    public class PFTSRFIDNoReaderProxy
    {
        private string m_com;
        private int m_baudRate = 57600;
        private SerialPort m_serialPort;
        private bool m_bIsTimeToDie = false;

        public event RFIDNoReaderHandler RFIDNoReaderDelegate;

        public PFTSRFIDNoReaderProxy(string com)
        {
            m_com = com;
            m_serialPort = new SerialPort();
        }

        public void Open()
        {
            m_serialPort.PortName = m_com;
            m_serialPort.BaudRate = m_baudRate;
            m_serialPort.Parity = System.IO.Ports.Parity.None;
            m_serialPort.DataBits = 8;
            m_serialPort.StopBits = System.IO.Ports.StopBits.One;
            m_serialPort.ReadBufferSize = 4096;
            m_serialPort.DataReceived += M_serialPort_DataReceived;
            m_serialPort.ReceivedBytesThreshold = 1;
            m_serialPort.Open();
        }

        public bool SendData(byte[] SendArray)
        {
            try
            {
                m_serialPort.Write(SendArray, 0, SendArray.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void StartAcquireRFIDNo()
        {
            Thread captureThread = new Thread(new ThreadStart(DoRead));
            captureThread.IsBackground = true;
            captureThread.Start();
            m_bIsTimeToDie = false;
        }

        private void DoRead()
        {
            while (!m_bIsTimeToDie)
            {
                byte[] queryCmd = { 0x09, 0x00, 0x01, 0x04, 0x00, 0x00, 0x80, 0x0A, 0x22, 0xDA };
                SendData(queryCmd);
                Thread.Sleep(200);
            }
        }

        public void Close()
        {
            m_serialPort.Close();
        }

        private void M_serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            System.Threading.Thread.Sleep(50);      //延时一定时间用于接收数据

            int len;
            len = m_serialPort.BytesToRead;//获得接收缓冲区中的字节数
            try
            {
                byte[] receiveArray = new byte[len];
                //从输入缓冲区中读取len个字节
                //并将其放入ReceiveArray中,并清除缓冲区
                int size = m_serialPort.Read(receiveArray, 0, len);
                if (size > 6)
                {
                    if (receiveArray[0] == 0x15 && receiveArray[1] == 0x00 && receiveArray[2] == 0x01
                         && receiveArray[3] == 0x01 && receiveArray[4] == 0x01 && receiveArray[5] == 0x01)
                    {
                        var labels = ReadLabels(receiveArray, size);
                        List<String> ss = new List<string>();
                        foreach (var l in labels)
                        {
                            string s = "";
                            foreach (var ll in l)
                            {
                                s += string.Format("{0:X2}", ll);
                            }
                            ss.Add(s);
                        }
                        if (ss.Count > 0)
                        {
                            if (RFIDNoReaderDelegate != null)
                            {
                                RFIDNoReaderDelegate(ss);
                            }
                        }
                    }
                }
            }
            catch
            {
                //产生错误,返回false
            }
        }

        //  帧头                          标签数量   第一个标签字节数  第一个标签数据            第一个标签校验码
        //  0x15 0x00 0x01 0x01 0x 01     0x02      0x0c    E2 00 30 98 07 08 02 77 18 70 55 92    C8 
        //  
        //  第二个标签字节数    第二个标签数据                             第二个标签校验码      总数据校验码
        //  0x0C                E2 00 20 64 90 09 01 29 16 20 6C 4E         D0                  AD 1A
        private List<byte[]> ReadLabels(byte[] buffer,int size)
        {
            List<byte[]> ret = new List<byte[]>();
           int pos = 5;
            int count = (int)buffer[pos];
            pos++;
            for (int i = 0;i < count; i++)
            {
                if (pos >= size) return ret;
                int isize = (int)buffer[pos];
                pos++;
                if (isize <= 0) return ret;
                byte[] data = new byte[isize];
                for (int j = 0;j < isize; j++)
                {
                    data[j] = buffer[pos + j];
                }
                ret.Add(data);
                pos += isize;
                pos++; //checksum
            }
            return ret;

        }
    }
}
