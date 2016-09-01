using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PFTSHwCtrl
{
    #region handler
    // 点击实时画面
    public delegate void BTrackerMoveToHandler(PFTSModel.btracker btracker, PFTSModel.view_rfid_info position);
    #endregion


    public class PFTSRFIDServer
    {
        #region private member
        private string m_ipAddress;
        private int m_port;
        private Socket m_server;
        #endregion

        public event BTrackerMoveToHandler BTrackerMove;

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
            m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_server.Bind(iep);
            m_server.Listen(20);
            m_server.BeginAccept(new AsyncCallback(Accept), m_server);
        }

        void Accept(IAsyncResult iar)
        {
            //还原传入的原始套接字
            var buffer =new byte[4096];
            Socket myServer = (Socket)iar.AsyncState;
            //在原始套接字上调用EndAccept方法，返回新的套接字
            Socket client = myServer.EndAccept(iar);
            PFTSRFIDProtocol.ProtocolBuffer pbff = new PFTSRFIDProtocol.ProtocolBuffer();
            //while (true)
            //{
                try
                {
                    Receive(client, pbff);
                    //        var rcvSize = client.Receive(buffer);
                    //        if (rcvSize > 0)
                    //        {
                    //            var rets = pbff.Put(buffer, 0, rcvSize);
                    //            if (rets != null && rets.Count > 0)
                    //            {
                    //                foreach (var bt in rets)
                    //                {
                    //                    var protocol = PFTSRFIDProtocol.Parse(bt,bt.Count());
                    //                    if (protocol.State == PFTSRFIDProtocol.ProtocolParseState.PPSParsed)
                    //                    {
                    //                        if (this.BTrackerMove != null)
                    //                        {
                    //                            this.BTrackerMove(protocol.BTracker, protocol.DevRFID);
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            client.Disconnect(false);
                    //            break;
                    //        }
                }
                catch
                {
                    //break;
                }
            //}
        }

        private void Receive(Socket client, PFTSRFIDProtocol.ProtocolBuffer pbff)
        {
            try
            {
                SocketState state = new SocketState();
                state.socket = client;
                state.Pbf = pbff;
                client.BeginReceive(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket     
                // from the asynchronous state object.     
                SocketState state = (SocketState)ar.AsyncState;
                Socket client = state.socket;
                // Read data from the remote device.     
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var rets = state.Pbf.Put(state.buffer, 0, bytesRead);
                    if (rets != null && rets.Count > 0)
                    {
                        foreach (var bt in rets)
                        {
                            var protocol = PFTSRFIDProtocol.Parse(bt, bt.Count());
                            if (protocol.State == PFTSRFIDProtocol.ProtocolParseState.PPSParsed)
                            {
                                if (this.BTrackerMove != null)
                                {
                                    this.BTrackerMove(protocol.BTracker, protocol.DevRFID);
                                }
                            }
                        }
                    }
                }
                Receive(state.socket, state.Pbf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        class SocketState
        {
            public byte[] buffer = new byte[4096];
            public Socket socket;
            public PFTSRFIDProtocol.ProtocolBuffer Pbf;

            public SocketState()
            {

            }
        }
    }
}
