using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public PFTSRFIDServer(string ipAddress, int port)
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
            PFTSTools.ConsoleManager.SetOut("开启TCP服务,监听地址:[" + this.m_ipAddress + ":" + this.m_port + "]");
            Thread tcpThread = new Thread(new ThreadStart(TcpListen));
            tcpThread.Start();
        }

        private void TcpListen()
        {
            while (true)
            {
                try
                {
                    Socket client = m_server.Accept();
                    ClientThread newClient = new ClientThread(this, client);
                    Thread newThread = new Thread(new ThreadStart(newClient.ClientService));
                    newThread.Start();
                }
                catch { }
            }
        }

        void Accept(IAsyncResult iar)
        {
            //还原传入的原始套接字
            var buffer = new byte[4096];
            Socket myServer = (Socket)iar.AsyncState;
            //在原始套接字上调用EndAccept方法，返回新的套接字
            Socket client = myServer.EndAccept(iar);
            PFTSRFIDProtocol.ProtocolBuffer pbff = new PFTSRFIDProtocol.ProtocolBuffer();
            try
            {
                Receive(client, pbff);
            }
            catch
            {
                //break;
            }
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

        class ClientThread
        {
            private PFTSRFIDServer m_server;
            private Socket m_client = null;
            private byte[] m_buffer = new byte[4096];
            //public Socket socket;
            private PFTSRFIDProtocol.ProtocolBuffer m_pbf = new PFTSRFIDProtocol.ProtocolBuffer();

            public ClientThread(PFTSRFIDServer server, Socket k)
            {
                m_server = server;
                m_client = k;
            }
            public void ClientService()
            {
                try
                {
                    PFTSTools.ConsoleManager.SetOut("收到一个TCP客户端连接:"+m_client.RemoteEndPoint);
                    while (true)
                    {
                        int bytesRead = m_client.Receive(m_buffer);
                        if (bytesRead > 0)
                        {
                            PFTSTools.ConsoleManager.SetOut("接受到" + bytesRead + "字节");
                            var rets = m_pbf.Put(m_buffer, 0, bytesRead);
                            if (rets != null && rets.Count > 0)
                            {
                                foreach (var bt in rets)
                                {
                                    var protocol = PFTSRFIDProtocol.Parse(bt, bt.Count());
                                    if (protocol.State == PFTSRFIDProtocol.ProtocolParseState.PPSParsed)
                                    {
                                        if (this.m_server.BTrackerMove != null)
                                        {
                                            this.m_server.BTrackerMove(protocol.BTracker, protocol.DevRFID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (System.Exception exp)
                {
                    //Console.WriteLine(exp.ToString());
                }
                PFTSTools.ConsoleManager.SetOut("TCP客户端断开连接:" + m_client.RemoteEndPoint);
                m_client.Close();
            }
        }

    }
}
