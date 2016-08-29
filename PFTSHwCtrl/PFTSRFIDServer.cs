using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PFTSHwCtrl
{
    public class PFTSRFIDServer
    {
        #region private member
        private string m_ipAddress;
        private int m_port;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PFTSRFIDServer(string ipAddress,int port)
        {
            this.m_ipAddress = ipAddress;
            this.m_port = port;
        }

        public void Start()
        {
            IPAddress local = IPAddress.Parse(this.m_ipAddress);
            IPEndPoint iep = new IPEndPoint(local, this.m_port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(20);
            server.BeginAccept(new AsyncCallback(Accept), server);
        }

        void Accept(IAsyncResult iar)
        {
            //还原传入的原始套接字
            var buffer =new byte[4096];
            Socket myServer = (Socket)iar.AsyncState;
            //在原始套接字上调用EndAccept方法，返回新的套接字
            Socket client = myServer.EndAccept(iar);
            try
            {
                var rcvSize = client.Receive(buffer);
                if (rcvSize > 0)
                {

                }
            }
            catch
            {

            }
        }
    }
}
