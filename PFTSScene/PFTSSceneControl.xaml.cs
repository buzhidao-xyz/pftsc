using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Threading;

namespace PFTSScene
{

    public enum CameraMode
    {
        // 显示所有位置，高亮显示
        ShowAllPosition,
        // 监控模式，只显示摄像头，并包含操作菜单
        Monitoring,
        // 配置模式，摄像头高亮，空位置灰掉
        Configing,
        // 隐藏所有
        Hidden
    }

    public enum RFIDMode
    {
        // 显示所有位置，高亮显示
        ShowAllPosition,
        // 监控模式，全部隐藏
        Monitoring,
        // 配置模式，摄像头高亮，空位置灰掉
        Configing,
        // 隐藏所有
        Hidden
    }

    #region handler
    // 点击嫌疑犯实时画面
    public delegate void BTrackerRealVideoHandler(PFTSModel.btracker btracker);
    // 点击嫌疑犯路径
    public delegate void BTrackerPathHandler(PFTSModel.btracker btracker);
    // 点击房间实时画面
    public delegate void RoomRealVideoHandler(PFTSModel.view_rfid_room_info room);
    #endregion

    /// <summary>
    /// PFTSSceneControl.xaml 的交互逻辑
    /// </summary>
    public partial class PFTSSceneControl : UserControl
    {
        static PFTSResourceLoader sceneResourceCache = new PFTSResourceLoader();
        private Dictionary<int, Grid> m_mapRooms = new Dictionary<int, Grid>();
        private Dictionary<int, Image> m_mapRfids = new Dictionary<int, Image>();
        private Dictionary<int, Image> m_mapCameras = new Dictionary<int, Image>();
        private Dictionary<int, ContextMenu> m_mapCameraMenu = new Dictionary<int, ContextMenu>();
        private Dictionary<int, Tools.GridRoom> m_mapGridRooms = new Dictionary<int, Tools.GridRoom>();
        private Dictionary<int, int> m_mapPeopleCounts = new Dictionary<int, int>();
        private Dictionary<int, PFTSModel.btracker> m_mapBtrackers = new Dictionary<int, PFTSModel.btracker>();
        private Dictionary<int, Image> m_mapPeopleImage = new Dictionary<int, Image>();
        private List<PFTSModel.btracker> m_listUnloadPeople = new List<PFTSModel.btracker>();
        private int m_prePathBtrackerId = -1;

        private CameraMode m_cameraMode;
        private RFIDMode m_rfidMode;

        private BitmapImage m_bmImgPeople;
        private BitmapImage m_bmImgPeopleAlert;
        // 人物提示名
        private ToolTip m_toolTip;
        private Label m_lblToolTip;
        private ContextMenu m_peopleMenu;
        private bool m_loaded = false;

        /// <summary>
        /// 实时画面回调
        /// </summary>
        public event BTrackerRealVideoHandler BTrackerRealVideoDelegate;
        /// <summary>
        /// 路径回调
        /// </summary>
        public event BTrackerPathHandler BTrackerPathDelegate;

        /// <summary>
        /// 房间实时画面
        /// </summary>
        public event RoomRealVideoHandler RoomRealVideoDelegate;

        private List<InArrow> m_paths = new List<InArrow>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public PFTSSceneControl()
        {
            InitializeComponent();
            // load controls
            loadLocalRooms();
            loadLocalRFIDImages();
            loadLocalCameraImages();
            initGridRooms();

            m_bmImgPeople = new BitmapImage();
            m_bmImgPeople.BeginInit();
            m_bmImgPeople.UriSource = new Uri(@"Images/person_normal.png", UriKind.RelativeOrAbsolute);
            m_bmImgPeople.EndInit();

            m_bmImgPeopleAlert = new BitmapImage();
            m_bmImgPeopleAlert.BeginInit();
            m_bmImgPeopleAlert.UriSource = new Uri(@"Images/person_alert.png", UriKind.RelativeOrAbsolute);
            m_bmImgPeopleAlert.EndInit();

            m_toolTip = new ToolTip();
            m_lblToolTip = new Label();
            m_toolTip.Content = m_lblToolTip;
            m_toolTip.Placement = PlacementMode.Top;

            m_peopleMenu = new ContextMenu();
            var menuItem1 = new MenuItem();
            menuItem1.Header = "实时监控";
            menuItem1.Click += MenuPeopleRealVideo_Click;
            var menuItem2 = new MenuItem();
            menuItem2.Header = "历史轨迹";
            menuItem2.Click += MenuPeoplePath_Click;
            var menuItem3 = new MenuItem();
            menuItem3.Header = "取消";
            m_peopleMenu.Items.Add(menuItem1);
            m_peopleMenu.Items.Add(menuItem2);
            m_peopleMenu.Items.Add(menuItem3);
            m_peopleMenu.Placement = PlacementMode.Top;

            textMsg.ScrollView = scrollView;
        }

        #region Dependency Properties

        public static readonly DependencyProperty RfidImageVisibleProperty = DependencyProperty.Register("RfidImageVisiable", typeof(Visibility), typeof(PFTSSceneControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty CameraImageVisibleProperty = DependencyProperty.Register("CameraImageVisible", typeof(Visibility), typeof(PFTSSceneControl), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        #endregion

        #region CLR Properties

        public Visibility RfidImageVisiable
        {
            get { return (Visibility)base.GetValue(RfidImageVisibleProperty); }
            set { base.SetValue(RfidImageVisibleProperty, value); }
        }

        public Visibility CameraImageVisible
        {
            get { return (Visibility)base.GetValue(CameraImageVisibleProperty); }
            set { base.SetValue(CameraImageVisibleProperty, value); }
        }

        #endregion

        #region 保存场景界面元素
        public void loadLocalRooms()
        {
            for (var i = 0; i < 42; i++)
            {
                object element = this.FindName("room" + (i + 1));
                if (element == null) continue;
                try
                {
                    Grid room = (Grid)element;
                    string tags = room.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapRooms.Add(tagi, room);
                }
                catch { continue; }
            }
        }

        public void loadLocalRFIDImages()
        {
            for (var i = 0; i < 42; i++)
            {
                object element = this.FindName("imgRfid" + (i + 1));
                if (element == null) continue;
                try
                {
                    Image img = (Image)element;
                    Grid room = (Grid)img.Parent;
                    string tags = room.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapRfids.Add(tagi, img);
                }
                catch { continue; }
            }
        }

        public void loadLocalCameraImages()
        {
            for (var i = 0; i < 40; i++)
            {
                object element = this.FindName("imgCamera" + (i + 1));
                if (element == null) continue;
                try
                {
                    Image img = (Image)element;
                    string tags = img.Tag.ToString();
                    int tagi = int.Parse(tags);
                    m_mapCameras.Add(tagi, img);
                }
                catch { continue; }
            }
        }

        public void initGridRooms()
        {
            var rooms = (new PFTSModel.Services.RFIDRoomService()).GetAll();
            foreach (int k in m_mapRooms.Keys)
            {
                var rfrm = from rm in rooms
                           where rm.id == k
                           select rm;

                var gridRoom = new Tools.GridRoom(30, 30, rfrm.SingleOrDefault());
                gridRoom.RealVideoDelegate += GridRoom_RealVideoDelegate;
                m_mapRooms[k].Children.Add(gridRoom);
                m_mapGridRooms[k] = gridRoom;
            }
        }

        private void GridRoom_RealVideoDelegate(PFTSModel.view_rfid_room_info room)
        {
            if (RoomRealVideoDelegate != null)
            {
                RoomRealVideoDelegate(room);
            }
        }

        #endregion

        /// <summary>
        /// 摄像头模式
        /// </summary>
        public CameraMode CameraMode
        {
            get
            {
                return m_cameraMode;
            }
            set
            {
                m_cameraMode = value;
                switch (m_cameraMode)
                {
                    case CameraMode.ShowAllPosition:
                        CameraImageVisible = Visibility.Visible;
                        break;
                    case CameraMode.Hidden:
                        CameraImageVisible = Visibility.Hidden;
                        break;
                    case CameraMode.Monitoring:
                        LoadMonitoringCameraInfos();
                        break;
                    case CameraMode.Configing:
                        LoadConfigCameraInfos();
                        break;
                }
            }
        }

        public RFIDMode RFIDMode
        {
            get
            {
                return m_rfidMode;
            }
            set
            {
                m_rfidMode = value;
                switch (m_rfidMode)
                {
                    case RFIDMode.ShowAllPosition:
                        RfidImageVisiable = Visibility.Visible;
                        break;
                    case RFIDMode.Monitoring:
                    case RFIDMode.Hidden:
                        RfidImageVisiable = Visibility.Hidden;
                        break;
                    case RFIDMode.Configing:
                        LoadConfigRFIDInfos();
                        break;
                }
            }
        }


        /// <summary>
        /// 更新rfid信息,Configing模式
        /// </summary>
        public void LoadConfigRFIDInfos()
        {
            var rfidPositions = PFTSSceneControl.sceneResourceCache.RoomInfos;
            if (rfidPositions == null) return;
            foreach (PFTSModel.view_rfid_room_info pr in rfidPositions)
            {
                if (!m_mapRfids.ContainsKey(pr.id)) continue;
                Image img = m_mapRfids[pr.id];
                if (pr.rfid_room_id == null)
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/rfid_gray.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/rfid.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
            }
        }

        /// <summary>
        /// 更新camera信息,Configing模式
        /// </summary>
        public void LoadConfigCameraInfos()
        {
            var camerapositions = PFTSSceneControl.sceneResourceCache.PositionCameras;
            if (camerapositions == null) return;
            foreach (PFTSModel.view_position_camera_info pr in camerapositions)
            {
                if (!m_mapCameras.ContainsKey(pr.id)) continue;
                Image img = m_mapCameras[pr.id];
                if (pr.dev_camera_id == null)
                {
                    BitmapImage bi = new BitmapImage();
                    // bitmapimage.urisource must be in a begininit/endinit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"images/camera_gray.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // bitmapimage.urisource must be in a begininit/endinit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"images/camera.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                    img.Cursor = Cursors.Hand;
                }
            }
        }

        /// <summary>
        /// 加载相机信息，监控模式
        /// </summary>
        public void LoadMonitoringCameraInfos()
        {
            var cameraPositions = PFTSSceneControl.sceneResourceCache.PositionCameras;
            if (cameraPositions == null) return;
            foreach (PFTSModel.view_position_camera_info pr in cameraPositions)
            {
                if (!m_mapCameras.ContainsKey(pr.id)) continue;
                Image img = m_mapCameras[pr.id];
                if (pr.dev_camera_id == null)
                {
                    img.Visibility = Visibility.Hidden;
                }
                else
                {
                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"Images/camera.png", UriKind.RelativeOrAbsolute);
                    bi.EndInit();
                    img.Source = bi;
                    img.Cursor = Cursors.Hand;
                    var ctxMenu = new ContextMenu();
                    var menuItem1 = new MenuItem();
                    menuItem1.Header = "实时监控";
                    var menuItem2 = new MenuItem();
                    menuItem2.Header = "取消";
                    ctxMenu.Items.Add(menuItem1);
                    ctxMenu.Items.Add(menuItem2);
                    menuItem1.Tag = pr.room_id;
                    menuItem1.Click += CameraRealVideo_Click;
                    m_mapCameraMenu.Add(pr.id, ctxMenu);
                    img.MouseUp += ImgCamera_MouseUp;
                }
            }
        }

        private void CameraRealVideo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            var rid = (int)mi.Tag;
            if (m_mapGridRooms.ContainsKey(rid))
            {
                var rm = m_mapGridRooms[rid].RoomInfo;
                if (RoomRealVideoDelegate != null)
                {
                    RoomRealVideoDelegate(rm);
                }
            }
            
        }

        /// <summary>
        /// 摄像头鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgCamera_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image img = (Image)sender;
                Grid room = (Grid)img.Parent;
                string tags = room.Tag.ToString();
                int tagi = int.Parse(tags);
                if (m_mapCameraMenu.ContainsKey(tagi))
                {
                    var ctxMenu = m_mapCameraMenu[tagi];
                    ctxMenu.PlacementTarget = img;
                    ctxMenu.Placement = PlacementMode.Top;
                    ctxMenu.IsOpen = true;
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 根据房间的路径经过数量计算位置
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="idx"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private Point CalculatePathPoint(FrameworkElement ele, int idx = 0, int count = 1)
        {
            var transform = ele.TransformToAncestor(this.baseGrid);
            Point point;// = transform.Transform(new Point(origin.ActualWidth / 2, origin.ActualHeight / 2));
            var xstep = ele.ActualWidth / 10 > 20.0 ? 20.0 : ele.ActualWidth / 10;
            var ystep = ele.ActualHeight / 10 > 20.0 ? 20.0 : ele.ActualHeight / 10;
            if (count % 2 == 0) //偶数
            {
                //for (var i = 0;i < idx; i++)
                {
                    if (idx % 2 == 0) //左下位置
                    {
                        var x = ele.ActualWidth / 2 - (idx / 2 + 1) * xstep;
                        var y = ele.ActualHeight / 2 + (idx / 2 + 1) * ystep;
                        x = x <= 0 ? ele.ActualWidth / 2 : x;
                        y = y >= ele.ActualHeight ? ele.ActualHeight / 2 : y;
                        point = new Point(x,y);
                    }
                    else //右上位置
                    {
                        var x = ele.ActualWidth / 2 + (idx / 2 + 1) * xstep;
                        var y = ele.ActualHeight / 2 - (idx / 2 + 1) * ystep;
                        x = x >= ele.ActualWidth ? ele.ActualWidth / 2 : x;
                        y = y <= 0 ? ele.ActualHeight / 2 : y;
                        point = new Point(x, y);
                    }
                }
            }else //奇数
            {
                if (idx == 0)
                {
                    point = new Point(ele.ActualWidth / 2, ele.ActualHeight / 2);
                }
                else if (idx % 2 == 0) //左下位置
                {
                    var x = ele.ActualWidth / 2 - (idx / 2) * xstep;
                    var y = ele.ActualHeight / 2 + (idx / 2) * ystep;
                    x = x <= 0 ? ele.ActualWidth / 2 : x;
                    y = y >= ele.ActualHeight ? ele.ActualHeight / 2 : y;
                    point = new Point(x, y);
                }
                else //右上位置
                {
                    var x = ele.ActualWidth / 2 + (idx / 2 + 1) * xstep;
                    var y = ele.ActualHeight / 2 - (idx / 2 + 1) * ystep;
                    x = x >= ele.ActualWidth ? ele.ActualWidth / 2 : x;
                    y = y <= 0 ? ele.ActualHeight / 2 : y;
                    point = new Point(x, y);
                }
            }
            return transform.Transform(point);
        }

        /// <summary>
        /// 画不同房间之间的路径（包含方向）
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dest"></param>
        private void PathTo(Grid origin, FrameworkElement dest,int originIndex = 0,int originCount = 1,int destIndex = 0,int destCount = 1)
        {
            //var transformStart = origin.TransformToAncestor(this.baseGrid);
            //Point pointStart = transformStart.Transform(new Point(origin.ActualWidth / 2, origin.ActualHeight / 2));
            Point pointStart = CalculatePathPoint(origin, originIndex, originCount);

            //var transformEnd = dest.TransformToAncestor(this.baseGrid);
            //Point pointEnd = transformEnd.Transform(new Point(dest.ActualWidth / 2, dest.ActualHeight / 2));
            Point pointEnd = CalculatePathPoint(dest, destIndex, destCount);

            var arrow = new Tools.Arrow();
            arrow.X1 = pointStart.X;
            arrow.Y1 = pointStart.Y;
            arrow.X2 = pointEnd.X;
            arrow.Y2 = pointEnd.Y;
            arrow.HeadWidth = 10;
            arrow.HeadHeight = 5;
            arrow.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            arrow.StrokeThickness = 2;
            this.gridPaths.Children.Add(arrow);
            var ia = new InArrow();
            ia.ArrowD = arrow;
            ia.RoomOrigin = origin;
            ia.RoomDest = dest;
            m_paths.Add(ia);
        }

        private void TimeOut(FrameworkElement ele, DateTime tm, int idx = 0, int count = 1)
        {
            //var transform = ele.TransformToAncestor(this.baseGrid);
            //Point point = transform.Transform(new Point(ele.ActualWidth / 2, ele.ActualHeight / 2));
            Point point = CalculatePathPoint(ele, idx, count);

            var lbl = new Label();
            lbl.Content = tm.ToString("HH:mm:ss");
            var w = 60;
            var h = 30;
            lbl.Margin = new Thickness(point.X - w/2, point.Y-h/2, this.gridPaths.ActualWidth - point.X-w/2,this.gridPaths.ActualHeight - point.Y-h/2);
            this.gridPaths.Children.Add(lbl);
        }

        private void RefreshPath(InArrow ia)
        {
            var transformStart = ia.RoomOrigin.TransformToAncestor(this.baseGrid);
            Point pointStart = transformStart.Transform(new Point(ia.RoomOrigin.ActualWidth / 2, ia.RoomOrigin.ActualHeight / 2));

            var transformEnd = ia.RoomDest.TransformToAncestor(this.baseGrid);
            Point pointEnd = transformEnd.Transform(new Point(ia.RoomDest.ActualWidth / 2, ia.RoomDest.ActualHeight / 2));

            ia.ArrowD.X1 = pointStart.X;
            ia.ArrowD.Y1 = pointStart.Y;
            ia.ArrowD.X2 = pointEnd.X;
            ia.ArrowD.Y2 = pointEnd.Y;
        }


        public void AddAPeople(PFTSModel.btracker btracker,bool opt = true)
        {
            if (btracker.room_id == null) return;
            int roomId = btracker.room_id.Value;
            if (m_mapGridRooms.ContainsKey(roomId))
            {
                var gr = m_mapGridRooms[roomId];
                Image img = new Image();
                bool alert = false;
                if (btracker.room_id == 2)  // 2:采集室 1:搜身室
                {
                    if (btracker.frisk_status != 1) //未搜身
                    {
                        alert = true;
                    }
                }
                else if (btracker.room_id > 2)
                {
                    if (btracker.frisk_status != 1 || btracker.gather_status != 1) //未搜身或者未采集
                    {
                        alert = true;
                    }
                }
                if (alert)
                {
                    img.Source = m_bmImgPeopleAlert;
                }
                else
                {
                    img.Source = m_bmImgPeople;
                }
                gr.AddAImage(img);
                if (m_mapPeopleCounts.ContainsKey(roomId))
                {
                    m_mapPeopleCounts[roomId] += 1;
                }
                else
                {
                    m_mapPeopleCounts[roomId] = 1;
                }
                if (opt)
                {
                    img.Tag = btracker.id;
                    img.Cursor = Cursors.Hand;
                    img.MouseEnter += Img_MouseEnter;
                    img.MouseLeave += Img_MouseLeave;
                    img.MouseUp += Img_MouseUp;
                    //[yyyy年MM-dd HH:mm:ss]
                    if (btracker.in_room_time == null) return;
                    string info = string.Format("[{0}]{1}进入了房间{2}", btracker.in_room_time.Value.ToString("yyyy年MM-dd HH:mm:ss"), btracker.name, gr.RoomInfo.name);
                    //textMsg.Debug(btracker.name + "进入了房间(" + gr.RoomInfo.name + ")",false);
                    textMsg.Debug(info, false);
                }
                m_mapBtrackers.Add(btracker.id, btracker);
                m_mapPeopleImage.Add(btracker.id, img);
            }
        }

        public void SetPeopleImageLoadedCallback(int btrackerId,EventHandler call)
        {
            if (m_mapPeopleImage.ContainsKey(btrackerId))
            {
                var img = m_mapPeopleImage[btrackerId];
                if (img.IsLoaded) call(null, null);
                else
                {
                    img.Loaded += delegate (object sender, RoutedEventArgs e)
                    {
                        call(null, null);
                    };
                }
            }
        }

        public void RefreshPeople(int id)
        {
            //异步刷新界面
            PFTSModel.btracker btracker = (new PFTSModel.Services.BTrackerService()).Get(id);
            if (btracker == null) return;
            if (m_mapBtrackers.ContainsKey(id))
            {
                PFTSModel.btracker oldBt = m_mapBtrackers[id];
                // same room
                if (oldBt.room_id == btracker.room_id) return;
                // leave

                if (btracker.room_id == null)
                {
                    RemovePeople(btracker.id, oldBt.room_id.Value);
                }
                else
                {
                    MovePeople(btracker, oldBt.room_id.Value);
                }
            }
            else if (btracker.status == 0 && btracker.room_id != null)
            {
                AddAPeople(btracker);
            }
        }

        public void RemovePeople(int id, int roomId)
        {
            if (m_mapPeopleCounts.ContainsKey(roomId))
            {
                m_mapPeopleCounts[roomId] = m_mapPeopleCounts[roomId] - 1;
                if (m_mapPeopleCounts[roomId] <= 0)
                {
                    m_mapPeopleCounts.Remove(roomId);
                }
                if (m_mapPeopleImage.ContainsKey(id))
                {
                    Image img = m_mapPeopleImage[id];
                    if (m_mapGridRooms.ContainsKey(roomId))
                    {
                        Tools.GridRoom gd = m_mapGridRooms[roomId];
                        gd.RemoveAImage(img);
                    }
                    if (img != null)
                    {
                        m_mapPeopleImage.Remove(id);
                    }
                }
                if (m_mapBtrackers.ContainsKey(id) && m_mapRooms.ContainsKey(roomId))
                {
                    var bt = m_mapBtrackers[id];
                    textMsg.Debug(bt.name + "离开了房间("+m_mapGridRooms[roomId].RoomInfo.name+")",false);
                }
                m_mapBtrackers.Remove(id);
            }
        }

        public void MovePeople(PFTSModel.btracker newBtracker, int oldRoom)
        {
            if (oldRoom == newBtracker.room_id) return;
            RemovePeople(newBtracker.id, oldRoom);
            AddAPeople(newBtracker);
        }

        public void RefreshPeoples()
        {
            foreach (int i in m_mapPeopleImage.Keys)
            {
                Image img = m_mapPeopleImage[i];
                Grid gd = (Grid)img.Parent;
                gd.Children.Remove(img);
            }
            foreach (var bt in m_listUnloadPeople)
            {
                AddAPeople(bt);
            }
        }

        public void PrePathOut(int btrackerId)
        {
            if (this.IsLoaded)
            {
                PathOut(btrackerId, false);
            }else
            {
                m_prePathBtrackerId = btrackerId;
            }
        }

        public void PathOut(int btrackerId, bool opt = true)
        {
            var service = new PFTSModel.Services.BTrackerService();
            var btr = service.Get(btrackerId);
            var paths = service.GetPaths(btrackerId);
            Dictionary<int, int> ocs = new Dictionary<int, int>();
            Dictionary<int, int> dcs = new Dictionary<int, int>();
            Dictionary<int, int> oidx = new Dictionary<int, int>();
            Dictionary<int, int> didx = new Dictionary<int, int>();
            foreach ( var p in paths)
            {
                if (ocs.ContainsKey(p.start_room_id))
                {
                    ocs[p.start_room_id]++;
                }
                else
                {
                    ocs[p.start_room_id] = 1;
                }
                if (dcs.ContainsKey(p.end_room_id))
                {
                    dcs[p.end_room_id]++;
                }
                else
                {
                    dcs[p.end_room_id] = 1;
                }
            }
            // 场内
            if (btr.status == 0 && m_mapPeopleImage.ContainsKey(btrackerId) && paths.Count > 0)
            {
                for (var i = 0;i < paths.Count - 1; i++)
                {
                    var p = paths[i];
                    int o = p.start_room_id;
                    int d = p.end_room_id;
                    if (m_mapRooms.ContainsKey(o) && m_mapRooms.ContainsKey(d))
                    {
                        var odx = oidx.ContainsKey(o) ? oidx[o] : 0;
                        var ddx = didx.ContainsKey(d) ? didx[d] : 0;
                        var oct = ocs.ContainsKey(o) ? ocs[o] : 1;
                        var dct = dcs.ContainsKey(d) ? dcs[d] : 1;
                        if (oidx.ContainsKey(o)) oidx[o]++;
                        else oidx[o] = 1;
                        if (didx.ContainsKey(d)) didx[d]++;
                        else didx[d] = 1;
                        PathTo(m_mapRooms[o], m_mapRooms[d],odx,oct,ddx,dct);
                        TimeOut(m_mapRooms[o], p.start_time, odx, oct);
                    }
                }
                int oe = paths[paths.Count - 1].start_room_id;
                PathTo(m_mapRooms[oe], m_mapPeopleImage[btrackerId],0,1,0,1);
                if (paths.Count > 0)
                {
                    TimeOut(m_mapRooms[oe], paths[paths.Count - 1].start_time, 0, 1);
                    TimeOut(m_mapPeopleImage[btrackerId], paths[paths.Count - 1].end_time, 0, 1);
                }
            }
            else
            {
                foreach (var p in paths)
                {
                    int o = p.start_room_id;
                    int d = p.end_room_id;
                    if (m_mapRooms.ContainsKey(o) && m_mapRooms.ContainsKey(d))
                    {
                        var odx = oidx.ContainsKey(o) ? oidx[o] : 0;
                        var ddx = didx.ContainsKey(d) ? didx[d] : 0;
                        var oct = ocs.ContainsKey(o) ? ocs[o] : 1;
                        var dct = dcs.ContainsKey(d) ? dcs[d] : 1;
                        PathTo(m_mapRooms[o], m_mapRooms[d], odx, oct, ddx, dct);
                        TimeOut(m_mapRooms[o], p.start_time, odx, oct);
                    }
                }
                if (paths.Count > 0)
                {
                    TimeOut(m_mapRooms[paths[paths.Count - 1].end_room_id], paths[paths.Count - 1].end_time,0,1);
                }
            }
            if (opt)
            {
                gridBack.Visibility = Visibility.Visible;
            }
            textMsg.Clear();
            var bt = (new PFTSModel.Services.BTrackerService()).GetInfoById(btrackerId);
            textMsg.Debug("姓名：" + bt.name, false);
            textMsg.Debug("性别：" + bt.sex, false);
            textMsg.Debug("档案号：" + bt.no, false);
            if (bt.officer_id != null) textMsg.Debug("负责民警：" + bt.officer_name, false);
            if (bt.vest_id != null) textMsg.Debug("马甲：" + bt.vest_name, false);
            if (paths.Count > 0)
            {
                textMsg.Debug("入场时间：" + paths[0].start_time.ToString("yyyy-MM-dd HH:mm:ss"), false);
            }
            if (btr.status == 0 && m_mapPeopleImage.ContainsKey(btrackerId) && paths.Count > 0)
            {

            }
            else if (paths.Count > 0)
                textMsg.Debug("离场时间：" + paths[paths.Count - 1].end_time.ToString("yyyy-MM-dd HH:mm:ss"), false);
        }

        private void MenuPeopleRealVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = (int)m_peopleMenu.Tag;
                if (m_mapBtrackers.ContainsKey(id))
                {
                    if (BTrackerRealVideoDelegate != null)
                    {
                        this.BTrackerRealVideoDelegate(m_mapBtrackers[id]);
                    }
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void MenuPeoplePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = (int)m_peopleMenu.Tag;
                PathOut(id);
                if (BTrackerPathDelegate != null && m_mapBtrackers.ContainsKey(id))
                {
                    this.BTrackerPathDelegate(m_mapBtrackers[id]);
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            try
            {
                int id = int.Parse(img.Tag.ToString());
                m_peopleMenu.Tag = id;
            }
            catch
            {

            }
            this.Img_MouseLeave(sender, null);
            m_peopleMenu.PlacementTarget = (Image)sender;
            m_peopleMenu.IsOpen = true;
            e.Handled = true;
        }

        private void Img_MouseLeave(object sender, MouseEventArgs e)
        {
            m_toolTip.IsOpen = false;
        }

        private void Img_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            try
            {
                int id = int.Parse(img.Tag.ToString());
                if (m_mapBtrackers.ContainsKey(id))
                {
                    var bt = m_mapBtrackers[id];
                    m_lblToolTip.Content = bt.name;
                }
            }
            catch
            {

            }
            m_toolTip.PlacementTarget = img;
            m_toolTip.IsOpen = true;
            //e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            m_loaded = true;
            //foreach (var gr in m_mapGridRooms)
            //{
            //    gr.Value.MetaSize = new Size(30, 30);
            //}
            //foreach (var bt in m_listUnloadPeople)
            //{
            //    AddAPeople(bt);
            //}
            //m_listUnloadPeople = new List<PFTSModel.btracker>();
            if (m_prePathBtrackerId >= 0)
            {
                PathOut(m_prePathBtrackerId, false);
                m_prePathBtrackerId = -1;
            }
        }

        public Tools.TextBlockDebug DebugBlock
        {
            get
            {
                return textMsg;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //foreach (var pc in m_mapPeopleCounts)
            //{
            //    if (pc.Value > 0)
            //    {
            //        if (m_mapGridRooms.ContainsKey(pc.Key))
            //        {
            //            m_mapGridRooms[pc.Key].Resize();
            //        }
            //    }
            //}
            foreach (var p in m_paths)
            {
                RefreshPath(p);
            }
        }

        public struct InArrow
        {
            public Tools.Arrow ArrowD;
            public Grid RoomOrigin;
            public FrameworkElement RoomDest;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var p in m_paths)
            //{
            //    Grid g = (Grid)p.ArrowD.Parent;
            //    g.Children.Remove(p.ArrowD);
            //}
            this.gridPaths.Children.Clear();
            m_paths = new List<InArrow>();
            gridBack.Visibility = Visibility.Hidden;
            textMsg.Clear();
        }
    }
}
